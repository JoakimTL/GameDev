namespace UserTest.AITest;

public sealed class RelationshipRole {
	private static readonly List<RelationshipRole> _roles = [];
	private static void Register( RelationshipRole role ) => _roles.Add( role );

	public static RelationshipRole FindFitting( Relationship relationship ) {
		(RelationshipRole? role, double fitness) fittest = (Unknown, 0.1);
		foreach (RelationshipRole role in _roles) {
			double fitness = role.GetFitness( relationship );
			if (fitness > fittest.fitness)
				fittest = (role, fitness);
		}
		return fittest.role;
	}

	public static readonly RelationshipRole Enemy = new("Enemy", (RelationshipTrait.Trustworthiness, -1));
	public static readonly RelationshipRole Friendship = new("Friend", (RelationshipTrait.Trustworthiness, 1), (RelationshipTrait.Interest, 0) );
	//public static readonly RelationshipRole Rival = new("Rival", (RelationshipTrait., 1) );
	//public static readonly RelationshipRole Colleague = new("Colleague");
	//public static readonly RelationshipRole Hateful = new("Hateful");
	public static readonly RelationshipRole Romantic = new("Romantic", (RelationshipTrait.Trustworthiness, 1), (RelationshipTrait.Interest, 1), (RelationshipTrait.BloodRelation, 0) );
	//public static readonly RelationshipRole Acquaintance = new("Acquantance");
	public static readonly RelationshipRole Family = new("Family", (RelationshipTrait.Familiality, 1) );
	public static readonly RelationshipRole Unknown = new("Unknown");

	private readonly (RelationshipTrait, double)[] _desiredValues;

	public string Name { get; }

	public RelationshipRole( string name, params (RelationshipTrait, double)[] desiredValues ) {
		this.Name = name;
		this._desiredValues = desiredValues;
		Register( this );
	}

	public double GetFitness( Relationship relationship ) {
		double fitness = 0;
		foreach ((RelationshipTrait, double) value in _desiredValues)
			fitness += relationship.Get( value.Item1 ) * value.Item2;
		return fitness / _desiredValues.Length;
	}
}
