using Engine.Time;

namespace Engine.Modules;

/// <param name="frequency">Desired frequency of execution in <c>Hz</c>.</param>
public sealed class FrequencyDelayedLoopTimer( double frequency ) : IModuleLoopTimer {
	private readonly TimedThreadBlocker<StopwatchTickSupplier> _timedThreadBlocker = new( new ThreadBlocker(), frequency.ToPeriodMs() );
	public bool Block() => _timedThreadBlocker.Block() != TimedBlockerState.Cancelled;
	public void Cancel() => _timedThreadBlocker.Cancel();
}
