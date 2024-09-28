namespace Engine.Modules.Rendering.Ogl.OOP;

public sealed class WindowSettings {
	//Todo: add mutability and event listening
	public required string Title { get; init; }
	/// <summary>
	/// Defines the display mode of the window.
	/// </summary>
	public required IWindowDisplayMode DisplayMode { get; init; }
	/// <summary>
	/// Defines the vertical synchronization level of the window. 0 = No VSync, 1 = Synchronized to 1x vertical refresh, 2 = Synchronized to 2x vertical refresh, etc...
	/// </summary>
	public uint VSyncLevel { get; init; } = 1;
	/// <summary>
	/// Defines the monitor to use for fullscreen modes. <see cref="nint.Zero"/> will use the primary monitor.<br/>
	/// To get available monitors you'll need to utilize Glfw.GetMonitors() and Glfw.GetMonitorName().
	/// </summary>
	public nint Monitor { get; init; } = nint.Zero;
	//public Window? Share { get; set; } = null;

}
