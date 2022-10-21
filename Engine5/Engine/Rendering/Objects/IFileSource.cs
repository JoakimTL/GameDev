namespace Engine.Rendering.Objects;

public interface IFileSource {
	event Action? FileChanged;
	string GetData();
}
