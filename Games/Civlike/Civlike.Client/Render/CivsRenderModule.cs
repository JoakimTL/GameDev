using Civlike.Client.Render.Ui;
using Civlike.Messages;
using Engine;
using Engine.Modularity;
using Engine.Module.Render;
using Engine.Module.Render.Ogl;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard;
using Engine.Standard.Render;
using Engine.Standard.Render.UserInterface;
using OpenGL;

namespace Civlike.Client.Render;

public sealed class CivsRenderModule : RenderModuleBase {

	public CivsRenderModule() : base( "render" ) {
		OnUpdate += Update;
		OnMessageReceived += MessageReceived;
	}

	protected override void ContextAdded( Context context ) {
		context.InstanceProvider.Catalog.Host<Render3Pipeline>();
		//context.InstanceProvider.Catalog.Host<ContextTest>();
		context.InstanceProvider.Catalog.Host<UserInterfaceRenderPipeline>();
		//context.InstanceProvider.Catalog.Host<CameraPanOnTribeCreated>();
		UserInterfaceService ui = context.InstanceProvider.Get<UserInterfaceService>();
		ui.UserInterfaceStateManager.AddAllElements();
		this.InstanceProvider.Get<GameStateService>().SetNewState( UiElementConstants.ShowStartMenu, true );
		this.InstanceProvider.Get<GameStateService>().SetNewState( UiElementConstants.ShowFPSCounter, true );
		context.OnInitialized += OnContextInitialized;
	}

	private void OnContextInitialized( Context context ) {
		Gl.Enable( EnableCap.Multisample );
		Gl.Enable( EnableCap.CullFace );
	}

	private void MessageReceived( Message message ) {
		if (message.Content is ExitGameMessage)
			this.InstanceProvider.Get<ContextManagementService>().CloseAll();
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
