using Engine.GlobalServices.LoggedInput;

namespace Engine.GlobalServices.InputMapping;

public interface IInputTrigger {
	public bool CurrentState { get; }
	event TriggerActivated? Triggered;
	/// <returns>True if input is triggered</returns>
	void RegisterInput( TimedEvent @event );
}
