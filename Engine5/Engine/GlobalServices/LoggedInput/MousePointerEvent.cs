using Engine.Rendering.Contexts.Input.StateStructs;

namespace Engine.GlobalServices.LoggedInput;

public sealed class MousePointerEvent : EventBase {
    public readonly MousePointerState State;
    public readonly bool LockState;

    public MousePointerEvent( MousePointerState state, bool lockState ) {
        State = state;
        LockState = lockState;
    }
}
