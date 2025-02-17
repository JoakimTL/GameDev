using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic;
public sealed class GameClockService : IUpdateable {
	public void Update( double time, double deltaTime ) {

	}
}

public sealed class GameClock {

	private double _daysSinceStart;

	public GameClock( ulong daysSinceStart = 0 ) {
		_daysSinceStart = daysSinceStart;
	}

	//TODO: need a way to slowly ramp down the days passing each tick as we near the end of the game

	public double YearsSinceStart => _daysSinceStart / 365.24225;

}
