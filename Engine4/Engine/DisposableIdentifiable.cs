using System.Diagnostics;

namespace Engine;

public abstract class DisposableIdentifiable : Identifiable, IDisposable {
	private bool _disposed;

	public DisposableIdentifiable() {
		this._disposed = false;
	}

	public DisposableIdentifiable( string name ) : base( name ) {
		this._disposed = false;
	}

	~DisposableIdentifiable() {
		if ( !this._disposed ) {
			this.LogWarning( "Undisposed! Attempting disposal." );
			Dispose();
		}
	}

	public bool Disposed => this._disposed;
	public event Action<object>? OnDisposed;

	public void Dispose() {
		if ( this._disposed ) {
			this.LogWarning( "Attempted to dispose when already disposed!" );
			return;
		}
		GC.SuppressFinalize( this );
		if ( OnDispose() ) {
			OnDisposed?.Invoke( this );
			this._disposed = true;
			this.LogLine( "Disposed!", Log.Level.LOW );
		} else {
			this.LogWarning( "Disposing failed!" );
			Debug.Fail( $"{this.FullName}: Undisposed!" );
		}
	}

	/// <returns>True if successfully disposed.</returns>
	protected abstract bool OnDispose();
}
