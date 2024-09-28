using System;

namespace UserTest.AITest.Actions;

/// <summary>
/// Handles minute actions for an agent. This means moving from one point to another, or picking up an item.
/// </summary>
public abstract class AgentSubGoalBase<TAgent>( TAgent agent )
	where TAgent : 
		AgentBase {
	public TAgent Agent { get; } = agent;
	public abstract ActionPath Evaluate();
}
