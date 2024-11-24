using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Glfw;

namespace Engine.Module.Render.Ogl.Utilities;

public static class GlfwUtilities {

	public static void InitHint( int hint, bool value ) => GLFW.InitHint( hint, value );
	public static void Init() => GLFW.Init();
	public static void Terminate() => GLFW.Terminate();
	public static void PollEvents() => GLFW.PollEvents();
	public static void SetHints( bool debug, int samples, int majorVersion = 4, int minorVersion = 6, Constants forwardCompatible = Constants.True, Profile oglProfile = Profile.Core ) {
		WindowUtilities.WindowHint( Hint.Samples, samples );
		WindowUtilities.WindowHint( Hint.ContextVersionMajor, majorVersion );
		WindowUtilities.WindowHint( Hint.ContextVersionMinor, minorVersion );
		WindowUtilities.WindowHint( Hint.OpenglForwardCompatible, (int) forwardCompatible );
		WindowUtilities.WindowHint( Hint.OpenglProfile, (int) oglProfile );
		WindowUtilities.WindowHint( Hint.OpenglDebugContext, debug ? 1 : 0 );
	}
	public static nint GetPrimaryMonitor() => GLFW.GetPrimaryMonitor();
}