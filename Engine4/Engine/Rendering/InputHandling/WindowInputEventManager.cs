using System.Runtime.InteropServices;
using GLFW;

namespace Engine.Rendering.InputHandling;

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
		this._window = window;
		this._fileDropCallback = OnFileDrop;
		this._focusCallback = OnFocusChange;
		this._contentScaleCallback = OnContentScaleChange;
		this._maximizedCallback = OnMaximized;
		this._positionCallback = OnPositionChange;
		this._sizeCallback = OnSizeChange;
		this._framebufferCallback = OnFramebufferChange;
		this._closeCallback = OnClosing;
		this._windowRefreshCallback = OnRefresh;

		Glfw.SetDropCallback( window.Pointer, this._fileDropCallback );
		Glfw.SetWindowFocusCallback( window.Pointer, this._focusCallback );
		Glfw.SetWindowContentScaleCallback( window.Pointer, this._contentScaleCallback );
		Glfw.SetWindowMaximizeCallback( window.Pointer, this._maximizedCallback );
		Glfw.SetWindowPositionCallback( window.Pointer, this._positionCallback );
		Glfw.SetWindowSizeCallback( window.Pointer, this._sizeCallback );
		Glfw.SetFramebufferSizeCallback( window.Pointer, this._framebufferCallback );
		Glfw.SetCloseCallback( window.Pointer, this._closeCallback );
		Glfw.SetWindowRefreshCallback( window.Pointer, this._windowRefreshCallback );
	}

	private void OnFileDrop( WindowPtr winPtr, int count, IntPtr pointer ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnFileDrop )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		string[]? paths = new string[ count ];
		int offset = 0;
		for ( int i = 0; i < count; i++, offset += IntPtr.Size )
			paths[ i ] = Engine.Utilities.PointerToStringNullStop( Marshal.ReadIntPtr( pointer + offset ), System.Text.Encoding.UTF8 );

		FilesDropped?.Invoke( paths );
	}

	private void OnFocusChange( WindowPtr winPtr, bool focusing ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnFocusChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		Focused?.Invoke( focusing );
	}

	private void OnContentScaleChange( WindowPtr winPtr, float xScale, float yScale ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnContentScaleChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		ContentScaleChanged?.Invoke( xScale, yScale );
	}

	private void OnMaximized( WindowPtr winPtr, bool maximized ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnMaximized )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		Maximized?.Invoke( maximized );
	}

	private void OnPositionChange( WindowPtr winPtr, double x, double y ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnPositionChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		PositionChanged?.Invoke( x, y );
	}

	private void OnSizeChange( WindowPtr winPtr, int width, int height ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnSizeChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		Resized?.Invoke( width, height );
	}

	private void OnFramebufferChange( WindowPtr winPtr, int width, int height ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnFramebufferChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		FramebufferResized?.Invoke( width, height );
	}

	private void OnRefresh( WindowPtr winPtr ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnRefresh )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		Refreshed?.Invoke();
	}

	private void OnClosing( WindowPtr winPtr ) {
		if ( this._window.Pointer != winPtr ) {
			Log.Warning( $"{nameof( OnClosing )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Pointer}]!" );
			return;
		}

		Closing?.Invoke();
	}
}
