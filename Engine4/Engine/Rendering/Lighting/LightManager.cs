using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.InteropServices;
using Engine.Data.Datatypes;
using Engine.Data.Datatypes.Composite;
using Engine.Data.Datatypes.Projections;
using Engine.Data.Datatypes.Views;
using Engine.Rendering.Framebuffers;
using Engine.Rendering.Lighting.Directional;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Utilities;
using OpenGL;

namespace Engine.Rendering.Lighting;
public class LightManager : DisposableIdentifiable {

	private readonly Scene _scene;
	private readonly ConcurrentQueue<LightBase> _outgoingLights;
	private readonly ConcurrentQueue<LightBase> _incomingLights;
	private readonly List<LightBase> _lights;
	private readonly Dictionary<LightBase, ISceneObject> _renderedLights;
	private readonly Dictionary<LightBase, DirectionalShadowLightRender> _renderedDirectionalShadowedLights;
	private readonly Dictionary<LightBase, IShadowBufferedLight> _shadowBufferedLights;
	private readonly DataBlockCollection _uniforms;
	private readonly UniformBlock _pfxBlock;
	private readonly UniformBlock[] _directionalCascadeBlocks;
	private readonly LightBuffer _buffer;

	public ulong LightBufferTextureHandle => this._buffer.DiffuseTexture?.GetHandleDirect() ?? 0;
	public DirectionalShadowLightRender? DirLight => this._renderedDirectionalShadowedLights.Values.FirstOrDefault();

	public LightManager( Window window, UniformBlock sceneBlock ) {
		this._scene = new LayerlessScene();
		this._outgoingLights = new ConcurrentQueue<LightBase>();
		this._incomingLights = new ConcurrentQueue<LightBase>();
		this._lights = new List<LightBase>();
		this._renderedLights = new Dictionary<LightBase, ISceneObject>();
		this._renderedDirectionalShadowedLights = new Dictionary<LightBase, DirectionalShadowLightRender>();
		this._shadowBufferedLights = new Dictionary<LightBase, IShadowBufferedLight>();
		this._uniforms = new DataBlockCollection(
			sceneBlock,
			this._pfxBlock = new UniformBlock( "PFXBlock", (uint) Marshal.SizeOf<PFXBlock>(), ShaderType.FragmentShader )
		);
		this._directionalCascadeBlocks = new UniformBlock[ CascadeData.NUMCASCADES ];
		for ( int i = 0; i < CascadeData.NUMCASCADES; i++ ) {
			this._directionalCascadeBlocks[ i ] = new UniformBlock( $"DirectionalCascadeBlock[{i}]", (uint) Marshal.SizeOf<CascadeData>(), ShaderType.FragmentShader );
			this._uniforms.AddBlock( this._directionalCascadeBlocks[ i ] );
		}
		this._buffer = new LightBuffer( window );
	}

	public void AddLight( LightBase light ) => this._incomingLights.Enqueue( light );

	public void RemoveLight( LightBase light ) => this._outgoingLights.Enqueue( light );

	public void Update( IMatrixProvider camera, MultiSceneRenderer scenes, View3 cameraView ) {
		while ( this._outgoingLights.TryDequeue( out LightBase? light ) )
			RemoveLightInternal( light );
		while ( this._incomingLights.TryDequeue( out LightBase? light ) )
			AddLightInternal( light );
		foreach ( IShadowBufferedLight? shadowBufferedLight in this._shadowBufferedLights.Values )
			shadowBufferedLight.UpdateShadowBuffers( camera, scenes, cameraView.Translation );
	}

	public void Render( Camera camera, View3 cameraView, Perspective projection, GeometryBuffer gBuffer ) {
		Gl.Disable( EnableCap.DepthTest );
		this._pfxBlock.DirectWrite( new PFXBlock( camera, cameraView, gBuffer, projection.ZNear, projection.ZFar ) );
		this._buffer.Clear();
		this._buffer.Bind();
		Gl.Enable( EnableCap.CullFace );
		Gl.Disable( EnableCap.CullFace );
		Gl.CullFace( CullFaceMode.Front );
		this._scene.Render( this._uniforms, Transparency );
		Gl.CullFace( CullFaceMode.Back );
		Gl.Enable( EnableCap.Blend );
		Gl.BlendFunc( 0, BlendingFactor.One, BlendingFactor.One );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.CullFace );
		foreach ( DirectionalShadowLightRender? directionalShadowedLight in this._renderedDirectionalShadowedLights.Values ) {
			if ( directionalShadowedLight.ShaderBundle is not null && directionalShadowedLight.Mesh is not null ) {
				ShaderPipeline? pipeline = directionalShadowedLight.ShaderBundle.Get( 0 );
				if ( directionalShadowedLight.TryGetIndirectCommand( out IndirectCommand? command ) && command.HasValue && pipeline is not null ) {
					for ( int i = 0; i < directionalShadowedLight.LightCascadeData.Count; i++ )
						this._directionalCascadeBlocks[ i ].DirectWrite( directionalShadowedLight.LightCascadeData[ i ] );
					RenderUtils.Render( pipeline, directionalShadowedLight.VertexArrayObject, command.Value, this._uniforms );
				}
			}
		}
	}

	private void Transparency( bool transparent ) {
		if ( transparent ) {
			Gl.Enable( EnableCap.Blend );
			Gl.BlendFunc( 0, BlendingFactor.One, BlendingFactor.One );
			Gl.BlendEquation( BlendEquationMode.FuncAdd );
		} else {
			Gl.Disable( EnableCap.Blend );
		}
	}

	private void RemoveLightInternal( LightBase light ) {
		if ( light is null ) {
			this.LogError( $"Cannot remove null!" );
			return;
		}
		this._lights.Remove( light );
		if ( this._renderedLights.Remove( light, out ISceneObject? s ) )
			this._scene.RemoveSceneObject( s );
		this._renderedDirectionalShadowedLights.Remove( light );
		this._shadowBufferedLights.Remove( light );
	}

	private void AddLightInternal( LightBase light ) {
		ISceneObject s = light.CreateRender();
		if ( s is null ) {
			this.LogError( $"{light} render was null!" );
			return;
		}

		this._lights.Add( light );
		if ( light is DirectionalLight { HasShadows: true } && s is Directional.DirectionalShadowLightRender dslr ) {
			this._renderedDirectionalShadowedLights.Add( light, dslr );
		} else {
			this._renderedLights.Add( light, s );
			this._scene.AddSceneObject( s );
		}
		if ( s is IShadowBufferedLight sbl )
			this._shadowBufferedLights.Add( light, sbl );
	}

	protected override bool OnDispose() {
		foreach ( IShadowBufferedLight? shadowBufferedLight in this._shadowBufferedLights.Values )
			shadowBufferedLight.Dispose();
		return true;
	}

	[StructLayout( LayoutKind.Explicit )]
	public struct PFXBlock {
		[FieldOffset( 0 )]
		public Matrix4x4 ipvMat;
		[FieldOffset( 64 )]
		public Vector3 eyeTranslation;
		[FieldOffset( 80 )]
		public Vector4 ViewValues;
		[FieldOffset( 96 )]
		public ulong gBufferDiffuse;
		[FieldOffset( 104 )]
		public ulong gBufferNormal;
		[FieldOffset( 112 )]
		public ulong gBufferDepth;
		[FieldOffset( 120 )]
		public ulong gBufferLightingData;

		public PFXBlock( Camera camera, View3 cameraView, GeometryBuffer gBuffer, float zNear, float zFar ) {
			this.ipvMat = camera.InverseMatrix;
			this.eyeTranslation = cameraView.Translation;
			this.gBufferDiffuse = gBuffer.DiffuseTexture?.GetHandleDirect() ?? 0;
			this.gBufferNormal = gBuffer.NormalTexture?.GetHandleDirect() ?? 0;
			this.gBufferDepth = gBuffer.DepthTexture?.GetHandleDirect() ?? 0;
			this.gBufferLightingData = gBuffer.LightPropertiesTexture?.GetHandleDirect() ?? 0;
			this.ViewValues = new Vector4( Viewport.Size.X, Viewport.Size.Y, zNear, zFar );
		}
	}
}
