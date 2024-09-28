using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserTest.AITest.Actions.Actions;

namespace UserTest.AITest.Actions.SubGoals;
public class Move3 : AgentSubGoalBase<Agent3> {

	public Move3( Agent3 agent ) : base( agent ) {
	}

	public override ActionPath Evaluate() {
		return new ActionPath( new MoveTo3( Agent, new Vector3<double>( 0, 0, 0 ) ) );
	}
}
