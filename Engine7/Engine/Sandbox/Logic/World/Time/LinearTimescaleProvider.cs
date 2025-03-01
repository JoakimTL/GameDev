namespace Sandbox.Logic.World.Time;

public sealed class LinearTimescaleProvider : ITimescaleProvider {

	private readonly IReadOnlyList<TimescalePoint> _points;

	public LinearTimescaleProvider( params Span<TimescalePoint> points ) {
		_points = points.ToArray();
	}

	public double GetDaysPassingPerTick( GameClock gameClock ) {
		if (_points.Count == 0)
			return 0;

		double t = gameClock.CurrentYear;

		double daysPerTick = _points[ 0 ].DaysPerTick;
		for (int i = 1; i < _points.Count; i++) {
			if (_points[ i ].Year > t)
				break;
			daysPerTick = _points[ i ].DaysPerTick;
		}
		return daysPerTick;
	}
}
