using Engine.Rendering.Contexts.Services;
using Engine.Rendering.OGL;
using Engine.Rendering.Services;
using Engine.Structure;
using Engine.Structure.Interfaces;
using Engine.Structure.ServiceProvider;
using OpenGL;

namespace Engine.Rendering;

public class RenderModule : ModuleBase<IRenderService>, ISystem, IInitializable, IUpdateable, IDisposable {

	//A module should have it's own thread.
	//The thread here will do the frame loop. The render module is the only place OpenGL and Glfw calls are made.
	//Multiple windows are allowed, using a windows service
	private readonly ServiceProviderUpdateExtension _serviceProviderUpdater;
	private readonly ServiceProviderInitializationExtension _serviceProviderInitializer;
	private readonly ServiceProviderDisposalExtension _serviceProviderDisposer;

	public bool SystemEssential => true;

	public RenderModule() : base( true ) {
		_serviceProviderUpdater = new( _serviceProvider );
		_serviceProviderInitializer = new( _serviceProvider );
		_serviceProviderDisposer = new( _serviceProvider );
	}

	public void Initialize() {
		Gl.Initialize();
		Get<GlDebugMessageService>().BindErrorCallback();
		GlfwUtilities.Init();
		//TODO: Fetch window saved settings
		Get<WindowService>().Create( new WindowSettings() );
		//Add other services. The rendermodule should be a selfcontained module extracting information from other modules
		//For voxel rendering use a mesh renderer inside a rendercomponent
		OnInitialize();
	}

	public void Update( float time, float deltaTime ) {
		_serviceProviderInitializer.Update( time, deltaTime );
		GlfwUtilities.PollEvents();
		_serviceProviderUpdater.Update( time, deltaTime );
		if ( !Get<WindowService>().Any() )
			Stop();
	}

	protected virtual void OnInitialize() { }

	public void Dispose() {
		_serviceProviderDisposer.Dispose();
		GlfwUtilities.Terminate();
	}
}
