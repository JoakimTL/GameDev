using Engine.GlobalServices.LoggedInput;
using GlfwBinding.Enums;

namespace Engine.GlobalServices.InputMapping;

public sealed class ButtonRequirement : IInputRequirement {

	private readonly MouseButton[] _buttons;
	private readonly bool[] _pressed;
	public bool RequirementMet { get; private set; }

	public ButtonRequirement( params MouseButton[] buttons ) {
		RequirementMet = false;
		_buttons = buttons;
		_pressed = new bool[ buttons.Length ];
	}

	public void RegisterInput<T>( T input ) {
		if ( input is TimedButtonEvent buttonEventState ) {
			for ( int i = 0; i < _buttons.Length; i++ )
				if ( buttonEventState.State.Button == _buttons[ i ] ) {
					_pressed[ i ] = buttonEventState.State.Pressed;
					break;
				}
			RequirementMet = _pressed.Any( p => p );
		}
	}
}
