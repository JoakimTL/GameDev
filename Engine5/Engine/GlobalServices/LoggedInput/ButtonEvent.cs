using Engine.Rendering.Contexts.Input.StateStructs;

namespace Engine.GlobalServices.LoggedInput;

public sealed class ButtonEvent : EventBase {
	public readonly ButtonEventState State;

	public ButtonEvent( ButtonEventState state ) {
		State = state;
	}
}
