namespace Engine;

public interface IListenableDisposable : IDisposable {
	bool Disposed { get; }
	event DisposalHandler? OnDisposed;
}

public delegate void DisposalHandler( IListenableDisposable disposable );
