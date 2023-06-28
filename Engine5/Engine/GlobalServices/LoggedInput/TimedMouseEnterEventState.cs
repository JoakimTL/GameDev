namespace Engine.GlobalServices.LoggedInput;

public readonly struct TimedMouseEnterEventState {
    public readonly double Time;
    public readonly bool EnterState;

    public TimedMouseEnterEventState( double time, bool enterState ) {
        Time = time;
        EnterState = enterState;
    }
}