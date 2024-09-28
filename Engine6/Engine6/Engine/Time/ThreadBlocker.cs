namespace Engine.Time;

public sealed class ThreadBlocker : DisposableIdentifiable, IThreadBlocker {

	private readonly ManualResetEvent _resetEvent = new( false );
	public bool Cancelled { get; private set; } = false;

	public bool Block( uint milliseconds ) => !Cancelled && !this._resetEvent.WaitOne( (int) milliseconds );
	public void Cancel() {
		this._resetEvent.Set();
		Cancelled = true;
	}

	protected override bool InternalDispose() {
		Cancel();
		_resetEvent.Dispose();
		return true;
	}
}
