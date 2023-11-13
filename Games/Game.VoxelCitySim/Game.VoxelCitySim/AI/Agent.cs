using Engine;
using Game.VoxelCitySim.AI.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.VoxelCitySim.AI;
public sealed class Agent : IUpdateable {

	//The agent contains the state of an entity in the world as is.
	//This data is then used to derive goals, and then actions to achieve those goals.
	//This using an HTN planner.

	//This then goes from traits (agent state) -> goals (derived from traits (such as hunger)) -> actions (to reach highly prioritized goal) -> world state changes.
	//Traits can lead to multiple goals, which can then lead to multiple actions being performed in optimal order. The priority of the goal determined which actions happen, which means if multiple goals are equally prioritized it can cause the agent to perform actions that might contradict each other.

	//

	private readonly GoalBase[] _goals;
	private readonly GoalBase[] _activeGoals;
	private readonly Queue<ActionBase> _actionQueue = new();
	private readonly Dictionary<Type, StateBase> _stateData = new();

	public Agent() {
		_goals = Engine.TypeHelper.AllTypes.Where( p => p.IsAssignableTo( typeof( GoalBase ) ) ).Select( p => (GoalBase?) Activator.CreateInstance( p ) ).OfType<GoalBase>().ToArray();
		_activeGoals = new GoalBase[5];
	}

	public void QueueAction<T>( T action ) where T : ActionBase => _actionQueue.Enqueue( action );

	public void SetState<T>( T state ) where T : StateBase => _stateData[ typeof( T ) ] = state;

	public T GetState<T>() where T : StateBase => (T) _stateData[ typeof( T ) ];

	public void Test() {
		var hunger = GetState<HungerState>();
	}

	public void Update( in double time, in double deltaTime ) {
		foreach (var goal in _goals) {
			goal.Update( this, time, deltaTime );
		}
	}
}
