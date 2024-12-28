using Engine.Logging;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Glfw;
using Engine.Module.Render.Ogl.Services;
using Engine.Module.Render.Ogl.OOP;

namespace Engine.Module.Render.Input;

public sealed class UserMouseInputEventService {
	private readonly OglWindow _window;
	private readonly MouseButtonCallback _buttonCallback;
	private readonly MouseEnterCallback _enterCallback;
	private readonly MouseCallback _cursorCallback;
	private readonly MouseCallback _scrollCallback;

	/// <summary>
	/// This event fires the mouse wheel has changed position.
	/// </summary>
	public event MouseScrollHandler? WheelScrolled;
	/// <summary>
	/// This event fires whenever a mouse button is pressed down.
	/// </summary>
	public event MouseButtonHandler? ButtonPressed;
	/// <summary>
	/// This event fires whenever a mouse button is released.
	/// </summary>
	public event MouseButtonHandler? ButtonReleased;
	/// <summary>
	/// If a mouse button is held down for a period of time, warranting a repeat event.
	/// </summary>
	public event MouseButtonHandler? ButtonRepeated;
	/// <summary>
	/// Triggered when the mouse is moved.
	/// </summary>
	public event MouseMoveHandler? MouseMoved;
	/// <summary>
	/// If the mouse is entering or leaving the window, this event will trigger.
	/// </summary>
	public event MouseEnterHandler? MouseEnter;

	public UserMouseInputEventService( WindowService windowService ) {
		this._window = windowService.Window;

		this._enterCallback = OnEnter;
		this._buttonCallback = OnButton;
		this._cursorCallback = OnCursor;
		this._scrollCallback = OnScroll;

		EventUtilities.SetCursorEnterCallback( this._window.Handle, this._enterCallback );
		EventUtilities.SetMouseButtonCallback( this._window.Handle, this._buttonCallback );
		EventUtilities.SetCursorPositionCallback( this._window.Handle, this._cursorCallback );
		EventUtilities.SetScrollCallback( this._window.Handle, this._scrollCallback );
	}

	private void OnEnter( nint winPtr, bool enter ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnEnter )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		MouseEnter?.Invoke( enter );
	}

	private void OnButton( nint winPtr, MouseButton button, InputState state, ModifierKeys modifiers ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnButton )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		switch (state) {
			case InputState.Press:
				ButtonPressed?.Invoke( button, modifiers );
				break;
			case InputState.Release:
				ButtonReleased?.Invoke( button, modifiers );
				break;
			case InputState.Repeat:
				ButtonRepeated?.Invoke( button, modifiers );
				break;
		}
	}

	private void OnCursor( nint winPtr, double x, double y ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnCursor )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		MouseMoved?.Invoke( x, y );
	}

	private void OnScroll( nint winPtr, double x, double y ) {
		if (this._window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnScroll )} {nameof( winPtr )} parameter [{winPtr}] does not match [{this._window.Handle}]!" );
			return;
		}

		WheelScrolled?.Invoke( (float) x, (float) y );
	}
}
