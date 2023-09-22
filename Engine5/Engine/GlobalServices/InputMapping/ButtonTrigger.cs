using Engine.GlobalServices.LoggedInput;
using GlfwBinding.Enums;

namespace Engine.GlobalServices.InputMapping;

public sealed class ButtonTrigger : InputTriggerBase {

	
	private readonly bool _triggerState;
	private readonly MouseButton[] _buttons;

	public ButtonTrigger( bool triggerState, params MouseButton[] buttons ) {
		_triggerState = triggerState;
		_buttons = buttons;
	}

	public override void RegisterInput<T>( T input ) {
		if ( input is not TimedButtonEvent buttonEventState )
			return;
		if ( _buttons.Any( p => p == buttonEventState.State.Button ) )
			SetState( buttonEventState.Time, buttonEventState.State.Pressed == _triggerState );
	}
}
