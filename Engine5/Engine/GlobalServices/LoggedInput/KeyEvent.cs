using Engine.Rendering.Contexts.Input.StateStructs;

namespace Engine.GlobalServices.LoggedInput;

public sealed class KeyEvent : EventBase {
    public readonly KeyEventState State;

    public KeyEvent( KeyEventState state ) {
        State = state;
    }
}
