using GLFW;

namespace Engine.Rendering.OGL;

public static class GlfwUtilities {

	public static void InitHint() => Glfw.Init();
	public static void Init() => Glfw.Init();
	public static void Terminate() => Glfw.Terminate();
	public static void PollEvents() => Glfw.PollEvents();
	public static void SetHints( bool debug, int samples, int majorVersion = 4, int minorVersion = 6, Constants forwardCompatible = Constants.True, Profile oglProfile = Profile.Core ) {
		Glfw.WindowHint( Hint.Samples, samples );
		Glfw.WindowHint( Hint.ContextVersionMajor, majorVersion );
		Glfw.WindowHint( Hint.ContextVersionMinor, minorVersion );
		Glfw.WindowHint( Hint.OpenglForwardCompatible, forwardCompatible );
		Glfw.WindowHint( Hint.OpenglProfile, (int) oglProfile );
		Glfw.WindowHint( Hint.OpenglDebugContext, debug );
	}
	public static MonitorPtr PrimaryMonitor => Glfw.PrimaryMonitor;
}
