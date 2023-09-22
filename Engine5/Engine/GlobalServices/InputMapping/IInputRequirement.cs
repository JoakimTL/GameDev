using Engine.GlobalServices.LoggedInput;

namespace Engine.GlobalServices.InputMapping;

public interface IInputRequirement {
	bool RequirementMet { get; }
	void RegisterInput( TimedEvent @event );
}
