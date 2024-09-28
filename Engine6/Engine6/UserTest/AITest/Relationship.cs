namespace UserTest.AITest;

public sealed class Relationship {

	private readonly Dictionary<RelationshipTrait, double> _traits;

	public Relationship() {
		_traits = [];
	}

	public double Get( RelationshipTrait trait ) => _traits[ trait ];

	public RelationshipRole DefineRelationship() => RelationshipRole.FindFitting( this );
}
