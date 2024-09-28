namespace UserTest.AITest;

public sealed class AgentRelationships {

	private readonly Dictionary<ulong, AgentRelationship> _relationships;

	public AgentRelationships() {
		_relationships = [];
	}

	public bool HasRelationship( AgentBase otherAgent ) => _relationships.ContainsKey( otherAgent.Uid );

	public AgentRelationship GetRelationship( AgentBase otherAgent ) {
		if (!_relationships.TryGetValue( otherAgent.Uid, out AgentRelationship? relationship ))
			_relationships.Add( otherAgent.Uid, relationship = new( otherAgent ) );
		return relationship;
	}

}
