using Engine;
using Engine.GameLogic.ECPS;
using Engine.GlobalServices.InputMapping;
using Engine.GlobalServices.LoggedInput;
using Engine.Structure.Interfaces;

namespace StandardPackage.ECPS.Components;

public sealed class UnmappedInputStateComponent : ComponentBase {

    private readonly List<SubtickEventBase> _subtickEvents;

    public UnmappedInputStateComponent() {
		_subtickEvents = new();
	}

    internal void SetStates( IEnumerable<SubtickEventBase> timedEvents ) {
		_subtickEvents.ClearThenAddRange( timedEvents );
    }

	public IReadOnlyList<TimedEvent> TimedEvents => _subtickEvents;


}

//How will this solve problems like mouse movement? This solves problems like input mapping, but for other more dynamic input we need a different solution.
public sealed class InputCommandComponent : ComponentBase, ICustomizedSerializable {
	public static Guid SerializationIdentity => new( "12e69243-9f95-42c3-88d1-2851a1be844f" );

	public bool ShouldSerialize => true;

	private readonly List<SubtickCommandState> _states;

	public bool DeserializeData( byte[] data ) {
		throw new NotImplementedException();
	}

	public byte[] SerializeData() {
		throw new NotImplementedException();
	}

	internal void SetStates( IReadOnlyList<SubtickCommandState> states ) {
		_states
	}

	public class SubtickCommandState {

		public float Subtick { get; }
		public bool State { get; }
		public string Command { get; }

		public SubtickCommandState( float subtick, bool state, string command ) {
			Subtick = subtick;
			State = state;
			Command = command;
		}

	}
}