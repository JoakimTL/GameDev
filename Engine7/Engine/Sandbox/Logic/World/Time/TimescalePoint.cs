namespace Sandbox.Logic.World.Time;

public readonly struct TimescalePoint( double year, double daysPerTick ) {
	public double Year { get; } = year;
	public double DaysPerTick { get; } = daysPerTick;
}