namespace Sandbox.Logic.Old.OldWorld.Tiles.Generation;

public sealed class TileTerrainGenerationLandscapeData( Tile tile, float rng, float height ) {
	public Tile Tile { get; } = tile;
	private readonly List<TerrainType> _availableTerrainTypes = [ .. TerrainType.AllLandTerrainTypes ];
	private readonly float _rng = rng;
	public int NumberOfAvailableTerrainTypes => _availableTerrainTypes.Count;
	public bool Applied { get; private set; } = false;
	public float Height { get; } = height;
	public void ApplyTerrainType( ref int lowestPossibleTerrainCount ) {
		if (Applied)
			throw new InvalidOperationException( "Terrain type already applied" );
		if (_availableTerrainTypes.Count == 0)
			throw new InvalidOperationException( "No terrain types available" );
		Applied = true;
		Tile.SetTerrainType( ChooseTerrainType() );
		Tile.TerrainGenData = null;
		foreach (Tile neighbour in Tile.Neighbours)
			neighbour.TerrainGenData?.UpdateAvailableTerrainTypes( ref lowestPossibleTerrainCount );
	}

	private int ChooseTerrainType() {
		Span<int> neighbourTerrain = stackalloc int[ Tile.Neighbours.Count ];
		for (int i = 0; i < neighbourTerrain.Length; i++)
			neighbourTerrain[ i ] = Tile.Neighbours[ i ].Terrain?.Id ?? -1;
		float maxWeight = 0;
		for (int i = 0; i < neighbourTerrain.Length; i++) {
			if (neighbourTerrain[ i ] == -1)
				continue;
			for (int j = 0; j < _availableTerrainTypes.Count; j++)
				maxWeight += TerrainAdjacencyRules.GetWeight( _availableTerrainTypes[ j ].Id, neighbourTerrain[ i ] );
		}
		if (maxWeight == 0)
			return TerrainType.Grass.Id;//_availableTerrainTypes[ (int) Math.Round( _rng * (_availableTerrainTypes.Count - 1) ) ].Id;
		float weightInv = 1 / maxWeight;
		float sum = 0;
		for (int i = 0; i < neighbourTerrain.Length; i++) {
			if (neighbourTerrain[ i ] == -1)
				continue;
			for (int j = 0; j < _availableTerrainTypes.Count; j++) {
				sum += TerrainAdjacencyRules.GetWeight( _availableTerrainTypes[ j ].Id, neighbourTerrain[ i ] ) * weightInv;
				if (_rng <= sum)
					return _availableTerrainTypes[ j ].Id;
			}
		}
		return TerrainType.Grass.Id;//_availableTerrainTypes[ (int) Math.Round( _rng * (_availableTerrainTypes.Count - 1) ) ].Id;
	}

	public void UpdateAvailableTerrainTypes( ref int lowestPossibleTerrainCount ) {
		int previousCount = _availableTerrainTypes.Count;
		foreach (Tile tileNeighbour in Tile.Neighbours) {
			TerrainType? neighbouringTerrainType = tileNeighbour.Terrain;
			if (neighbouringTerrainType is not null)
				_availableTerrainTypes.RemoveAll( t => !TerrainAdjacencyRules.IsTileAllowedNextTo( t.Id, neighbouringTerrainType.Id ) );
		}
		if (previousCount != _availableTerrainTypes.Count) {
			if (_availableTerrainTypes.Count < lowestPossibleTerrainCount)
				lowestPossibleTerrainCount = _availableTerrainTypes.Count;
			foreach (Tile neighbour in Tile.Neighbours)
				neighbour.TerrainGenData?.UpdateAvailableTerrainTypes( ref lowestPossibleTerrainCount );
		}
	}
}
