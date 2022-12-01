namespace GlfwBinding.Enums;

[Flags]
public enum ModifierKeys {
	Shift = 0x0001,
	Control = 0x0002,
	Alt = 0x0004,
	/// <summary>
	///     The super key ("Windows" key on Windows)
	/// </summary>
	Super = 0x0008,
	CapsLock = 0x0010,
	NumLock = 0x0020
}
