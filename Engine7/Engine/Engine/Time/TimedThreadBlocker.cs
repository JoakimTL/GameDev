namespace Engine.Time;

/// <summary>
/// Enforces a periodic exectution of code by blocking for a certain amount of time. The period enforcer expects the block method to be used for each loop. 
/// </summary>
public sealed class TimedThreadBlocker<TTickSupplier> : DisposableIdentifiable, ITimedThreadBlocker
	where TTickSupplier :
		ITickSupplier {

	private static readonly double _invFreqMs = 1 / (TTickSupplier.Frequency / 1000d);

	public bool Cancelled => this._threadBlocker.Cancelled;
	public uint PeriodMs { get; private set; }
	public double AccumulatedMs { get; private set; }

	private readonly IThreadBlocker _threadBlocker;
	private long _lastPeriodStartTick;

	public TimedThreadBlocker( IThreadBlocker threadBlocker, uint periodMs ) {
		this._threadBlocker = threadBlocker ?? throw new ArgumentNullException( nameof( threadBlocker ) );
		this._lastPeriodStartTick = TTickSupplier.Ticks;
		this.PeriodMs = periodMs;
	}

	public void Set() {
		this._lastPeriodStartTick = TTickSupplier.Ticks;
		this.AccumulatedMs = 0;
	}

	public TimedBlockerState Block() {
		if (this.Cancelled || this.Disposed)
			return TimedBlockerState.Cancelled;

        long currentTick = TTickSupplier.Ticks;
		long tickDelta = currentTick - this._lastPeriodStartTick;
		this._lastPeriodStartTick = currentTick;

		double elapsedMilliseconds = tickDelta * _invFreqMs;
		this.AccumulatedMs += elapsedMilliseconds - this.PeriodMs;
		this.LogLine( $"Blocking! {this.PeriodMs}ms A:{this.AccumulatedMs:N4}ms {currentTick}t {tickDelta}dt {elapsedMilliseconds:N4}dms", Log.Level.VERBOSE, ConsoleColor.DarkGray );

		if (this.AccumulatedMs > this.PeriodMs) {
			double skippedPeriods = Math.Floor( this.AccumulatedMs / this.PeriodMs );
			this.AccumulatedMs -= skippedPeriods * this.PeriodMs;
			this.LogWarning( $"Skipping {skippedPeriods:N0} period{(skippedPeriods > 1 ? "s" : "")}, equivalent to {skippedPeriods * this.PeriodMs}ms!" );
			return TimedBlockerState.Skipping;
		}

		return this.AccumulatedMs < 0
			? this._threadBlocker.Block( (uint) Math.Abs( this.AccumulatedMs ) ) 
				? TimedBlockerState.Blocking
				: TimedBlockerState.Cancelled
			: TimedBlockerState.NonBlocking;
	}

	public void SetPeriod( uint periodMs ) => this.PeriodMs = periodMs;
	public void Cancel() => this._threadBlocker.Cancel();

	protected override bool InternalDispose() {
		Cancel();
		return true;
	}
}
