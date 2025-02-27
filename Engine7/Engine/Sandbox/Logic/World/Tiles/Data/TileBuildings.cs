using Sandbox.Logic.Setup;

namespace Sandbox.Logic.World.Tiles.Data;

public sealed class TileBuildings : TileDataBase, ITileData {
	private const string _dataCode = "0STR";
	public static int CodeId { get; } = _dataCode.ToIntCode();

	private readonly List<BuildingBase> _buildings = [];

	public event Action<BuildingBase>? BuildingAdded;
	public event Action<BuildingBase>? BuildingRemoved;

	public void AddBuilding( BuildingBase building ) {
		this._buildings.Add( building );
		BuildingAdded?.Invoke( building );
	}

	public void RemoveBuilding( BuildingBase building ) {
		_buildings.Remove( building );
		BuildingRemoved?.Invoke( building );
	}

	public IReadOnlyList<BuildingBase> Buildings => _buildings;
}