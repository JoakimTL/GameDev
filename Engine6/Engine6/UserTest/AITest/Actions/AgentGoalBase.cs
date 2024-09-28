namespace UserTest.AITest.Actions;

/// <summary>
/// Handles overarching goals for an agent. This means eating food, or finding a place to sleep.
/// </summary>
public abstract class AgentGoalBase<TAgent>( TAgent agent, string name, string description ) 
	where TAgent : 
		AgentBase {
	public TAgent Agent { get; } = agent;
	public string Name { get; } = name;
	public string Description { get; } = description;
	public abstract AgentSubGoalBase<TAgent> Evaluate();
}
