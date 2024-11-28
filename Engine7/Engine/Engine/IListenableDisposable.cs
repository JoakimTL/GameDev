namespace Engine;

public interface IListenableDisposable : IDisposable {
	event Action? OnDisposed;
}
