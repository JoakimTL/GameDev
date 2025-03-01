namespace Sandbox.Logic.World.Time;

public interface ITimescaleProvider {
	double GetDaysPassingPerTick( GameClock gameClock );
}
