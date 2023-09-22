using Engine.GlobalServices.LoggedInput;
using GlfwBinding.Enums;

namespace Engine.GlobalServices.InputMapping;

public sealed class KeyTrigger : InputTriggerBase {

	private readonly bool _triggerState;
	private readonly Keys[] _keys;

	public KeyTrigger( bool triggerState, params Keys[] keys ) {
		_triggerState = triggerState;
		_keys = keys;
	}


	public override void RegisterInput<T>( T input ) {
		if ( input is not TimedKeyEvent keyEventState )
			return;
		if ( _keys.Any( p => p == keyEventState.State.Key ) )
			SetState( keyEventState.Time, keyEventState.State.Pressed == _triggerState );
	}

}
