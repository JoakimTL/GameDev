using Engine.Module.Render;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.OOP.Textures;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Render.SingleTextureRendering;
using OpenGL;

namespace Engine.Standard.Render.UserInterface;
public sealed class UserInterfaceRenderPipeline : DisposableIdentifiable, IInitializable, IRenderPipeline {
	private readonly SceneService _sceneService;
	private readonly DataBlockService _dataBlockService;
	private readonly CameraService _cameraService;
	private readonly FramebufferStateService _framebufferStateService;
	private readonly WindowService _windowService;
	private readonly TextureRenderingService _textureRenderingService;
	private UniformBlock _uiSceneCamera = null!;
	private Scene _uiScene = null!;
	private CameraSuite _cameraSuite = null!;
	private DataBlockCollection _dataBlockCollection = null!;
	private OglFramebuffer _framebuffer = null!;
	private OglMultisampledTexture _framebufferTexture = null!;
	private OglTextureBase<OglMultisampledTextureMetadata>.TextureReference _textureReference = null!;

	public UserInterfaceRenderPipeline( SceneService sceneService, DataBlockService dataBlockService, CameraService cameraService, FramebufferStateService framebufferStateService, WindowService windowService, TextureRenderingService textureRenderingService ) {
		this._sceneService = sceneService;
		this._dataBlockService = dataBlockService;
		this._cameraService = cameraService;
		this._framebufferStateService = framebufferStateService;
		this._windowService = windowService;
		this._textureRenderingService = textureRenderingService;
	}

	public void Initialize() {
		_uiScene = _sceneService.GetScene( "ui" );
		if (!this._dataBlockService.CreateUniformBlock( nameof( SceneCameraBlock ), 256, [ ShaderType.VertexShader ], out this._uiSceneCamera! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		_cameraSuite = _cameraService.Get( "ui" );
		_dataBlockCollection = new( _uiSceneCamera );
		_framebuffer = _framebufferStateService.CreateFramebuffer( _windowService.Window.Size );
		_framebufferTexture = new OglMultisampledTexture( "uiFramebufferColor", TextureTarget.Texture2dMultisample, _framebuffer.Size, 2, InternalFormat.Rgba8, true );
		_textureReference = _framebufferTexture.GetTextureReference();
		_framebuffer.OnFramebufferGeneration += UiFramebufferGeneration;
		_windowService.Window.OnResized += OnWindowResized;
	}

	private void UiFramebufferGeneration( OglFramebuffer framebuffer ) {
		_framebufferTexture.Dispose();
		_framebufferTexture = new OglMultisampledTexture( "uiFramebufferColor", TextureTarget.Texture2dMultisample, _framebuffer.Size, 2, InternalFormat.Rgba8, true );
		_textureReference = _framebufferTexture.GetTextureReference();
		framebuffer.AttachTexture( FramebufferAttachment.ColorAttachment0, _framebufferTexture.TextureID, 0 );
	}

	private void OnWindowResized( IResizableSurface<int, float> surface ) {
		_framebuffer.Resize( surface.Size );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		Vector2<float> cameraRotationRight = new( float.Cos( this._cameraSuite.View2.Rotation ), float.Sin( this._cameraSuite.View2.Rotation ) );
		Vector2<float> cameraRotationUp = new( -cameraRotationRight.Y, cameraRotationRight.X );
		_uiSceneCamera.Buffer.Write( 0u, new SceneCameraBlock( _cameraSuite.Camera2.Matrix, (cameraRotationUp.X, cameraRotationUp.Y, 0), (cameraRotationRight.X, cameraRotationRight.Y, 0) ) );
		_framebufferStateService.BindFramebuffer( FramebufferTarget.DrawFramebuffer, _framebuffer );
		_framebuffer.Clear( OpenGL.Buffer.Color, 0, [ 0 ] );
		_uiScene.Render( "default", _dataBlockCollection, null, PrimitiveType.Triangles );
		_framebufferStateService.UnbindFramebuffer( FramebufferTarget.DrawFramebuffer );
	}

	public void DrawToScreen() {
		//Draw the framebuffer to the screen.
		_textureRenderingService.RenderTexture( _textureReference.GetHandle() );
	}

	protected override bool InternalDispose() {
		return true;
	}

}