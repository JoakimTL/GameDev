using Sandbox.Logic;

namespace Sandbox;

public interface ITickable {
	void Tick( GameClock gameClock );
}