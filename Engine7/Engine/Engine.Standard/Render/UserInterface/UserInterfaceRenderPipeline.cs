using Engine.Module.Render;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.OOP.Framebuffers;
using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render.SingleTextureRendering;
using OpenGL;

namespace Engine.Standard.Render.UserInterface;
public sealed class UserInterfaceRenderPipeline( SceneService sceneService, DataBlockService dataBlockService, CameraService cameraService, FramebufferStateService framebufferStateService, WindowService windowService, TextureRenderingService textureRenderingService ) : DisposableIdentifiable, IInitializable, IRenderPipeline {
	private readonly SceneService _sceneService = sceneService;
	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly CameraService _cameraService = cameraService;
	private readonly FramebufferStateService _framebufferStateService = framebufferStateService;
	private readonly WindowService _windowService = windowService;
	private readonly TextureRenderingService _textureRenderingService = textureRenderingService;

	private UniformBlock _uiSceneCamera = null!;
	private Scene _uiScene = null!;
	private CameraSuite _cameraSuite = null!;
	private DataBlockCollection _dataBlockCollection = null!;

	private AutoscalingFramebuffer _framebuffer = null!;
	private AutoGeneratingFramebuffer _framebufferGenerator = null!;

	private AutoscalingFramebuffer _displayedFramebuffer = null!;
	private AutoGeneratingFramebuffer _displayedFramebufferGenerator = null!;
	private FramebufferScaledTextureGenerator _displayTexture = null!;

	public void Initialize() {
		_uiScene = _sceneService.GetScene( "ui" );
		_uiSceneCamera = _dataBlockService.CreateUniformBlockOrThrow( nameof( SceneCameraBlock ), 256, [ ShaderType.VertexShader ] );
		_cameraSuite = _cameraService.Get( "ui" );
		_dataBlockCollection = new( _uiSceneCamera );
		_framebuffer = _framebufferStateService.CreateAutoscalingFramebuffer( _windowService.Window, 1 );
		_framebufferGenerator = new( _framebuffer );
		_framebufferGenerator.AddTexture(FramebufferAttachment.ColorAttachment0, dimensions => new OglMultisampledTexture( "uiFramebufferColor", TextureTarget.Texture2dMultisample, dimensions, 4, InternalFormat.Rgba8, true ) );

		_displayedFramebuffer = _framebufferStateService.CreateAutoscalingFramebuffer( _windowService.Window, 1 );
		_displayedFramebufferGenerator = new( _displayedFramebuffer );
		_displayTexture = _displayedFramebufferGenerator.AddTexture( FramebufferAttachment.ColorAttachment0, dimensions => new OglTexture( "uiFramebufferColor", TextureTarget.Texture2d, dimensions, InternalFormat.Rgba8, (TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear), (TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear) ) );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		Gl.Enable( EnableCap.Multisample );
		Gl.Enable( EnableCap.Blend );
		Gl.Disable( EnableCap.CullFace );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		Vector2<float> cameraRotationRight = new( float.Cos( this._cameraSuite.View2.Rotation ), float.Sin( this._cameraSuite.View2.Rotation ) );
		Vector2<float> cameraRotationUp = new( -cameraRotationRight.Y, cameraRotationRight.X );
		_uiSceneCamera.Buffer.Write( 0u, new SceneCameraBlock( _cameraSuite.Camera2.Matrix, (cameraRotationUp.X, cameraRotationUp.Y, 0), (cameraRotationRight.X, cameraRotationRight.Y, 0) ) );
		_framebufferStateService.BindFramebuffer( FramebufferTarget.Framebuffer, _framebuffer );
		_framebuffer.Clear( OpenGL.Buffer.Color, 0, [ 0, 0, 0, 0 ] );
		_uiScene.Render( "default", _dataBlockCollection, null, PrimitiveType.Triangles );
		_framebufferStateService.UnbindFramebuffer( FramebufferTarget.Framebuffer );
		_framebufferStateService.BlitToFrameBuffer( _framebuffer, _displayedFramebuffer, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear );
	}

	public void DrawToScreen() {
		//Draw the framebuffer to the screen.
		Gl.Enable( EnableCap.Blend );
		Gl.Disable( EnableCap.CullFace );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		_textureRenderingService.RenderTexture( _displayTexture.TextureReference.GetHandle() );
	}

	protected override bool InternalDispose() {
		_framebuffer.Dispose();
		_framebufferGenerator.Dispose();
		_displayedFramebuffer.Dispose();
		_displayedFramebufferGenerator.Dispose();
		return true;
	}

}