using Engine.Module.Render.Glfw.Enums;
using Engine.Time;

namespace Engine.Module.Render.Input;

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
