using Game.VoxelCitySim.AI.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.VoxelCitySim.AI;
public sealed class Agent {

	//The agent contains the state of an entity in the world as is.
	//This data is then used to derive goals, and then actions to achieve those goals.
	//This using an HTN planner.

	//This then goes from traits (agent state) -> goals (derived from traits (such as hunger)) -> actions (to reach highly prioritized goal) -> world state changes.
	//Traits can lead to multiple goals, which can then lead to multiple actions being performed in optimal order. The priority of the goal determined which actions happen, which means if multiple goals are equally prioritized it can cause the agent to perform actions that might contradict each other.

	//

	private readonly Queue<ActionBase> _actionQueue = new();



}
