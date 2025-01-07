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
	private OglTextureBase<OglMultisampledTextureMetadata>.TextureReference? _textureReference = null;
	private OglFramebuffer _copyFramebuffer = null!;
	private OglTexture _copyFramebufferTexture = null!;
	private OglTextureBase<OglMipmappedTextureMetadata>.TextureReference? _copyTextureReference = null;

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
		_framebuffer.OnFramebufferGeneration += UiFramebufferGeneration;
		_copyFramebuffer = _framebufferStateService.CreateFramebuffer( _windowService.Window.Size );
		_copyFramebuffer.OnFramebufferGeneration += CopyFramebufferGeneration;
		_windowService.Window.OnResized += OnWindowResized;
	}

	private void CopyFramebufferGeneration( OglFramebuffer framebuffer ) {
		_copyTextureReference = null;
		_copyFramebufferTexture?.Dispose();
		_copyFramebufferTexture = new( "copyFrameBuffer", TextureTarget.Texture2d, _windowService.Window.Size, InternalFormat.Rgba8, (TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear), (TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear) );
		_copyFramebuffer.AttachTexture( FramebufferAttachment.ColorAttachment0, _copyFramebufferTexture.TextureID, 0 );
		_copyFramebuffer.EnableCurrentColorAttachments();
	}

	private void UiFramebufferGeneration( OglFramebuffer framebuffer ) {
		_textureReference = null;
		_framebufferTexture?.Dispose();
		_framebufferTexture = new OglMultisampledTexture( "uiFramebufferColor", TextureTarget.Texture2dMultisample, _framebuffer.Size, 4, InternalFormat.Rgba8, true );
		framebuffer.AttachTexture( FramebufferAttachment.ColorAttachment0, _framebufferTexture.TextureID, 0 );
		framebuffer.EnableCurrentColorAttachments();
	}

	private void OnWindowResized( IResizableSurface<int, float> surface ) {
		if (surface.Size == 0)
			return;
		_framebuffer.DetachTexture( FramebufferAttachment.ColorAttachment0 );
		_framebuffer.Resize( surface.Size );
		_copyFramebuffer.DetachTexture( FramebufferAttachment.ColorAttachment0 );
		_copyFramebuffer.Resize( surface.Size );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		Gl.Enable( EnableCap.Blend );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		Gl.Enable( EnableCap.Multisample );
		Vector2<float> cameraRotationRight = new( float.Cos( this._cameraSuite.View2.Rotation ), float.Sin( this._cameraSuite.View2.Rotation ) );
		Vector2<float> cameraRotationUp = new( -cameraRotationRight.Y, cameraRotationRight.X );
		_uiSceneCamera.Buffer.Write( 0u, new SceneCameraBlock( _cameraSuite.Camera2.Matrix, (cameraRotationUp.X, cameraRotationUp.Y, 0), (cameraRotationRight.X, cameraRotationRight.Y, 0) ) );
		_framebufferStateService.BindFramebuffer( FramebufferTarget.Framebuffer, _framebuffer );
		_framebuffer.Clear( OpenGL.Buffer.Color, 0, [ 0 ] );
		_uiScene.Render( "default", _dataBlockCollection, null, PrimitiveType.Triangles );
		_framebufferStateService.UnbindFramebuffer( FramebufferTarget.Framebuffer );
		_framebufferStateService.BlitToFrameBuffer( _framebuffer, _copyFramebuffer, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear );
	}

	public void DrawToScreen() {
		//Draw the framebuffer to the screen.
		Gl.Enable( EnableCap.Blend );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		_copyTextureReference ??= _copyFramebufferTexture.GetTextureReference();
		_textureRenderingService.RenderTexture( _copyTextureReference.GetHandle() );
	}

	protected override bool InternalDispose() {
		_framebuffer.Dispose();
		_framebufferTexture?.Dispose();
		return true;
	}

}