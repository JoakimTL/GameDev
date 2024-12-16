namespace Sandbox.Logic.World.Generation;

public static class TerrainAdjacencyRules {

	private static readonly List<List<float>> _isTileAllowedNextTo;

	static TerrainAdjacencyRules() {
		_isTileAllowedNextTo = [];
	}

	public static void Initialize() {
		for (int i = 0; i < TerrainType.AllTerrainTypes.Count; i++)
			_isTileAllowedNextTo.Add( [] );
		for (int i = 0; i < TerrainType.AllTerrainTypes.Count; i++)
			for (int j = 0; j < TerrainType.AllTerrainTypes.Count; j++)
				_isTileAllowedNextTo[ i ].Add( 0 );
	}

	public static bool IsTileAllowedNextTo( int terrainTypeA, int terrainTypeB ) => _isTileAllowedNextTo[ terrainTypeA ][ terrainTypeB ] > 0;
	public static float GetWeight( int terrainTypeA, int terrainTypeB ) => _isTileAllowedNextTo[ terrainTypeA ][ terrainTypeB ];

	public static void SetAdjacencyWeight( int terrainTypeA, int terrainTypeB, float weight ) {
		_isTileAllowedNextTo[ terrainTypeA ][ terrainTypeB ] = weight;
		_isTileAllowedNextTo[ terrainTypeB ][ terrainTypeA ] = weight;
	}

}
