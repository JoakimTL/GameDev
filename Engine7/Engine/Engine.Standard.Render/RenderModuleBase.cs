using Engine.Modularity;
using Engine.Module.Render;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl;
using Engine.Module.Render.Ogl.Utilities;
using OpenGL;

namespace Engine.Standard.Render;

public abstract class RenderModuleBase : ModuleBase {

	public RenderModuleBase( string address ) : base( true, double.PositiveInfinity, address ) {
		OnInitialize += InitializeModule;
		OnUpdate += CheckShutdownConditions;
	}

	protected void InitializeModule() {
		Gl.Initialize();
		GlfwUtilities.Init();
		this.InstanceProvider.Get<ContextManagementService>().OnContextAdded += InternalContextAdded;
		this.InstanceProvider.Get<ContextManagementService>().OnContextAdded += ContextAdded;
		this.InstanceProvider.Get<ContextManagementService>().CreateContext( new WindowSettings { DisplayMode = new WindowedDisplayMode( (800, 600) ), Title = "Engine", VSyncLevel = 1 } );
	}

	private void InternalContextAdded( Context context ) => context.InstanceProvider.Inject( InstanceProvider.Get<GameStateProvider>(), true );

	protected void CheckShutdownConditions( double time, double deltaTime ) {
		if (this.InstanceProvider.Get<ContextManagementService>().ShouldStop)
			Stop();
	}
	protected abstract void ContextAdded( Context context );
}
