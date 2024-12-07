using Engine.Logging;
using Engine.Module.Render.Glfw;
using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.Services;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.Module.Render.Input;

public sealed class WindowEventService {
	private readonly OglWindow _window;
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

	public WindowEventService( WindowService windowService ) {
		this._window = windowService.Window;
		this._fileDropCallback = OnFileDrop;
		this._focusCallback = OnFocusChange;
		this._contentScaleCallback = OnContentScaleChange;
		this._maximizedCallback = OnMaximized;
		this._positionCallback = OnPositionChange;
		this._sizeCallback = OnSizeChange;
		this._framebufferCallback = OnFramebufferChange;
		this._closeCallback = OnClosing;
		this._windowRefreshCallback = OnRefresh;

		EventUtilities.SetDropCallback( this._window.Handle, this._fileDropCallback );
		EventUtilities.SetWindowFocusCallback( this._window.Handle, this._focusCallback );
		EventUtilities.SetWindowContentScaleCallback( this._window.Handle, this._contentScaleCallback );
		EventUtilities.SetWindowMaximizeCallback( this._window.Handle, this._maximizedCallback );
		EventUtilities.SetWindowPositionCallback( this._window.Handle, this._positionCallback );
		EventUtilities.SetWindowSizeCallback( this._window.Handle, this._sizeCallback );
		EventUtilities.SetFramebufferSizeCallback( this._window.Handle, this._framebufferCallback );
		EventUtilities.SetCloseCallback( this._window.Handle, this._closeCallback );
		EventUtilities.SetWindowRefreshCallback( this._window.Handle, this._windowRefreshCallback );
	}

	private void OnFileDrop( nint winPtr, int count, nint pointer ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnFileDrop )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		string[]? paths = new string[ count ];
		int offset = 0;
		for (int i = 0; i < count; i++, offset += nint.Size)
			paths[ i ] = Marshal.ReadIntPtr( pointer + offset ).ToStringNullStop( Encoding.UTF8 );

		FilesDropped?.Invoke( paths );
	}

	private void OnFocusChange( nint winPtr, bool focusing ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnFocusChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		Focused?.Invoke( focusing );
	}

	private void OnContentScaleChange( nint winPtr, float xScale, float yScale ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnContentScaleChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		ContentScaleChanged?.Invoke( xScale, yScale );
	}

	private void OnMaximized( nint winPtr, bool maximized ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnMaximized )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		Maximized?.Invoke( maximized );
	}

	private void OnPositionChange( nint winPtr, double x, double y ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnPositionChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		PositionChanged?.Invoke( x, y );
	}

	private void OnSizeChange( nint winPtr, int width, int height ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnSizeChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		Resized?.Invoke( width, height );
	}

	private void OnFramebufferChange( nint winPtr, int width, int height ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnFramebufferChange )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		FramebufferResized?.Invoke( width, height );
	}

	private void OnRefresh( nint winPtr ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnRefresh )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		Refreshed?.Invoke();
	}

	private void OnClosing( nint winPtr ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnClosing )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		Closing?.Invoke();
	}
}
