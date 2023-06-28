using Engine.Rendering.Contexts.Input.StateStructs;

namespace Engine.GlobalServices.LoggedInput;

public readonly struct TimedKeyEventState {
    public readonly double Time;
    public readonly KeyEventState State;

    public TimedKeyEventState( double time, KeyEventState state ) {
        Time = time;
        State = state;
    }
}
