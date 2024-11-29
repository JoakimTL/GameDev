namespace Engine;

public interface IListenableDisposable : IDisposable {
	event DisposalHandler? OnDisposed;
}

public delegate void DisposalHandler( IListenableDisposable disposable );