using Engine.Module.Render.Glfw.Enums;

namespace Engine.Module.Render.Input;

public readonly struct KeyboardEvent( double time, Keys key, int scanCode, ModifierKeys modifiers, TactileInputType inputType ) {
	public readonly double Time = time;
	public readonly Keys Key = key;
	public readonly int ScanCode = scanCode;
	public readonly ModifierKeys Modifiers = modifiers;
	public readonly TactileInputType InputType = inputType;
}
