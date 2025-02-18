using Engine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic;
public sealed class GameClockService : IUpdateable {

	public GameClock Clock { get; }

	public GameClockService() {
		Clock = new GameClock();
	}

	public void Start( ITimescaleProvider timescaleProvider ) {
		Clock.Start( timescaleProvider );
	}

	public void Update( double time, double deltaTime ) {

	}
}

public sealed class GameClock {

	private ITimescaleProvider? _timeScaleProvider;
	private double _daysSinceStart;
	private int _currentTick;

	public GameClock() {
		_timeScaleProvider = null;
		_daysSinceStart = 0;
		_currentTick = 0;
	}

	public void Start( ITimescaleProvider timeScaleProvider ) {
		_timeScaleProvider = timeScaleProvider;
		_daysSinceStart = 0;
		_currentTick = 0;
	}

	//TODO: need a way to slowly ramp down the days passing each tick as we near the end of the game

	public double DaysSinceStart => _daysSinceStart;

	public double YearsSinceStart => _daysSinceStart / 365.24225;

	public double GameSpeed { get; private set; }



	public void Update() {
		if (_timeScaleProvider is null) {
			this.LogWarning( "Game clock must have a timescale provider provided." );
			return;
		}
		_daysSinceStart += _timeScaleProvider.GetDaysPassingPerTick( this );
		_currentTick++;
	}

}

public interface ITimescaleProvider {
	double GetDaysPassingPerTick( GameClock gameClock );
}

public sealed class SigmoidTimescaleProvider : ITimescaleProvider {

	private readonly IReadOnlyList<TimescalePoint> _points;

	public SigmoidTimescaleProvider( params Span<TimescalePoint> points ) {
		_points = points.ToArray();
	}

	private static double Logistic( double t, double tCenter, double k ) => 1.0 / (1.0 + double.Exp( -k * (t - tCenter) ));

	public double GetDaysPassingPerTick( GameClock gameClock ) {
		if (_points.Count == 0)
			return 0;

		double t = gameClock.YearsSinceStart;

		double sum = _points[ 0 ].DaysPerTick;
		for (int i = 1; i < _points.Count; i++)
			sum -= (_points[ i - 1 ].DaysPerTick - _points[ i ].DaysPerTick) * Logistic( t, _points[ i ].Year, 0.002 );
		return sum;

		//double S1 = Logistic( t, 6000, 0.002 );
		//double S2 = Logistic( t, 8000, 0.002 );
		//double S3 = Logistic( t, 9000, 0.002 );
		//double S4 = Logistic( t, 10000, 0.002 );
		//double S5 = Logistic( t, 10500, 0.002 );
		//double S6 = Logistic( t, 11000, 0.002 );

		//return 70 - 49 * S1 - 7 * S2 - 7 * S3 - 3 * S4 - 2 * S5 - 1 * S6;
	}
}
public sealed class LinearTimescaleProvider : ITimescaleProvider {

	private readonly IReadOnlyList<TimescalePoint> _points;

	public LinearTimescaleProvider( params Span<TimescalePoint> points ) {
		_points = points.ToArray();
	}

	public double GetDaysPassingPerTick( GameClock gameClock ) {
		if (_points.Count == 0)
			return 0;

		double t = gameClock.YearsSinceStart;

		double daysPerTick = _points[ 0 ].DaysPerTick;
		for (int i = 1; i < _points.Count; i++) {
			if (_points[ i ].Year > t)
				break;
			daysPerTick = _points[ i ].DaysPerTick;
		}
		return daysPerTick;
	}
}

public readonly struct TimescalePoint( double year, double daysPerTick ) {
	public double Year { get; } = year;
	public double DaysPerTick { get; } = daysPerTick;
}