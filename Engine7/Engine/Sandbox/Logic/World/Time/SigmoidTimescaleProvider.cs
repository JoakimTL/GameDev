namespace Sandbox.Logic.World.Time;

public sealed class SigmoidTimescaleProvider : ITimescaleProvider {

	private readonly IReadOnlyList<TimescalePoint> _points;

	public SigmoidTimescaleProvider( params Span<TimescalePoint> points ) {
		_points = points.ToArray();
	}

	private static double Logistic( double t, double tCenter, double k ) => 1.0 / (1.0 + double.Exp( -k * (t - tCenter) ));

	public double GetDaysPassingPerTick( GameClock gameClock ) {
		if (_points.Count == 0)
			return 0;

		double t = gameClock.CurrentYear;

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
