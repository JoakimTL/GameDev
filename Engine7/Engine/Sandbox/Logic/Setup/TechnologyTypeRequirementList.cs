namespace Sandbox.Logic.Setup;

public sealed class TechnologyTypeRequirementList {
	public TechnologyTypeRequirementList( Type requiringType ) {
		List<Type> requirements = TypeManager.ResolveType( requiringType ).GetAttributes<IRequirement>().Select( p => p.RequiredType ).ToList();
		TechnologyRequirements = requirements.Where( p => p.IsAssignableTo( typeof( TechnologyTypeBase ) ) ).ToList();
		ResourceRequirements = requirements.Where( p => p.IsAssignableTo( typeof( ResourceTypeBase ) ) ).ToList();
		BuildingRequirements = requirements.Where( p => p.IsAssignableTo( typeof( BuildingTypeBase ) ) ).ToList();
		ProfessionRequirements = requirements.Where( p => p.IsAssignableTo( typeof( ProfessionTypeBase ) ) ).ToList();
	}

	public IReadOnlyList<Type> TechnologyRequirements { get; }
	public IReadOnlyList<Type> ResourceRequirements { get; }
	public IReadOnlyList<Type> BuildingRequirements { get; }
	public IReadOnlyList<Type> ProfessionRequirements { get; }
}