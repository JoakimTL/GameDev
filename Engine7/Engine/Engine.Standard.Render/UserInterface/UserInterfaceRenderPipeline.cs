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
		this._uiScene = this._sceneService.GetScene( "ui" );
		this._uiSceneCamera = this._dataBlockService.CreateUniformBlockOrThrow( nameof( SceneCameraBlock ), 256, [ ShaderType.VertexShader ] );
		this._cameraSuite = this._cameraService.Get( "ui" );
		this._dataBlockCollection = new( this._uiSceneCamera );
		this._framebuffer = this._framebufferStateService.CreateAutoscalingFramebuffer( this._windowService.Window, 1 );
		this._framebufferGenerator = new( this._framebuffer );
		this._framebufferGenerator.AddTexture( FramebufferAttachment.ColorAttachment0, dimensions => new OglMultisampledTexture( "uiFramebufferColor", TextureTarget.Texture2dMultisample, dimensions, 4, InternalFormat.Rgba8, true ) );

		this._displayedFramebuffer = this._framebufferStateService.CreateAutoscalingFramebuffer( this._windowService.Window, 1 );
		this._displayedFramebufferGenerator = new( this._displayedFramebuffer );
		this._displayTexture = this._displayedFramebufferGenerator.AddTexture( FramebufferAttachment.ColorAttachment0, dimensions => new OglTexture( "uiFramebufferColor", TextureTarget.Texture2d, dimensions, InternalFormat.Rgba8, (TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear), (TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear) ) );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		Gl.Enable( EnableCap.Blend );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		Vector2<float> cameraRotationRight = new( float.Cos( this._cameraSuite.View2.Rotation ), float.Sin( this._cameraSuite.View2.Rotation ) );
		Vector2<float> cameraRotationUp = new( -cameraRotationRight.Y, cameraRotationRight.X );
		this._uiSceneCamera.Buffer.Write( 0u, new SceneCameraBlock( this._cameraSuite.Camera2.Matrix, (cameraRotationUp.X, cameraRotationUp.Y, 0), (cameraRotationRight.X, cameraRotationRight.Y, 0) ) );
		this._framebufferStateService.BindFramebuffer( FramebufferTarget.Framebuffer, this._framebuffer );
		this._framebuffer.Clear( OpenGL.Buffer.Color, 0, [ 0, 0, 0, 0 ] );
		this._uiScene.Render( "default", this._dataBlockCollection, null, PrimitiveType.Triangles );
		this._framebufferStateService.UnbindFramebuffer( FramebufferTarget.Framebuffer );
		this._framebufferStateService.BlitToFrameBuffer( this._framebuffer, this._displayedFramebuffer, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear );
	}

	public void DrawToScreen() {
		//Draw the framebuffer to the screen.
		Gl.Enable( EnableCap.Blend );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		this._textureRenderingService.RenderTexture( this._displayTexture.TextureReference.GetHandle() );
	}

	protected override bool InternalDispose() {
		this._framebuffer.Dispose();
		this._framebufferGenerator.Dispose();
		this._displayedFramebuffer.Dispose();
		this._displayedFramebufferGenerator.Dispose();
		return true;
	}

}