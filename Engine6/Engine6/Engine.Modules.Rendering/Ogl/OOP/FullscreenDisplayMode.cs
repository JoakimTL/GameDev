namespace Engine.Modules.Rendering.Ogl.OOP;

public readonly struct FullscreenDisplayMode() : IWindowDisplayMode {
	public Vector2<uint> Size { get; } = 0;
	public WindowDisplayMode Mode { get; } = WindowDisplayMode.Fullscreen;
}
