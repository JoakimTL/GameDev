namespace Engine.Time;

/// <summary>
/// Enforces a periodic exectution of code by blocking for a certain amount of time. The period enforcer expects the block method to be used for each loop. 
/// </summary>
public sealed class TimedThreadBlocker<TTickSupplier> : DisposableIdentifiable, ITimedThreadBlocker
	where TTickSupplier :
		ITickSupplier {

	private static readonly double _invFreqMs = 1 / (TTickSupplier.Frequency / 1000d);

	public bool Cancelled => _threadBlocker.Cancelled;
	public uint PeriodMs { get; private set; }
	public double AccumulatedMs { get; private set; }

	private readonly IThreadBlocker _threadBlocker;
	private long _lastPeriodStartTick;

	public TimedThreadBlocker( IThreadBlocker threadBlocker, uint periodMs ) {
		_threadBlocker = threadBlocker ?? throw new ArgumentNullException( nameof( threadBlocker ) );
		_lastPeriodStartTick = TTickSupplier.Ticks;
		PeriodMs = periodMs;
	}

	public void Set() {
		_lastPeriodStartTick = TTickSupplier.Ticks;
		AccumulatedMs = 0;
	}

	public TimedBlockerState Block() {
		if (Cancelled || Disposed)
			return TimedBlockerState.Cancelled;

        long currentTick = TTickSupplier.Ticks;
		long tickDelta = currentTick - _lastPeriodStartTick;
		_lastPeriodStartTick = currentTick;

		double elapsedMilliseconds = tickDelta * _invFreqMs;
		AccumulatedMs += elapsedMilliseconds - PeriodMs;
		this.LogLine( $"Blocking! {PeriodMs}ms A:{AccumulatedMs:N4}ms {currentTick}t {tickDelta}dt {elapsedMilliseconds:N4}dms" );

		if (AccumulatedMs > PeriodMs) {
			double skippedPeriods = Math.Floor( AccumulatedMs / PeriodMs );
			AccumulatedMs -= skippedPeriods * PeriodMs;
			this.LogWarning( $"Skipping {skippedPeriods:N0} periods, equivalent to {skippedPeriods * PeriodMs}ms!" );
			return TimedBlockerState.Skipping;
		}

		return AccumulatedMs < 0
			? _threadBlocker.Block( (uint) Math.Abs( AccumulatedMs ) ) 
				? TimedBlockerState.Blocking
				: TimedBlockerState.Cancelled
			: TimedBlockerState.NonBlocking;
	}

	public void SetPeriod( uint periodMs ) => PeriodMs = periodMs;
	public void Cancel() => _threadBlocker.Cancel();

	protected override bool InternalDispose() {
		Cancel();
		return true;
	}
}
