using Engine.Rendering.Contexts.Input.StateStructs;

namespace Engine.GlobalServices.LoggedInput;

public readonly struct TimedMousePointerState {
    public readonly double Time;
    public readonly MousePointerState State;
    public readonly bool LockState;

    public TimedMousePointerState( double time, MousePointerState state, bool lockState ) {
        Time = time;
        State = state;
        LockState = lockState;
    }
}
