using Engine.Logging;

namespace Sandbox.Logic.World.Time;

public sealed class GameClock {

	private ITimescaleProvider? _timeScaleProvider;
	private double _previousDay;
	private double _daysSinceStart;

	public GameClock() {
		_timeScaleProvider = null;
		_previousDay = 0;
		_daysSinceStart = 0;
	}

	public void Start( ITimescaleProvider timeScaleProvider ) {
		_timeScaleProvider = timeScaleProvider;
		_previousDay = 0;
		_daysSinceStart = 0;
	}

	//TODO: need a way to slowly ramp down the days passing each tick as we near the end of the game

	public ITimescaleProvider? TimeScaleProvider => _timeScaleProvider;

	public double PreviousDay => _previousDay;

	public double CurrentDay => _daysSinceStart;

	public double PreviousYear => _previousDay / 365.24225;

	public double CurrentYear => _daysSinceStart / 365.24225;

	public double DeltaDays => _daysSinceStart - _previousDay;

	public double GameSpeed { get; private set; }

	public void Update() {
		if (_timeScaleProvider is null) {
			this.LogWarning( "Game clock must have a timescale provider provided." );
			return;
		}
		_previousDay = _daysSinceStart;
		_daysSinceStart += _timeScaleProvider.GetDaysPassingPerTick( this );
	}

}
