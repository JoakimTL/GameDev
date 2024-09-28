using Engine.Modules.Rendering.Ogl.OOP;
using Engine.Modules.Rendering.Ogl.Utilities;
using OpenGL;

namespace Engine.Modules.Rendering;

[PrimaryModule]
public sealed class DefaultRenderModule : ModuleBase {

	public DefaultRenderModule() : base( new NoDelayLoopTimer(), new( 1 ), null ) {
	}

	protected override void OnInitialize() {
		Gl.Initialize();
		GlfwUtilities.Init();
		_serviceProvider.GetService<ContextManagementService>().CreateContext( new WindowSettings { DisplayMode = new WindowedDisplayMode( (800, 600) ), Title = "Engine", VSyncLevel = 1 } );
	}

	protected override void OnUpdate( in double time, in double deltaTime ) {
		if (_serviceProvider.GetService<ContextManagementService>().ShouldStop)
			Stop();
	}

	protected override void OnDispose() {

	}
}
