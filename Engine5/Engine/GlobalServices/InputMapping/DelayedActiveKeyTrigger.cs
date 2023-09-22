using Engine.GlobalServices.LoggedInput;
using Engine.Structure.Interfaces;
using GlfwBinding.Enums;

namespace Engine.GlobalServices.InputMapping;

public sealed class DelayedActiveKeyTrigger : InputTriggerBase, IUpdateable {

	private readonly DelayedKey[] _delayedKeys;

	public DelayedActiveKeyTrigger( params (Keys key, bool triggerState, float delay)[] keys ) {
		_delayedKeys = keys.Select( p => new DelayedKey( p.key, p.triggerState, p.delay ) ).ToArray();
	}

	public override void RegisterInput<T>( T input ) {
		if ( input is not TimedKeyEvent keyEventState )
			return;
		for ( int i = 0; i < _delayedKeys.Length; i++ )
			_delayedKeys[ i ].RegisterInput( keyEventState );
	}

	public void Update( float time, float deltaTime ) {
		if ( _delayedKeys.Any( p => p.IsTriggered( time ) ) ) {
			SetState( time, true );
			return;
		}
		SetState( time, false );
	}

	public class DelayedKey {
		private readonly Keys _key;
		private readonly bool _triggerState;
		private readonly float _delay;
		private bool _pressed;
		private bool _triggered;
		private float _pressedTime;

		public DelayedKey( Keys key, bool triggerState, float delay ) {
			_key = key;
			_triggerState = triggerState;
			_delay = delay;
		}

		public void RegisterInput( TimedKeyEvent keyEventState ) {
			if ( keyEventState.State.Key != _key || !( _pressed = keyEventState.State.Pressed == _triggerState ) )
				return;

			_pressedTime = keyEventState.Time;
			_triggered = false;
		}

		public bool IsTriggered( float time ) {
			if ( _triggered )
				return false;
			if ( !_pressed )
				return false;
			if ( time - _pressedTime < _delay )
				return false;

			_triggered = true;
			return true;
		}
	}
}
