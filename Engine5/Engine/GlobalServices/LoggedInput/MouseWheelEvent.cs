using Engine.Rendering.Contexts.Input.StateStructs;

namespace Engine.GlobalServices.LoggedInput;

public sealed class MouseWheelEvent : EventBase {
	public readonly Point2d ScrollState;

	public MouseWheelEvent( Point2d scrollState ) {
		ScrollState = scrollState;
	}
}
