namespace Sandbox.Logic.Setup;

public abstract class BuildingTypeBase : SelfIdentifyingBase {
	protected BuildingTypeBase( string name, ResourceTable constructionCost, bool permanentBuilding = true ) {
		this.Name = name;
		this.ConstructionCost = constructionCost;
		this.IsPermanent = permanentBuilding;
	}

	public string Name { get; }
	public ResourceTable ConstructionCost { get; }
	public bool IsPermanent { get; }
}

public sealed class ResourceTable {
	private readonly List<ResourceAmount> _resources;

	public ResourceTable( params List<ResourceAmount> resources ) {
		this._resources = resources;
	}

	public void AddResource( ResourceAmount resourceAmount ) => _resources.Add( resourceAmount );

	public IReadOnlyList<ResourceAmount> Resources => _resources;
}

public sealed class ResourceContainer {

	private readonly Dictionary<ResourceBase, double> _resources;

	public ResourceContainer() {
		_resources = [];
	}

	public void ChangeResourceAmount( ResourceBase resource, double amount ) {
		if (amount == 0)
			return;
		if (!_resources.ContainsKey( resource ))
			_resources.Add( resource, 0 );
		_resources[ resource ] += amount;
	}

	public void SetResourceAmount( ResourceBase resource, double amount ) => _resources[ resource ] = amount;

	public double GetResourceAmount( ResourceBase resource ) => _resources.TryGetValue( resource, out double amount ) ? amount : 0;

	public void ConsumeResources( ResourceTable consumption ) {
		foreach (ResourceAmount resourceAmount in consumption.Resources)
			ChangeResourceAmount( resourceAmount.Resource, -resourceAmount.AmountKg );
	}
	public void AddResources( ResourceTable addition ) {
		foreach (ResourceAmount resourceAmount in addition.Resources)
			ChangeResourceAmount( resourceAmount.Resource, resourceAmount.AmountKg );
	}

	public ResourceTable GetAllResources() => new( _resources.Select( p => new ResourceAmount( p.Key, p.Value ) ).ToList() );

	public void UpdateTableValues( ResourceTable table ) {
		foreach (KeyValuePair<ResourceBase, double> kvp in _resources) {
			ResourceAmount? resourceAmount = table.Resources.FirstOrDefault( p => p.Resource == kvp.Key );
			if (resourceAmount == null)
				table.AddResource( new( kvp.Key, kvp.Value ) );
			else
				resourceAmount.AmountKg = kvp.Value;
		}
	}

}

public static class BuildingTypeList {
	private static readonly Dictionary<Type, BuildingTypeBase> _buildingTypes;

	static BuildingTypeList() {
		_buildingTypes = [];
		IEnumerable<Type> buildingTypes = TypeManager.Registry.ImplementationTypes.Where( p => p.IsAssignableTo( typeof( BuildingTypeBase ) ) );
		foreach (Type type in buildingTypes) {
			if (!TypeManager.ResolveType( type ).HasParameterlessConstructor)
				throw new InvalidOperationException( $"Building type {type.Name} does not have a parameterless constructor." );
			BuildingTypeBase buildingType = type.CreateInstance( null ) as BuildingTypeBase ?? throw new InvalidOperationException( $"Building type {type.Name} could not be instantiated." );
			_buildingTypes.Add( type, buildingType );
		}
	}

	public static T Get<T>() where T : BuildingTypeBase => (T) _buildingTypes[ typeof( T ) ];
}

public abstract class BuildingBase<T> where T : BuildingTypeBase {
	private readonly T _buildingType;

	protected BuildingBase() {
		_buildingType = BuildingTypeList.Get<T>();
	}

	public T BuildingType => _buildingType;

	/// <summary>
	/// A number between 0 and 1, where 1 is perfect condition.
	/// </summary>
	public float BuildingCondition { get; protected set; }
	/// <summary>
	/// Decay rate in percent each year when left empty.
	/// </summary>
	public float BuildingDecayRate { get; protected set; }

	public abstract int GetCurrentEmployment<TProfession>() where TProfession : ProfessionBase;
	public abstract int GetMaxEmployment<TProfession>() where TProfession : ProfessionBase;
}

public abstract class ProfessionBase : SelfIdentifyingBase {
	protected ProfessionBase( string name ) {
		this.Name = name;
	}
	public string Name { get; }
}