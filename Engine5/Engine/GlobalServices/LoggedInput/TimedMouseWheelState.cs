using Engine.Rendering.Contexts.Input.StateStructs;

namespace Engine.GlobalServices.LoggedInput;

public readonly struct TimedMouseWheelState {
    public readonly double Time;
    public readonly Point2d ScrollState;

    public TimedMouseWheelState( double time, Point2d scrollState ) {
        Time = time;
        ScrollState = scrollState;
    }
}
