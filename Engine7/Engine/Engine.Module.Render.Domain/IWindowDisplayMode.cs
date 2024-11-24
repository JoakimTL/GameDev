namespace Engine.Module.Render.Domain;

public interface IWindowDisplayMode {
	Vector2<uint> Size { get; }
	WindowDisplayMode Mode { get; }
}
