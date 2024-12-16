using System.Diagnostics.CodeAnalysis;

namespace Sandbox.Logic.World.Generation;

public static class TileTerrainGenerator {
	public static bool GenerateTerrain( Tile tile, float rng, float height, [NotNullWhen( true )] out TileTerrainGenerationLandscapeData? landscapeData ) {
		landscapeData = null;
		if (height < 0) {
			if (height < -.07f) 				tile.SetTerrainType( TerrainType.Water.Id );
else 				tile.SetTerrainType( height > -0.01f ? TerrainType.Sand.Id : TerrainType.ShallowWater.Id );
			return false;
		}
		landscapeData = new( tile, rng, height );
		return true;
	}
}
