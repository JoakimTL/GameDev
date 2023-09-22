using Engine.GlobalServices.LoggedInput;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices.InputMapping;

public delegate void TriggerActivated( bool state, float time );
public delegate void InputTriggered( string inputCode, bool state, float time );

public sealed class Input : Identifiable, IUpdateable {

	public event InputTriggered? Triggered;
	public string InputCode { get; }
	private bool _currentState;

	private readonly List<IInputTrigger> _triggers;
	private readonly List<IUpdateable> _updateableTriggers;
	private readonly List<IInputRequirement> _requirements;

	public Input( string inputCode, IEnumerable<IInputTrigger> triggers, IEnumerable<IInputRequirement> requirements ) {
		InputCode = inputCode;
		_triggers = new( triggers );
		_updateableTriggers = _triggers.OfType<IUpdateable>().ToList();
		_requirements = new( requirements );
		foreach ( var trigger in _triggers )
			trigger.Triggered += OnTriggerActivated;
	}

	private void SetState(bool state, float time) {
		if ( _currentState == state )
			return;
		_currentState = state;
		Triggered?.Invoke( InputCode, state, time );
	}

	private void OnTriggerActivated( bool state, float time ) {
		if (_triggers.Any(p => p.CurrentState))
			return;
		SetState( state, time );
	}

	public void RegisterInput( TimedEvent @event ) {
		foreach ( var requirement in _requirements )
			requirement.RegisterInput( @event );
		bool requirementsMet = _requirements.All( r => r.RequirementMet );
		if ( !requirementsMet ) {
			SetState( false, @event.Time );
			return;
		}
		foreach ( var trigger in _triggers )
			trigger.RegisterInput( @event );
	}

	public void Update( float time, float deltaTime ) {
		foreach ( var trigger in _updateableTriggers )
			trigger.Update( time, deltaTime );
	}
}
