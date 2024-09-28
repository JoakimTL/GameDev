namespace UserTest.AITest;

public sealed class AgentRelationship {

	/// <summary>
	/// The agent the main agent has a relation to.
	/// </summary>
	private readonly Guid _relatedAgentEntityId;

	public Relationship Relationship { get; }

	public AgentRelationship( AgentBase relatedAgent ) {
		_relatedAgentEntityId = relatedAgent.Entity.EntityId;
		Relationship = new();
	}
}
