using System.Runtime.InteropServices;
using Engine.Data.Buffers;
using OpenGL;

namespace Engine.Rendering.Standard.Scenes;

public abstract class Scene : DisposableIdentifiable, IScene {

	private readonly List<ISceneObject> _sceneObjects;
	private readonly List<RenderStage> _renderStages;
	private readonly List<IDataSegment> _segments;
	private readonly VertexBufferDataObject _commandBuffer;
	private readonly AutoResetEvent _lock;
	private bool _updated;

	public uint Count => (uint) this._sceneObjects.Count;
	public int RenderCount { get; private set; }
	public uint RenderStages => (uint) this._renderStages.Count;

	protected override string UniqueNameTag => $"{this._sceneObjects.Count}";

	public Scene() {
		this._sceneObjects = new List<ISceneObject>();
		this._renderStages = new List<RenderStage>();
		this._commandBuffer = new VertexBufferDataObject( "Scene Indirect", ushort.MaxValue * (uint) Marshal.SizeOf<IndirectCommand>(), BufferUsage.DynamicDraw, (uint) Marshal.SizeOf<IndirectCommand>() );
		this._segments = new List<IDataSegment>();
		this._lock = new AutoResetEvent( true );
	}

	public void AddSceneObject( ISceneObject sceneObject ) {
		this._lock.WaitOne();
		this._sceneObjects.Add( sceneObject );
		this._updated |= sceneObject.Valid;
		sceneObject.RenderPropertiesChanged += Changed;
		sceneObject.SceneObjectDisposed += DisposedSceneObject;
		if ( this._segments.Count < this._sceneObjects.Count )
			this._segments.Add( this._commandBuffer.Buffer.AllocateSynchronized( (uint) Marshal.SizeOf<IndirectCommand>() ) );
		this._lock.Set();
	}

	private void DisposedSceneObject( ISceneObject obj ) => RemoveSceneObject( obj );

	public void RemoveSceneObject( ISceneObject sceneObject ) {
		this._lock.WaitOne();
		this._sceneObjects.Remove( sceneObject );
		this._updated |= sceneObject.Valid;
		sceneObject.RenderPropertiesChanged -= Changed;
		sceneObject.SceneObjectDisposed -= DisposedSceneObject;
		this._lock.Set();
	}

	private void Changed( ISceneObject obj ) => this._updated = true;

	public void Render( IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, uint shaderUse = 0, PrimitiveType prim = PrimitiveType.Triangles ) {
		this._lock.WaitOne();
		UpdateScene();
		RenderScene( dataBlocks, blendActivationFunction, shaderUse, prim );
		this._lock.Set();
	}

	private void RenderScene( IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, uint shaderUse, PrimitiveType prim ) {
		if ( this._renderStages.Count == 0 )
			return;

		//Render
		Gl.BindBuffer( BufferTarget.DrawIndirectBuffer, this._commandBuffer.VBO.BufferId );

		ShaderBundle? currentShader = null;
		VertexArrayObject? currentVAO = null;

		for ( int i = 0; i < this._renderStages.Count; i++ ) {
			RenderStage stage = this._renderStages[ i ];
			ShaderBundle newShaderBundle = stage.Shaders;
			ShaderPipeline? newShader = newShaderBundle.Get( shaderUse );
			if ( newShader is null ) {
				this.LogWarning( $"No shader found for usecase {shaderUse} in {newShaderBundle}" );
				continue;
			}

			if ( newShaderBundle != currentShader ) {
				if ( currentShader is null || currentShader.UsesTransparency != newShaderBundle.UsesTransparency )
					blendActivationFunction?.Invoke( newShaderBundle.UsesTransparency );
				currentShader = newShaderBundle;
				ShaderPipeline s = newShader;
				s.DirectBind();
				dataBlocks?.DirectBindShader( s );
			}

			if ( stage.VAO != currentVAO ) {
				currentVAO = stage.VAO;
				currentVAO.DirectBind();
			}

			Gl.MultiDrawElementsIndirect( prim, DrawElementsType.UnsignedInt, new IntPtr( stage.Offset * (uint) Marshal.SizeOf<IndirectCommand>() ), stage.CommandCount, 0 );
		}

		dataBlocks?.DirectUnbindBuffers();
		blendActivationFunction?.Invoke( false );

	}

	private void UpdateScene() {
		if ( !this._updated )
			return;
		this._updated = false;

		this.RenderCount = 0;
		this._sceneObjects.Sort( SortMethod );

		/*int validCount = 0;
		for ( int i = 0; i < this._sceneObjects.Count; i++ )
			if ( this._sceneObjects[ i ].TryGetIndirectCommand( out IndirectCommand? command ) && command.HasValue ) {
				this._segments[ validCount ].Write( 0, command.Value );
				validCount++;
				RenderCount++;
			}*/

		//this._commandBuffer.ContextUpdate();

		this._renderStages.Clear();
		uint offset = 0;
		uint count = 0;
		ShaderBundle? currentShader = null;
		VertexArrayObject? currentVAO = null;
		bool? currentTransparency = null;

		//This means something changed. We must rebuild the indirect buffers.
		for ( int i = 0; i < this._sceneObjects.Count; i++ ) {
			ISceneObject so = this._sceneObjects[ i ];
			if ( so.ShaderBundle is not null && so.VertexArrayObject is not null && this._sceneObjects[ i ].TryGetIndirectCommand( out IndirectCommand? command ) && command.HasValue ) {
				if ( so.ShaderBundle != currentShader || so.VertexArrayObject != currentVAO ) {
					if ( currentShader is not null && currentVAO is not null && currentTransparency.HasValue ) {
						this._renderStages.Add( new RenderStage( currentVAO, currentShader, (int) count, offset ) );
						offset += count;
					}
					currentShader = so.ShaderBundle;
					currentVAO = so.VertexArrayObject;
					currentTransparency = so.HasTransparency;
					count = 1;
				} else
					count++;
				this._segments[ this.RenderCount++ ].Write( 0, command.Value );
			}
		}

		if ( currentShader is not null && currentVAO is not null )
			this._renderStages.Add( new RenderStage( currentVAO, currentShader, (int) count, offset ) );
	}

	protected abstract int SortMethod( ISceneObject x, ISceneObject y );

	protected override bool OnDispose() {
		this._commandBuffer.Dispose();
		return true;
	}

	private class RenderStage {

		public readonly VertexArrayObject VAO;
		public readonly ShaderBundle Shaders;
		/// <summary>
		/// The number of commands in this stage
		/// </summary>
		public readonly int CommandCount;
		/// <summary>
		/// The offset of the drawcall!
		/// </summary>
		public readonly uint Offset;

		public RenderStage( VertexArrayObject vao, ShaderBundle bundle, int commandCount, uint offset ) {
			this.VAO = vao;
			this.Shaders = bundle;
			this.CommandCount = commandCount;
			this.Offset = offset;
		}

	}
}
