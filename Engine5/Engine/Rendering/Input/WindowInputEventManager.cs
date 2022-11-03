using System.Runtime.InteropServices;
using Engine.Rendering.Objects;
using GLFW;

namespace Engine.Rendering.Input;

public class WindowInputEventManager {

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

	internal WindowInputEventManager( Window window ) {
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

		Glfw.SetDropCallback( window.Pointer, _fileDropCallback );
		Glfw.SetWindowFocusCallback( window.Pointer, _focusCallback );
		Glfw.SetWindowContentScaleCallback( window.Pointer, _contentScaleCallback );
		Glfw.SetWindowMaximizeCallback( window.Pointer, _maximizedCallback );
		Glfw.SetWindowPositionCallback( window.Pointer, _positionCallback );
		Glfw.SetWindowSizeCallback( window.Pointer, _sizeCallback );
		Glfw.SetFramebufferSizeCallback( window.Pointer, _framebufferCallback );
		Glfw.SetCloseCallback( window.Pointer, _closeCallback );
		Glfw.SetWindowRefreshCallback( window.Pointer, _windowRefreshCallback );
	}

	private void OnFileDrop( WindowPtr winPtr, int count, IntPtr pointer ) {
#if DEBUG
		if ( _window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnFileDrop )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Pointer}]!" );
			return;
		}
#endif

		string[]? paths = new string[ count ];
		int offset = 0;
		for ( int i = 0; i < count; i++, offset += IntPtr.Size )
			paths[ i ] = Utilities.UtilityMethods.PointerToStringNullStop( Marshal.ReadIntPtr( pointer + offset ), System.Text.Encoding.UTF8 );

		FilesDropped?.Invoke( paths );
	}

	private void OnFocusChange( WindowPtr winPtr, bool focusing ) {
#if DEBUG
		if ( _window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnFocusChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Pointer}]!" );
			return;
		}
#endif

		Focused?.Invoke( focusing );
	}

	private void OnContentScaleChange( WindowPtr winPtr, float xScale, float yScale ) {
#if DEBUG
		if ( _window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnContentScaleChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Pointer}]!" );
			return;
		}
#endif

		ContentScaleChanged?.Invoke( xScale, yScale );
	}

	private void OnMaximized( WindowPtr winPtr, bool maximized ) {
#if DEBUG
		if ( _window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnMaximized )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Pointer}]!" );
			return;
		}
#endif

		Maximized?.Invoke( maximized );
	}

	private void OnPositionChange( WindowPtr winPtr, double x, double y ) {
#if DEBUG
		if ( _window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnPositionChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Pointer}]!" );
			return;
		}
#endif

		PositionChanged?.Invoke( x, y );
	}

	private void OnSizeChange( WindowPtr winPtr, int width, int height ) {
#if DEBUG
		if ( _window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnSizeChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Pointer}]!" );
			return;
		}
#endif

		Resized?.Invoke( width, height );
	}

	private void OnFramebufferChange( WindowPtr winPtr, int width, int height ) {
#if DEBUG
		if ( _window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnFramebufferChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Pointer}]!" );
			return;
		}
#endif

		FramebufferResized?.Invoke( width, height );
	}

	private void OnRefresh( WindowPtr winPtr ) {
#if DEBUG
		if ( _window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnRefresh )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Pointer}]!" );
			return;
		}
#endif

		Refreshed?.Invoke();
	}

	private void OnClosing( WindowPtr winPtr ) {
#if DEBUG
		if ( _window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnClosing )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Pointer}]!" );
			return;
		}
#endif

		Closing?.Invoke();
	}
}
