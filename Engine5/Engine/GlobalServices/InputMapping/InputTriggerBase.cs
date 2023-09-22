using Engine.GlobalServices.LoggedInput;

namespace Engine.GlobalServices.InputMapping;

public abstract class InputTriggerBase : IInputTrigger {

	public bool CurrentState { get; private set; }
	public event TriggerActivated? Triggered;

	protected void SetState( float time, bool state ) {
		if ( state == CurrentState )
			return;
		CurrentState = state;
		Triggered?.Invoke( CurrentState, time );
	}

	public abstract void RegisterInput( TimedEvent @event );

}