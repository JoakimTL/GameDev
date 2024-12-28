using Engine.Module.Render.Glfw.Enums;

namespace Engine.Module.Render.Input;

public readonly struct KeyboardCharacterEvent( double time, uint keyCode, ModifierKeys modifierKeys ) {
	public readonly double Time = time;
	public readonly uint KeyCode = keyCode;
	public readonly ModifierKeys Modifiers = modifierKeys;
	public readonly char Character => (char) this.KeyCode;
}
