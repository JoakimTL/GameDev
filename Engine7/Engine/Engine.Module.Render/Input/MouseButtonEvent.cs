using Engine.Module.Render.Glfw.Enums;

namespace Engine.Module.Render.Input;

public readonly struct MouseButtonEvent( double time, MouseButton button, ModifierKeys modifiers, TactileInputType inputType ) {
	public readonly double Time = time;
	public readonly MouseButton Button = button;
	public readonly ModifierKeys Modifiers = modifiers;
	public readonly TactileInputType InputType = inputType;
}
