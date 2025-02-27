namespace Sandbox.Logic.Setup;

public abstract class BuildingBase( BuildingTypeBase buildingType ) : Identifiable {

	private readonly BuildingTypeBase _buildingType = buildingType;
	private readonly Dictionary<ProfessionTypeBase, int> _currentEmployeeCount = [];

	public BuildingTypeBase BuildingType => _buildingType;

	/// <summary>
	/// A number between 0 and 1, where 1 is perfect condition.
	/// </summary>
	public float BuildingCondition { get; protected set; }
	/// <summary>
	/// Decay rate in percent each year when left empty.
	/// </summary>
	public float BuildingDecayRate { get; protected set; }

	public abstract int GetCurrentEmployment<TProfession>() where TProfession : ProfessionTypeBase;
	public abstract int GetMaxEmployment<TProfession>() where TProfession : ProfessionTypeBase;
}

public abstract class BuildingBase<T>() : BuildingBase( Definitions.BuildingTypes.Get<T>() ) where T : BuildingTypeBase;
