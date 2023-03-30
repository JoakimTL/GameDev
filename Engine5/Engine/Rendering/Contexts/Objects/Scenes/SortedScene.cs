using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Contexts.Objects.Scenes;

public unsafe class SortedScene : Identifiable, IDisposable {

	public string ShaderIndex { get; }
	private readonly List<SceneObjectUsage> _sortedSceneObjects;
	private readonly List<SceneRenderStage> _renderStages;

	private uint _byteCount;
	private readonly VertexBufferObject _commandBuffer;
	private IndirectCommand* _commands;

	private bool _updated;
	private readonly Scene _scene;
	private readonly uint _initialSize;

	public SortedScene( Scene scene, string shaderIndex, uint initialSize ) {
		_scene = scene;
		ShaderIndex = shaderIndex;
		_initialSize = initialSize;
		_byteCount = initialSize * (uint) sizeof( IndirectCommand );
		_commandBuffer = new VertexBufferObject( "Command Buffer", _byteCount, BufferUsage.DynamicDraw );
		_commands = (IndirectCommand*) NativeMemory.AllocZeroed( _byteCount );
		_sortedSceneObjects = new();
		_renderStages = new();
		_scene.SceneObjectAdded += SceneObjectAdded;
		_scene.SceneObjectRemoved += SceneObjectRemoved;
	}

#if DEBUG
	~SortedScene()
	{
		System.Diagnostics.Debug.Fail($"{this} was not disposed!");
	}
#endif

	private void SceneObjectAdded( ISceneObject so ) {
		so.RenderPropertiesChanged += SceneObjectChanged;
		_sortedSceneObjects.Add( new( so, ShaderIndex ) );
		_updated |= so.Valid;
	}

	private void SceneObjectRemoved( ISceneObject so ) {
		so.RenderPropertiesChanged -= SceneObjectChanged;
		_sortedSceneObjects.RemoveAll( p => p.SceneObject == so );
		_updated |= so.Valid;
	}

	private void SceneObjectChanged( ISceneObject so ) {
		_updated = true;
	}

	private void Expand() {
		uint numCommands = _byteCount / (uint) sizeof( IndirectCommand );
		numCommands += _initialSize;
		_byteCount = numCommands * (uint) sizeof( IndirectCommand );
		_commands = (IndirectCommand*) NativeMemory.Realloc( _commands, _byteCount );
		_commandBuffer.ResizeWrite( (nint) _commands, _byteCount );
	}

	public void Render( IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType prim = PrimitiveType.Triangles ) {
		UpdateScene();
		RenderScene( dataBlocks, blendActivationFunction, prim );
	}

	private void UpdateScene() {
		if ( !_updated )
			return;
		_updated = false;

		while ( _byteCount / (uint) sizeof( IndirectCommand ) < _sortedSceneObjects.Count )
			Expand();

		_sortedSceneObjects.Sort( _scene.SortMethod );

		uint renderCount = 0;
		_renderStages.Clear();
		uint offset = 0;
		uint count = 0;
		ShaderBundleBase? currentShader = null;
		VertexArrayObjectBase? currentVAO = null;
		bool? currentTransparency = null;

		//This means something changed. We must rebuild the indirect buffers.
		for ( int i = 0; i < _sortedSceneObjects.Count; i++ ) {
			ISceneObject so = _sortedSceneObjects[ i ].SceneObject;
			if ( so.ShaderBundle is not null && so.VertexArrayObject is not null && so.TryGetIndirectCommand( out IndirectCommand? command ) && command.HasValue ) {
				if ( so.ShaderBundle != currentShader || so.VertexArrayObject != currentVAO ) {
					if ( currentShader is not null && currentVAO is not null && currentTransparency.HasValue ) {
						_renderStages.Add( new( currentVAO, currentShader, (int) count, offset ) );
						offset += count;
					}
					currentShader = so.ShaderBundle;
					currentVAO = so.VertexArrayObject;
					currentTransparency = so.ShaderBundle?.Get( ShaderIndex )?.UsesTransparency;
					count = 1;
				} else
					count++;
				_commands[ renderCount++ ] = command.Value;
			}
		}

		_commandBuffer.Write( (nint) _commands, 0, 0, renderCount * (uint) sizeof( IndirectCommand ) );

		if ( currentShader is not null && currentVAO is not null )
			_renderStages.Add( new( currentVAO, currentShader, (int) count, offset ) );
	}

	private void RenderScene( IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType prim ) {
		if ( _renderStages.Count == 0 )
			return;

		//Render
		Gl.BindBuffer( BufferTarget.DrawIndirectBuffer, _commandBuffer.BufferId );

		ShaderPipelineBase? currentShader = null;
		VertexArrayObjectBase? currentVAO = null;

		for ( int i = 0; i < _sortedSceneObjects.Count; i++ )
			_sortedSceneObjects[ i ].SceneObject.Bind();

		for ( int i = 0; i < _renderStages.Count; i++ ) {
			SceneRenderStage stage = _renderStages[ i ];
			ShaderBundleBase newShaderBundle = stage.Shaders;
			ShaderPipelineBase? newShader = newShaderBundle.Get( ShaderIndex );
			if ( newShader is null ) {
				this.LogWarning( $"No shader found for usecase {ShaderIndex} in {newShaderBundle}" );
				continue;
			}

			if ( newShaderBundle != currentShader ) {
				if ( currentShader is null || currentShader.UsesTransparency != newShader.UsesTransparency )
					blendActivationFunction?.Invoke( newShader.UsesTransparency );
				currentShader = newShader;
				ShaderPipelineBase s = newShader;
				s.DirectBind();
				dataBlocks?.DirectBindShader( s );
			}

			if ( stage.VAO != currentVAO ) {
				currentVAO = stage.VAO;
				currentVAO.Bind();
			}

			Gl.MultiDrawElementsIndirect( prim, DrawElementsType.UnsignedInt, (nint) ( stage.Offset * (uint) Marshal.SizeOf<IndirectCommand>() ), stage.CommandCount, 0 );
		}

		dataBlocks?.DirectUnbindBuffers();
		blendActivationFunction?.Invoke( false );

	}

	public void Dispose()
	{
		_commandBuffer.Dispose();
		NativeMemory.Free(_commands);
		foreach (var so in _sortedSceneObjects)
			so.SceneObject.RenderPropertiesChanged -= SceneObjectChanged;
		_sortedSceneObjects.Clear();
		_renderStages.Clear();
	}
}
