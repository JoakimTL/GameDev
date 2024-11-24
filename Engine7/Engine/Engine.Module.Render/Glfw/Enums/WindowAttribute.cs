namespace Engine.Module.Render.Glfw.Enums;

public enum WindowAttribute {
	Focused = 0x00020001,
	/// <summary>Indicates whether the full screen window will automatically iconify and restore the previous video mode on input focus loss.<para>This hint is ignored for windowed mode windows.</para></summary>
	AutoIconify = 0x00020002,
	Maximized = 0x00020008,
	Visible = 0x00020004,
	Resizable = 0x00020003,
	Decorated = 0x00020005,
	Floating = 0x00020007,
	MouseHover = 0x0002000B
}
