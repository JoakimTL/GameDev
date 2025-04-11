using Sandbox.Logic.Nations.People;
using Sandbox.Logic.Setup;
using Sandbox.Logic.World.Tiles;
using Sandbox.Logic.World.Tiles.Data;
using Sandbox.Logic.World.Time;

namespace Sandbox.Logic.Nations;
public sealed class PopulationCenter : ITickable {
	/*
	 * Has people, resources, buildings, culture and technology
	 */

	private static ushort _currentAvailableId = 0;

	public ushort Id { get; }
	private readonly Census _census;
	private readonly ResourceContainer _resources;
	private readonly List<Tile> _tiles;
	private readonly HashSet<BuildingBase> _buildings;

	public event Action<Tile>? TileAdded;
	public event Action<Tile>? TileRemoved;
	public event Action<BuildingBase>? BuildingAdded;
	public event Action<BuildingBase>? BuildingRemoved;

	public PopulationCenter() {
		if (_currentAvailableId == ushort.MaxValue)
			throw new InvalidOperationException( "No more available IDs for PopulationCenter." );
		Id = _currentAvailableId++;
		_census = new();
		_resources = new();
		_tiles = [];
		_buildings = [];
		TileAdded += OnTileAdded;
		TileRemoved += OnTileRemoved;
	}

	private void OnTileAdded( Tile tile ) {
		TileBuildings tileBuildings = tile.DataModel.GetData<TileBuildings>();
		foreach (BuildingBase building in tileBuildings.Buildings)
			AddBuilding( building );
		tileBuildings.BuildingAdded += OnBuildingAdded;
		tileBuildings.BuildingRemoved += OnBuildingRemoved;
	}

	private void OnTileRemoved( Tile tile ) {
		TileBuildings tileBuildings = tile.DataModel.GetData<TileBuildings>();
		foreach (BuildingBase building in tileBuildings.Buildings)
			RemoveBuilding( building );
		tileBuildings.BuildingAdded -= OnBuildingAdded;
		tileBuildings.BuildingRemoved -= OnBuildingRemoved;
	}

	private void OnBuildingAdded( BuildingBase building ) => AddBuilding( building );

	private void OnBuildingRemoved( BuildingBase building ) => RemoveBuilding( building );

	public void MoveTile( Tile tile, PopulationCenter otherPopulationCenter ) {
		RemoveTile( tile );
		otherPopulationCenter.AddTile( tile );
	}

	private void RemoveBuilding( BuildingBase building ) {
		_buildings.Remove( building );
		BuildingRemoved?.Invoke( building );
	}

	public void AddTile( Tile tile ) {
		_tiles.Add( tile );
		TileAdded?.Invoke( tile );
	}

	public void RemoveTile( Tile tile ) {
		_tiles.Remove( tile );
		TileRemoved?.Invoke( tile );
	}
	private void AddBuilding( BuildingBase building ) {
		_buildings.Add( building );
		BuildingAdded?.Invoke( building );
	}

	public void Tick( GameClock gameClock ) {
		gameClock.
	}
}
