using Engine.Rendering.Objects;
using GLFW;

namespace Engine.Rendering.Input;
public class ClientInput : Identifiable, IEventListener {

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
	/// If the cursor is visible, this event will trigger when the mouse is moved.<br/>
	/// This is equivalent to <see cref="IMouseChangeEventListener.OnMouseCursorMove(Window, MouseState)"/>!<br/> 
	/// Use <see cref="MovedHidden"/> for hidden mouse movements.
	/// </summary>
	public event MouseMoveHandler? MovedVisible;
	/// <summary>
	/// If the cursor is hidden, this event will trigger when the mouse is moved.<br/>
	/// This is equivalent to <see cref="IMouseChangeEventListener.OnMouseLockedMove(Window, MouseState)(Window, MouseState)"/>!<br/> 
	/// Use <see cref="MovedVisible"/> for visible mouse movements.
	/// </summary>
	public event MouseMoveHandler? MovedHidden;

	public void OnButtonPressed( MouseButton btn, ModifierKeys modifier, MouseState state ) => ButtonPressed?.Invoke( btn, modifier, state );
	public void OnButtonReleased( MouseButton btn, ModifierKeys modifier, MouseState state ) => ButtonReleased?.Invoke( btn, modifier, state );
	public void OnButtonRepeat( MouseButton btn, ModifierKeys modifier, MouseState state ) => ButtonRepeated?.Invoke( btn, modifier, state );
	public void OnCharacterWritten( uint charCode, string character, ModifierKeys mods ) => CharacterInput?.Invoke( charCode, character, mods );
	public void OnKeyPressed( Keys key, ModifierKeys mods, int scanCode ) => KeyPressed?.Invoke( key, mods, scanCode );
	public void OnKeyReleased( Keys key, ModifierKeys mods, int scanCode ) => KeyReleased?.Invoke( key, mods, scanCode );
	public void OnKeyRepeated( Keys key, ModifierKeys mods, int scanCode ) => KeyRepeated?.Invoke( key, mods, scanCode );
	public void OnMouseCursorMove( MouseState state ) => MovedVisible?.Invoke( state );
	public void OnMouseLockedMove( MouseState state ) => MovedHidden?.Invoke( state );
	public void OnMouseScroll( double xAxis, double yAxis, MouseState state ) => WheelScrolled?.Invoke( xAxis, yAxis, state );
}
