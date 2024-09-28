namespace Engine.Modules.Rendering.Ogl.OOP;

public readonly struct WindowedDisplayMode(Vector2<uint> size) : IWindowDisplayMode {
	public Vector2<uint> Size { get; } = size;
	public WindowDisplayMode Mode { get; } = WindowDisplayMode.Windowed;
}