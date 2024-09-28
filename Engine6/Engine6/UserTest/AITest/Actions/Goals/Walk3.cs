using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserTest.AITest.Actions.Goals;
public sealed class Walk3( Agent3 agent ) : AgentGoalBase<Agent3>( agent, "Going for a walk", "Walking is good for you :) You never know what might be around the next bend!" ) {
	public override AgentSubGoalBase<Agent3> Evaluate() {
		throw new NotImplementedException();
	}
}
