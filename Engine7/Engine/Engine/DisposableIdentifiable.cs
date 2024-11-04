using System.Diagnostics;

namespace Engine;

public abstract class DisposableIdentifiable : Identifiable {

	public bool Disposed { get; private set; } = false;

	/// <summary>
	/// Invoked after diposal is complete.
	/// </summary>
	public event Action? OnDisposed;

	~DisposableIdentifiable() {
		if (!this.Disposed)
			Debug.Fail( $"Object \"{this}\" was not disposed before destruction!" );
	}

	public void Dispose() {
		if (this.Disposed)
			return;
		if (InternalDispose()) {
			this.Disposed = true;
			OnDisposed?.Invoke();
		}
		GC.SuppressFinalize( this );
	}

	/// <returns>True if the object was fully disposed. False if there are still undisposed parts.</returns>
	protected abstract bool InternalDispose();
}
