namespace Engine.Module.Render.Domain;

public sealed class WindowedDisplayMode( Vector2<uint> size ) : IWindowDisplayMode {
	public Vector2<uint> Size { get; } = size;
	public WindowDisplayMode Mode { get; } = WindowDisplayMode.Windowed;
}
