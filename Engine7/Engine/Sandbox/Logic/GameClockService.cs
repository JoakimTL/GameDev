using Sandbox.Logic.World.Time;

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
		if (Clock.TimeScaleProvider is null)
			return;
		Clock.Update();
	}
}
