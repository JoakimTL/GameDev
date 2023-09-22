namespace Engine.GlobalServices.LoggedInput;

public sealed class MouseEnterEvent : EventBase {
    public readonly bool EnterState;

    public MouseEnterEvent( bool enterState ) {
        EnterState = enterState;
    }
}