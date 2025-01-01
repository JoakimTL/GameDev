using Engine.Module.Render.Glfw.Enums;

namespace Engine.Standard.Entities.Components;

public sealed class UiElementClicked( double time, MouseButton button ) {
	public double Time { get; } = time;
	public MouseButton Button { get; } = button;
}

