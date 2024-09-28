using Engine;
using Engine.Standard.ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserTest.AITest.Actions.Actions;
public sealed class MoveTo3( Agent3 agent, Vector3<double> destination ) : IAgentAction {

	private readonly Agent3 _agent = agent;
	private readonly Vector3<double> _destination = destination;

	//path

	//move each tick
	//check if blocked
	//if blocked
	//	check for new path
	//if no new path
	//	return reevaluate
	//check if arrived
	//if arrived, return completed

	public ActionState PerformAction() {
		//if (!_agent.Entity.TryGetComponent( out Transform3Component? t3c ) || !_agent.Entity.TryGetComponent(out CharacterBody3Component? cb3c))
		//	return ActionState.Failed;

		Console.WriteLine( "MoveTo3" );
		return ActionState.Completed;
	}
}
