namespace Engine.GlobalServices.LoggedInput;

public readonly struct TimedMouseLockEventState {
    public readonly double Time;
    public readonly bool LockState;

    public TimedMouseLockEventState( double time, bool lockState ) {
        Time = time;
        LockState = lockState;
    }
}
