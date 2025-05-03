using Civs.Messages;
using Engine;
using Engine.Modularity;
using Engine.Module.Render;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard;
using Engine.Standard.Render;
using Engine.Standard.Render.SingleTextureRendering;
using Engine.Standard.Render.UserInterface;
using OpenGL;

namespace Civs.Render;

public sealed class CivsRenderModule : RenderModuleBase {

	public CivsRenderModule() : base( "render" ) {
		OnUpdate += Update;
		OnMessageReceived += MessageReceived;
	}

	protected override void ContextAdded( Context context ) {
		context.InstanceProvider.Catalog.Host<Render3Pipeline>();
		//context.InstanceProvider.Catalog.Host<ContextTest>();
		context.InstanceProvider.Catalog.Host<UserInterfaceRenderPipeline>();
		UserInterfaceService ui = context.InstanceProvider.Get<UserInterfaceService>();
		ui.UserInterfaceStateManager.AddAllElements();
		InstanceProvider.Get<GameStateService>().SetNewState( "showStartMenu", true );
		InstanceProvider.Get<GameStateService>().SetNewState( "showFpsCounter", true );
		context.OnInitialized += OnContextInitialized;
	}

	private void OnContextInitialized( Context context ) {
		Gl.Enable( EnableCap.Multisample );
		Gl.Enable( EnableCap.CullFace );
	}

	private void MessageReceived( Message message ) {
		if (message.Content is ExitGameMessage) {
			InstanceProvider.Get<ContextManagementService>().CloseAll();
		}
	}

	private void Update( double time, double deltaTime ) {
	}
}

public sealed class ContextTest( WindowService windowService, CameraService cameraService ) : Identifiable, IUpdateable, IInitializable {
	private readonly WindowService _windowService = windowService;

	public void Initialize() {
		cameraService.Main.View3.Translation = (0, 0, -2);
	}

	public void Update( double time, double deltaTime ) {
		//this._windowService.Window.Title = $"Time: {time:#,##0.###}s, DeltaTime: {deltaTime:#,##0.###}s, FPS: {(1 / deltaTime):#,##0.###}f/s";
	}
}


public sealed class Render3Pipeline( WindowService windowService, DataBlockService dataBlockService, SceneService sceneService, CameraService cameraService, TextureRenderingService textureRenderingService, FramebufferStateService framebufferStateService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {

	private readonly WindowService _windowService = windowService;
	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly SceneService _sceneService = sceneService;
	private readonly CameraService _cameraService = cameraService;
	private readonly TextureRenderingService _textureRenderingService = textureRenderingService;
	private readonly FramebufferStateService _framebufferStateService = framebufferStateService;

	private Render3PipelineState? _state = null!;

	public void Initialize() {
		_state = new( _windowService, _dataBlockService, _sceneService, _cameraService, _textureRenderingService, _framebufferStateService );
	}

	public void PrepareRendering( double time, double deltaTime ) {
		if (_state is null)
			throw new InvalidOperationException( $"{nameof( Render3Pipeline )} not initialized" );
		_state.SceneCamera.Buffer.Write( 0u, new SceneCameraBlock( _cameraService.Main.Camera3.Matrix, _cameraService.Main.View3.Rotation.Up, -_cameraService.Main.View3.Rotation.Left ) );
		_windowService.Window.Title = $"Camera: {_cameraService.Main.View3.Translation}";

		Gl.Disable( EnableCap.Blend );
		Gl.Enable( EnableCap.DepthTest );
		Gl.DepthMask( true );

		_state.TerrainSceneRenderer.Render( "default", _state.DataBlocks, null, PrimitiveType.Triangles );

		_state.GameObjectSceneRenderer.Render( "default", _state.DataBlocks, null, PrimitiveType.Triangles );

		//_state.GridSceneRenderer.BlitDepthBuffer( _state.TerrainSceneRenderer );

		Gl.Enable( EnableCap.Blend );
		Gl.Disable( EnableCap.DepthTest );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );
		Gl.DepthMask( false );

		_state.GridSceneRenderer.Render( "default", _state.DataBlocks, null, PrimitiveType.Triangles );

	}

	public void DrawToScreen() {
		if (_state is null)
			throw new InvalidOperationException( $"{nameof( Render3Pipeline )} not initialized" );
		Gl.Enable( EnableCap.Blend );
		Gl.Disable( EnableCap.DepthTest );
		Gl.DepthMask( false );
		Gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
		Gl.BlendEquation( BlendEquationMode.FuncAdd );

		_textureRenderingService.RenderTexture( _state.TerrainSceneRenderer.DisplayTexture.TextureReference.GetHandle() );
		_textureRenderingService.RenderTexture( _state.GameObjectSceneRenderer.DisplayTexture.TextureReference.GetHandle() );
		_textureRenderingService.RenderTexture( _state.GridSceneRenderer.DisplayTexture.TextureReference.GetHandle() );

	}

	protected override bool InternalDispose() {
		_state?.Dispose();
		return true;
	}
}