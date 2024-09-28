using System.Diagnostics;

namespace Engine;

public abstract class DisposableIdentifiable : Identifiable, IDisposable {

	public bool Disposed { get; private set; } = false;

	/// <summary>
	/// Invoked after diposal is complete.
	/// </summary>
	public event Action? OnDisposed;

	~DisposableIdentifiable() {
		if (!Disposed)
			Debug.Fail( $"Object \"{this.FullName}\" was not disposed before destruction!" );
	}

	public void Dispose() {
		if (Disposed)
			return;
		if (InternalDispose()) {
			Disposed = true;
			OnDisposed?.Invoke();
		}
		GC.SuppressFinalize( this );
	}

	/// <returns>True if the object was fully disposed. False if there are still undisposed parts.</returns>
	protected abstract bool InternalDispose();
}