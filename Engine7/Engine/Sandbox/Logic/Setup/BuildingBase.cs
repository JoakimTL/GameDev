namespace Sandbox.Logic.Setup;

public abstract class BuildingBase( BuildingTypeBase buildingType ) : Identifiable {

	private readonly BuildingTypeBase _buildingType = buildingType;
	public Employer Employer { get; } = new();

	public BuildingTypeBase BuildingType => _buildingType;

	/// <summary>
	/// A number between 0 and 1, where 1 is perfect condition.
	/// </summary>
	public float BuildingCondition { get; protected set; }
	/// <summary>
	/// Decay rate in percent each year when left empty.
	/// </summary>
	public float BuildingDecayRate { get; protected set; }
}

public abstract class BuildingBase<T>() : BuildingBase( Definitions.BuildingTypes.Get<T>() ) where T : BuildingTypeBase;

public sealed class Employer {
	private readonly Dictionary<ProfessionTypeBase, int> _maximumEmployment;
	private readonly Dictionary<ProfessionTypeBase, int> _wantedEmployment;
	private readonly Dictionary<ProfessionTypeBase, int> _currentEmployment;
	private readonly double _currentRevenue;
	private readonly double _currentExpenses;

	public Employer() {
		_maximumEmployment = [];
		_wantedEmployment = [];
		_currentEmployment = [];
	}

	public void SetMaxEmployment( ProfessionTypeBase professionType, int count ) => _maximumEmployment[ professionType ] = count;
	public void SetWantedEmployment( ProfessionTypeBase professionType, int count ) => _wantedEmployment[ professionType ] = count;
	public void SetMaxEmployment<TProfession>( int count ) where TProfession : ProfessionTypeBase => _maximumEmployment[ Definitions.Professions.Get<TProfession>() ] = count;
	public void SetWantedEmployment<TProfession>( int count ) where TProfession : ProfessionTypeBase => _wantedEmployment[ Definitions.Professions.Get<TProfession>() ] = count;
}