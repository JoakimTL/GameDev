namespace Sandbox.Logic.Setup;

public abstract class BuildingTypeBase : SelfIdentifyingBase {
	protected BuildingTypeBase( string name, bool permanentBuilding = true ) {
		this.Name = name;
		this.IsPermanent = permanentBuilding;
	}

	public string Name { get; }
	public bool IsPermanent { get; }
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
	/// Decay rate in percent each year.
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