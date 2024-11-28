using Engine.Logging;
using Engine.Modularity;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl;
using Engine.Module.Render.Ogl.Utilities;
using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Module.Render;

public abstract class RenderModuleBase : ModuleBase {

	public RenderModuleBase() : base( true, double.PositiveInfinity ) {
		OnInitialize += InitializeModule;
		OnUpdate += CheckShutdownConditions;
	}

	protected void InitializeModule() {
		Gl.Initialize();
		GlfwUtilities.Init();
		Gl.DebugMessageCallback( OglDebugCallback, nint.Zero );
		this.InstanceProvider.Get<ContextManagementService>().OnContextAdded += ContextAdded;
		this.InstanceProvider.Get<ContextManagementService>().CreateContext( new WindowSettings { DisplayMode = new WindowedDisplayMode( (800, 600) ), Title = "Engine", VSyncLevel = 1 } );
	}

	protected void CheckShutdownConditions( double time, double deltaTime ) {
		if (this.InstanceProvider.Get<ContextManagementService>().ShouldStop)
			Stop();
	}

	private void OglDebugCallback( DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, nint message, nint userParam ) {
		string msg = Marshal.PtrToStringAnsi( message, length );
		this.LogLine( $"OpenGL: {msg}" );
	}

	protected abstract void ContextAdded( Context context );
}
