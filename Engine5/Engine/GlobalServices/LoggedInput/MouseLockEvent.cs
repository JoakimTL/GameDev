namespace Engine.GlobalServices.LoggedInput;

public sealed class MouseLockEvent : EventBase {
    public readonly bool LockState;

    public MouseLockEvent( bool lockState ) {
        LockState = lockState;
    }
}
