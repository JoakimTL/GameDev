using GlfwBinding;
using GlfwBinding.Enums;

namespace Engine.Rendering.OGL;

public static class GlfwUtilities
{

    public static void InitHint(int hint, bool value) => Glfw.InitHint(hint, value);
    public static void Init() => Glfw.Init();
    public static void Terminate() => Glfw.Terminate();
    public static void PollEvents() => Glfw.PollEvents();
    public static void SetHints(bool debug, int samples, int majorVersion = 4, int minorVersion = 6, Constants forwardCompatible = Constants.True, Profile oglProfile = Profile.Core)
    {
        WindowUtilities.WindowHint(Hint.Samples, samples);
        WindowUtilities.WindowHint(Hint.ContextVersionMajor, majorVersion);
        WindowUtilities.WindowHint(Hint.ContextVersionMinor, minorVersion);
        WindowUtilities.WindowHint(Hint.OpenglForwardCompatible, (int)forwardCompatible);
        WindowUtilities.WindowHint(Hint.OpenglProfile, (int)oglProfile);
        WindowUtilities.WindowHint(Hint.OpenglDebugContext, debug ? 1 : 0);
    }
    public static nint GetPrimaryMonitor() => Glfw.GetPrimaryMonitor();
}
