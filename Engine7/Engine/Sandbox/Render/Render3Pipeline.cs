using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.OOP.Framebuffers;
using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render;
using Engine.Standard.Render.SingleTextureRendering;
using OpenGL;

namespace Sandbox.Render;

public sealed class Render3Pipeline( WindowService windowService, DataBlockService dataBlockService, SceneService sceneService, CameraService cameraService, TextureRenderingService textureRenderingService, FramebufferStateService framebufferStateService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {
	private readonly WindowService _windowService = windowService;
	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly SceneService _sceneService = sceneService;
	private readonly CameraService _cameraService = cameraService;
	private readonly TextureRenderingService _textureRenderingService = textureRenderingService;
	private readonly FramebufferStateService _framebufferStateService = framebufferStateService;

	private UniformBlock _sceneCamera = null!;
	private DataBlockCollection _dataBlocks = null!;
	private Scene _gameObjectScene = null!;
	private Scene _gridScene = null!;

	private AutoscalingFramebuffer _gridFramebuffer = null!;
	private AutoGeneratingFramebuffer _gridFramebufferGenerator = null!;

	private AutoscalingFramebuffer _gridDisplayedFramebuffer = null!;
	private AutoGeneratingFramebuffer _gridDisplayedFramebufferGenerator = null!;
	private FramebufferScaledTextureGenerator _gridDisplayTexture = null!;

	public void Initialize() {
		_sceneCamera = _dataBlockService.CreateUniformBlockOrThrow( nameof( SceneCameraBlock ), 256, [ ShaderType.VertexShader ] );
		this._dataBlocks = new DataBlockCollection( this._sceneCamera );
		_gameObjectScene = _sceneService.GetScene( "gameObjects" );
		_gridScene = _sceneService.GetScene( "grid" );

		_gridFramebuffer = _framebufferStateService.CreateAutoscalingFramebuffer( _windowService.Window, 1 );
		_gridFramebufferGenerator = new( _gridFramebuffer );
		_gridFramebufferGenerator.AddTexture( FramebufferAttachment.ColorAttachment0, dimensions => new OglMultisampledTexture( "gridFramebufferColor", TextureTarget.Texture2dMultisample, dimensions, 4, InternalFormat.Rgba8, true ) );

		_gridDisplayedFramebuffer = _framebufferStateService.CreateAutoscalingFramebuffer( _windowService.Window, 1 );
		_gridDisplayedFramebufferGenerator = new( _gridDisplayedFramebuffer );
		_gridDisplayTexture = _gridDisplayedFramebufferGenerator.AddTexture( FramebufferAttachment.ColorAttachment0, dimensions => new OglTexture( "gridFramebufferColor", TextureTarget.Texture2d, dimensions, InternalFormat.Rgba8, (TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear), (TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear) ) );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		this._sceneCamera.Buffer.Write( 0u, new SceneCameraBlock( _cameraService.Main.Camera3.Matrix, _cameraService.Main.View3.Rotation.Up, -_cameraService.Main.View3.Rotation.Left ) );
		_windowService.Window.Title = $"Camera: {_cameraService.Main.View3.Translation}";

		Gl.Enable( EnableCap.Blend );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		Gl.Enable( EnableCap.Multisample );
		_framebufferStateService.BindFramebuffer( FramebufferTarget.Framebuffer, _gridFramebuffer );
		_gridFramebuffer.Clear( OpenGL.Buffer.Color, 0, [ 0 ] );
		_gridScene.Render( "default", _dataBlocks, null, PrimitiveType.Triangles );
		_framebufferStateService.UnbindFramebuffer( FramebufferTarget.Framebuffer );
		_framebufferStateService.BlitToFrameBuffer( _gridFramebuffer, _gridDisplayedFramebuffer, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear );
	}

	public void DrawToScreen() {
		Gl.Enable( EnableCap.DepthTest );
		Gl.Enable( EnableCap.CullFace );
		Gl.DepthMask( true );
		Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		this._gameObjectScene.Render( "default", this._dataBlocks, _ => { }, PrimitiveType.Triangles );

		Gl.Enable( EnableCap.Blend );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		_textureRenderingService.RenderTexture( _gridDisplayTexture.TextureReference.GetHandle() );
	}

	protected override bool InternalDispose() {
		return true;
	}
}
