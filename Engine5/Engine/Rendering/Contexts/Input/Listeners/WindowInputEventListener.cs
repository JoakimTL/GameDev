using System.Runtime.InteropServices;
using Engine.Rendering.OGL;
using GlfwBinding;

namespace Engine.Rendering.Contexts.Input.Listeners;

public class WindowInputEventListener
{

    private readonly Window _window;
    private readonly FileDropCallback _fileDropCallback;
    private readonly FocusCallback _focusCallback;
    private readonly WindowContentsScaleCallback _contentScaleCallback;
    private readonly WindowMaximizedCallback _maximizedCallback;
    private readonly PositionCallback _positionCallback;
    private readonly SizeCallback _sizeCallback;
    private readonly SizeCallback _framebufferCallback;
    private readonly WindowCallback _closeCallback;
    private readonly WindowCallback _windowRefreshCallback;

    /// <summary>
    /// Whenever files are dropped on the window.
    /// </summary>
    public event FileDropHandler? FilesDropped;

    /// <summary>
    /// Whenever the window is resized.
    /// </summary>
    public event WindowSizeHandler? Resized;

    /// <summary>
    /// Whenever the framebuffer is resized.
    /// </summary>
    public event WindowSizeHandler? FramebufferResized;

    /// <summary>
    /// Whenever the content scale of the window is changed.
    /// </summary>
    public event WindowContentScaleHandler? ContentScaleChanged;

    /// <summary>
    /// Fires when the position of the window is changed.
    /// </summary>
    public event WindowPositionHandler? PositionChanged;

    /// <summary>
    /// Fires when the window is maximized and unmaximized.
    /// </summary>
    public event WindowBooleanValueHandler? Maximized;

    /// <summary>
    /// Fires when the focus of the window changed. True if this window is focused, false if another window is focused. Can be used to lower framerates when the player isn't paying attention to the window.
    /// </summary>
    public event WindowBooleanValueHandler? Focused;

    /// <summary>
    /// Fires when a state of the window is changed and the window is refreshed.
    /// </summary>
    public event WindowEventHandler? Refreshed;

    /// <summary>
    /// Fires when the window is told to close.
    /// </summary>
    public event WindowEventHandler? Closing;

    internal WindowInputEventListener(Window window)
    {
        _window = window;
        _fileDropCallback = OnFileDrop;
        _focusCallback = OnFocusChange;
        _contentScaleCallback = OnContentScaleChange;
        _maximizedCallback = OnMaximized;
        _positionCallback = OnPositionChange;
        _sizeCallback = OnSizeChange;
        _framebufferCallback = OnFramebufferChange;
        _closeCallback = OnClosing;
        _windowRefreshCallback = OnRefresh;

        EventUtilities.SetDropCallback(window.Pointer, _fileDropCallback);
        EventUtilities.SetWindowFocusCallback(window.Pointer, _focusCallback);
        EventUtilities.SetWindowContentScaleCallback(window.Pointer, _contentScaleCallback);
        EventUtilities.SetWindowMaximizeCallback(window.Pointer, _maximizedCallback);
        EventUtilities.SetWindowPositionCallback(window.Pointer, _positionCallback);
        EventUtilities.SetWindowSizeCallback(window.Pointer, _sizeCallback);
        EventUtilities.SetFramebufferSizeCallback(window.Pointer, _framebufferCallback);
        EventUtilities.SetCloseCallback(window.Pointer, _closeCallback);
        EventUtilities.SetWindowRefreshCallback(window.Pointer, _windowRefreshCallback);
    }

    private void OnFileDrop(nint winPtr, int count, nint pointer)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnFileDrop)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        string[]? paths = new string[count];
        int offset = 0;
        for (int i = 0; i < count; i++, offset += nint.Size)
            paths[i] = Utilities.UtilityMethods.PointerToStringNullStop(Marshal.ReadIntPtr(pointer + offset), System.Text.Encoding.UTF8);

        FilesDropped?.Invoke(paths);
    }

    private void OnFocusChange(nint winPtr, bool focusing)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnFocusChange)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        Focused?.Invoke(focusing);
    }

    private void OnContentScaleChange(nint winPtr, float xScale, float yScale)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnContentScaleChange)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        ContentScaleChanged?.Invoke(xScale, yScale);
    }

    private void OnMaximized(nint winPtr, bool maximized)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnMaximized)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        Maximized?.Invoke(maximized);
    }

    private void OnPositionChange(nint winPtr, double x, double y)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnPositionChange)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        PositionChanged?.Invoke(x, y);
    }

    private void OnSizeChange(nint winPtr, int width, int height)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnSizeChange)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        Resized?.Invoke(width, height);
    }

    private void OnFramebufferChange(nint winPtr, int width, int height)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnFramebufferChange)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        FramebufferResized?.Invoke(width, height);
    }

    private void OnRefresh(nint winPtr)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnRefresh)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        Refreshed?.Invoke();
    }

    private void OnClosing(nint winPtr)
    {
#if DEBUG
        if (_window.Pointer != winPtr)
        {
            Log.Warning($"{nameof(OnClosing)} {nameof(winPtr)} parameter [{winPtr}] does not match [{_window.Pointer}]!");
            return;
        }
#endif

        Closing?.Invoke();
    }
}
