namespace Engine.Modules.Rendering.Ogl.OOP;

public sealed class WindowSettings {
	public required string Title { get; init; }
	public required int Width { get; init; }
	public required int Height { get; init; }
	public required WindowFullscreenMode FullscreenMode { get; init; }
	public required uint VSyncLevel { get; init; }
	public nint Monitor { get; init; } = nint.Zero;
	//public Window? Share { get; set; } = null;

}
