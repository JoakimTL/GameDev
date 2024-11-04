namespace Engine.Time;

public sealed class ThreadBlocker : DisposableIdentifiable, IThreadBlocker {

	private readonly ManualResetEvent _resetEvent = new( false );
	public bool Cancelled { get; private set; } = false;

	public bool Block( uint milliseconds ) => !this.Cancelled && !this._resetEvent.WaitOne( (int) milliseconds );

	public void Cancel() {
		this._resetEvent.Set();
		this.Cancelled = true;
	}

	protected override bool InternalDispose() {
		Cancel();
		this._resetEvent.Dispose();
		return true;
	}
}
