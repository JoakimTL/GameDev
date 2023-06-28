using Engine.Rendering.Contexts.Input.StateStructs;

namespace Engine.GlobalServices.LoggedInput;

public readonly struct TimedButtonEventState {
    public readonly double Time;
    public readonly MouseButtonEventState State;

    public TimedButtonEventState( double time, MouseButtonEventState state ) {
        Time = time;
        State = state;
    }
}
