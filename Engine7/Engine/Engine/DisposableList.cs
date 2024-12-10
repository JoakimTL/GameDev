
namespace Engine;

public sealed class DisposableList : DisposableIdentifiable {
	private readonly HashSet<IListenableDisposable> _disposables = [];

	public void Add( IListenableDisposable disposable ) {
		disposable.OnDisposed += OnDisposableDisposed;
		this._disposables.Add( disposable );
	}
	public void Remove( IListenableDisposable disposable ) {
		disposable.OnDisposed -= OnDisposableDisposed;
		this._disposables.Remove( disposable );
	}

	private void OnDisposableDisposed( IListenableDisposable disposable ) {
		disposable.OnDisposed -= OnDisposableDisposed;
		this._disposables.Remove( disposable );
	}

	protected override bool InternalDispose() {
		foreach (IListenableDisposable disposable in this._disposables) {
			disposable.OnDisposed -= OnDisposableDisposed;
			disposable.Dispose();
		}
		return true;
	}

	public void Clear( bool disposeCleared ) {
		if (disposeCleared)
			foreach (IListenableDisposable disposable in this._disposables) {
				disposable.OnDisposed -= OnDisposableDisposed;
				disposable.Dispose();
			}
		this._disposables.Clear();
	}
}