using Engine.GlobalServices.LoggedInput;
using Engine.Structure.Interfaces;
using GlfwBinding.Enums;

namespace Engine.GlobalServices.InputMapping;

public sealed class DelayedActiveButtonTrigger : InputTriggerBase, IUpdateable {

	private readonly DelayedButton[] _delayedButtons;

	public DelayedActiveButtonTrigger( params (MouseButton button, bool triggerState, float delay)[] buttons ) {
		_delayedButtons = buttons.Select( p => new DelayedButton( p.button, p.triggerState, p.delay ) ).ToArray();
	}

	public override void RegisterInput<T>( T input ) {
		if ( input is not TimedButtonEvent buttonEventState )
			return;
		for ( int i = 0; i < _delayedButtons.Length; i++ )
			_delayedButtons[ i ].RegisterInput( buttonEventState );
	}

	public void Update( float time, float deltaTime ) {
		if ( _delayedButtons.Any( p => p.IsTriggered( time ) ) ) {
			SetState( time, true );
			return;
		}
		SetState( time, false );
	}

	public class DelayedButton {
		private readonly MouseButton _button;
		private readonly bool _triggerState;
		private readonly float _delay;
		private bool _pressed;
		private bool _triggered;
		private float _pressedTime;

		public DelayedButton( MouseButton button, bool triggerState, float delay ) {
			_button = button;
			_triggerState = triggerState;
			_delay = delay;
		}

		public void RegisterInput( TimedButtonEvent buttonEventState ) {
			if ( buttonEventState.State.Button != _button || !( _pressed = buttonEventState.State.Pressed == _triggerState ) )
				return;

			_pressedTime = buttonEventState.Time;
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
