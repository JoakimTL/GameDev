namespace Engine.Module.Render.OpenGL.Glfw.Enums;

[Flags]
public enum ModifierKeys {
	Shift = 0b00_0001,
	Control = 0b00_0010,
	Alt = 0b00_0100,
	/// <summary>
	///     The super key ("Windows" key on Windows)
	/// </summary>
	Super = 0b00_1000,
	CapsLock = 0b01_0000,
	NumLock = 0b10_0000
}
