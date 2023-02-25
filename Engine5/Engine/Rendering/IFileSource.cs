namespace Engine.Rendering;

public interface IFileSource
{
    event Action? FileChanged;
    string GetData();
}
