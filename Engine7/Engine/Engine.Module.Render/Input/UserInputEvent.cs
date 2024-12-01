using Engine.Logging;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Glfw;
using Engine.Module.Render.Ogl.Services;
using Engine.Module.Render.Ogl.OOP;
using Engine.Time;

namespace Engine.Module.Render.Input;
public unsafe struct UserInputEvent : IUserInputEvent {
	public double Time { get; }
	public EventType EventType { get; }
	public fixed byte Content[ 128 ];
}

public interface IUserInputEvent {
	double Time { get; }
	EventType EventType { get; }
}

public enum EventType : byte {
	MouseEvent,
	KeyEvent,
	CharacterEvent,
	JoystickEvent,

}

//MOUSE
public delegate void TimedMouseButtonHandler( MouseButtonEvent @event );
public delegate void TimedMouseScrollHandler( MouseWheelEvent @event );
public delegate void TimedMouseMoveHandler( MouseMoveEvent @event );
public delegate void TimedMouseEnterHandler( MouseEnterEvent @event );
//KEYBOARD
public delegate void TimedKeyboardHandler( KeyboardEvent @event );
public delegate void TimedKeyboardCharHandler( KeyboardCharacterEvent @event );

public sealed class UserInputEventService {

	public event TimedMouseButtonHandler? OnMouseButton;
	public event TimedMouseScrollHandler? OnMouseWheelScrolled;
	public event TimedMouseMoveHandler? OnMouseMoved;
	public event TimedMouseEnterHandler? OnMouseEnter;

	public event TimedKeyboardHandler? OnKey;
	public event TimedKeyboardCharHandler? OnCharacter;

	public UserInputEventService( UserMouseInputEventService userMouseInputEventService, UserKeyboardInputEventService userKeyboardInputEventService ) {
		userMouseInputEventService.ButtonPressed += MouseButtonPressed;
		userMouseInputEventService.ButtonReleased += MouseButtonReleased;
		userMouseInputEventService.ButtonRepeated += MouseButtonRepeated;
		userMouseInputEventService.WheelScrolled += MouseWheelScrolled;
		userMouseInputEventService.MouseMoved += MouseMoved;
		userMouseInputEventService.MouseEnter += MouseEnter;

		userKeyboardInputEventService.KeyPressed += KeyPressed;
		userKeyboardInputEventService.KeyReleased += KeyReleased;
		userKeyboardInputEventService.KeyRepeated += KeyRepeated;
		userKeyboardInputEventService.CharacterInput += CharacterInput;
	}


	private void MouseButtonPressed( MouseButton button, ModifierKeys modifiers ) => OnMouseButton?.Invoke( new MouseButtonEvent( Clock<double, StopwatchTickSupplier>.ReferenceClock.Time, button, modifiers, TactileInputType.Press ) );
	private void MouseButtonReleased( MouseButton button, ModifierKeys modifiers ) => OnMouseButton?.Invoke( new MouseButtonEvent( Clock<double, StopwatchTickSupplier>.ReferenceClock.Time, button, modifiers, TactileInputType.Release ) );
	private void MouseButtonRepeated( MouseButton button, ModifierKeys modifiers ) => OnMouseButton?.Invoke( new MouseButtonEvent( Clock<double, StopwatchTickSupplier>.ReferenceClock.Time, button, modifiers, TactileInputType.Repeat ) );
	private void MouseWheelScrolled( double xAxis, double yAxis ) => OnMouseWheelScrolled?.Invoke( new MouseWheelEvent( Clock<double, StopwatchTickSupplier>.ReferenceClock.Time, xAxis, yAxis ) );
	private void MouseMoved( double xAxis, double yAxis ) => OnMouseMoved?.Invoke( new MouseMoveEvent( Clock<double, StopwatchTickSupplier>.ReferenceClock.Time, xAxis, yAxis ) );
	private void MouseEnter( bool entered ) => OnMouseEnter?.Invoke( new MouseEnterEvent( Clock<double, StopwatchTickSupplier>.ReferenceClock.Time, entered ) );

	private void KeyPressed( Keys key, ModifierKeys mods, int scanCode ) => OnKey?.Invoke( new KeyboardEvent( Clock<double, StopwatchTickSupplier>.ReferenceClock.Time, key, scanCode, mods, TactileInputType.Press ) );
	private void KeyReleased( Keys key, ModifierKeys mods, int scanCode ) => OnKey?.Invoke( new KeyboardEvent( Clock<double, StopwatchTickSupplier>.ReferenceClock.Time, key, scanCode, mods, TactileInputType.Release ) );
	private void KeyRepeated( Keys key, ModifierKeys mods, int scanCode ) => OnKey?.Invoke( new KeyboardEvent( Clock<double, StopwatchTickSupplier>.ReferenceClock.Time, key, scanCode, mods, TactileInputType.Repeat ) );
	private void CharacterInput( uint charCode, ModifierKeys mods ) => OnCharacter?.Invoke( new KeyboardCharacterEvent( Clock<double, StopwatchTickSupplier>.ReferenceClock.Time, charCode, mods ) );

}

public sealed class EventChain<T>( uint eventCount ) where T : unmanaged {
	private readonly T[] _items = new T[ eventCount ];
	private int _currentIndex = 0;
	public IReadOnlyList<T> Chain => _items[ ^_currentIndex.._currentIndex ];

	public void Add( T item ) {
		_items[ _currentIndex ] = item;
		_currentIndex++;
		if (_currentIndex >= _items.Length)
			_currentIndex = 0;
	}
}

public enum TactileInputType : byte {
	Press,
	Release,
	Repeat,
}
public readonly struct KeyboardCharacterEvent( double time, uint keyCode, ModifierKeys modifierKeys ) {
	public readonly double Time = time;
	public readonly uint KeyCode = keyCode;
	public readonly ModifierKeys ModifierKeys = modifierKeys;
	public readonly char Character => (char) KeyCode;
}

public readonly struct KeyboardEvent( double time, Keys key, int scanCode, ModifierKeys modifiers, TactileInputType inputType ) {
	public readonly double Time = time;
	public readonly Keys Key = key;
	public readonly int ScanCode = scanCode;
	public readonly ModifierKeys Modifiers = modifiers;
	public readonly TactileInputType InputType = inputType;
}

public readonly struct MouseButtonEvent( double time, MouseButton button, ModifierKeys modifiers, TactileInputType inputType ) {
	public readonly double Time = time;
	public readonly MouseButton Button = button;
	public readonly ModifierKeys Modifiers = modifiers;
	public readonly TactileInputType InputType = inputType;
}

public readonly struct MouseEnterEvent( double time, bool enterState ) {
	public readonly double Time = time;
	public readonly bool State = enterState;
}

public readonly struct MouseWheelEvent( double time, double xAxis, double yAxis ) {
	public readonly double Time = time;
	public readonly Vector2<double> Movement = new( xAxis, yAxis );
}

public readonly struct MouseMoveEvent( double time, double xAxis, double yAxis ) {
	public readonly double Time = time;
	public readonly Vector2<double> Movement = new( xAxis, yAxis );
}



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
		_window = windowService.Window;

		_enterCallback = OnEnter;
		_buttonCallback = OnButton;
		_cursorCallback = OnCursor;
		_scrollCallback = OnScroll;

		EventUtilities.SetCursorEnterCallback( _window.Handle, _enterCallback );
		EventUtilities.SetMouseButtonCallback( _window.Handle, _buttonCallback );
		EventUtilities.SetCursorPositionCallback( _window.Handle, _cursorCallback );
		EventUtilities.SetScrollCallback( _window.Handle, _scrollCallback );
	}

	private void OnEnter( nint winPtr, bool enter ) {
		if (_window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnEnter )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Handle}]!" );
			return;
		}

		MouseEnter?.Invoke( enter );
	}

	private void OnButton( nint winPtr, MouseButton button, InputState state, ModifierKeys modifiers ) {
		if (_window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnButton )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Handle}]!" );
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
		if (_window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnCursor )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Handle}]!" );
			return;
		}

		MouseMoved?.Invoke( x, y );
	}

	private void OnScroll( nint winPtr, double x, double y ) {
		if (_window.Handle != winPtr) {
			this.LogWarning( $"{nameof( OnScroll )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Handle}]!" );
			return;
		}

		WheelScrolled?.Invoke( (float) x, (float) y );
	}
}

public sealed class UserKeyboardInputEventService {
	private readonly OglWindow _window;
	private readonly KeyCallback _keyCallback;
	private readonly CharModsCallback _characterCallback;

	/// <summary>
	/// Occurs when the window receives character input.<br/>
	/// This is usually used for writing text, as opposed to <see cref="KeyPressed"/> and <see cref="KeyReleased"/> being used for gameplay.
	/// </summary>
	public event KeyboardCharHandler? CharacterInput;
	/// <summary>
	/// Occurs when a key is pressed.<br/>
	/// This is usually used for gameplay, as opposed to <see cref="CharacterInput"/>, which is used for writing.
	/// </summary>
	public event KeyboardHandler? KeyPressed;
	/// <summary>
	/// Occurs when a key is released.<br/>
	/// This is usually used for gameplay, as opposed to <see cref="CharacterInput"/>, which is used for writing.
	/// </summary>
	public event KeyboardHandler? KeyReleased;
	/// <summary>
	/// Occurs when a key is held down for a period of time, warranting a repeat event.
	/// </summary>
	public event KeyboardHandler? KeyRepeated;

	public UserKeyboardInputEventService( WindowService windowService ) {
		_window = windowService.Window;

		_keyCallback = OnKey;
		_characterCallback = OnCharacter;

		EventUtilities.SetKeyCallback( _window.Handle, _keyCallback );
		EventUtilities.SetCharModsCallback( _window.Handle, _characterCallback );
	}

	private void OnKey( nint winPtr, Keys key, int scanCode, InputState state, ModifierKeys mods ) {
		if (key == Keys.Unknown)
			return;

		if (_window.Handle != winPtr) {
			Log.Warning( $"{nameof( OnKey )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Handle}]!" );
			return;
		}

		switch (state) {
			case InputState.Press:
				KeyPressed?.Invoke( key, mods, scanCode );
				break;
			case InputState.Release:
				KeyReleased?.Invoke( key, mods, scanCode );
				break;
			case InputState.Repeat:
				KeyRepeated?.Invoke( key, mods, scanCode );
				break;
		}
	}

	private void OnCharacter( nint winPtr, uint codePoint, ModifierKeys mods ) {
		if (_window.Handle != winPtr) {
			Log.Warning( $"{nameof( OnCharacter )} {nameof( winPtr )} parameter [{winPtr}] does not match [{_window.Handle}]!" );
			return;
		}

		CharacterInput?.Invoke( codePoint, mods );
	}
}