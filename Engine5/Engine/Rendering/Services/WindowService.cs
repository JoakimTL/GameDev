using Engine.Rendering.Objects;
using Engine.Rendering.OGL;
using Engine.Structure.Interfaces;
using GLFW;

namespace Engine.Rendering.Services;

public sealed class WindowService : IRenderService, IUpdateable
{
    //Allows for multiple windows
    //The window service handles event polling and buffer swapping
    //What about multiple contexts? Should a window be it's own renderserviceprovider? A contextservice provider? The context contains context data, which the window(context) holds.

    private List<Window> _windows;

    public WindowService()
    {
        _windows = new List<Window>();
    }

    #region Creation
    public Window Create(WindowSettings settings)
    {
        var pointer = settings.windowMode switch
        {
            WindowSettings.Mode.WINDOW => CreateWindow(settings.windowName, settings.resolution.X, settings.resolution.Y, settings?.share),
            WindowSettings.Mode.FULLSCREEN => CreateFullscreen(settings.windowName, settings.monitor, settings?.share),
            WindowSettings.Mode.WINDOWEDFULLSCREEN => CreateWindowedFullscreen(settings.windowName, settings.monitor, settings?.share),
            _ => CreateWindow("Untitled", 800, 600, settings.share)
        };
        Window window = new(pointer);
        _windows.Add(window);
        return window;
    }

    private static WindowPtr CreateWindow(string title, int width, int height, Window? share = null)
    {
        GlfwUtilities.SetHints(true, 0);
        WindowPtr winPtr = Glfw.CreateWindow(width, height, title, MonitorPtr.None, share?.Pointer ?? WindowPtr.None);
        return winPtr;
    }

    /// <summary>
    /// Creates a new fullscreen window, and adds it to the list of windows.
    /// </summary>
    /// <param name="title">The initial title of the window.</param>
    /// <param name="m">The monitor the window will fill.</param>
    /// <returns>The window handle.</returns>
    private static WindowPtr CreateFullscreen(string title, MonitorPtr m, Window? share = null)
    {
        VideoMode vm = Glfw.GetVideoMode(m);
        GlfwUtilities.SetHints(true, 0);
        WindowPtr winPtr = Glfw.CreateWindow(vm.Width, vm.Height, title, m, share?.Pointer ?? WindowPtr.None);
        return winPtr;
    }

    /// <summary>
    /// Creates a new borderless windowed fullscreen window, and adds it to the list of windows.
    /// </summary>
    /// <param name="title">The initial title of the window.</param>
    /// <param name="m">The monitor the window will fill.</param>
    /// <returns>The window handle.</returns>
    private static WindowPtr CreateWindowedFullscreen(string title, MonitorPtr m, Window? share = null)
    {
        VideoMode vm = Glfw.GetVideoMode(m);
        GlfwUtilities.SetHints(true, 0);
        WindowPtr winPtr = Glfw.CreateWindow(vm.Width, vm.Height, title, MonitorPtr.None, share?.Pointer ?? WindowPtr.None);
        Glfw.SetWindowAttribute(winPtr, WindowAttribute.Floating, true);
        Glfw.SetWindowAttribute(winPtr, WindowAttribute.Decorated, false);
        Glfw.GetMonitorPosition(m, out int mx, out int my);
        Glfw.SetWindowPosition(winPtr, mx, my);
        return winPtr;
    }
    #endregion

    public bool Any() => _windows.Count > 0;

    public void Update(float time, float deltaTime)
    {
        _windows.RemoveAll(p => p.Closed);
        for (int i = 0; i < _windows.Count; i++)
            _windows[i].Update(time, deltaTime);
    }
}
