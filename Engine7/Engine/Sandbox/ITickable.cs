using Sandbox.Logic.World.Time;

namespace Sandbox;

public interface ITickable {
	void Tick( GameClock gameClock );
}