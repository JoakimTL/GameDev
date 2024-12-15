using System.Diagnostics.CodeAnalysis;

namespace Sandbox.Logic.World;

public static class TileTerrainGenerator {
	public static bool GenerateTerrain( Tile tile, float rng, float height, [NotNullWhen( true )] out TileTerrainGenerationLandscapeData? landscapeData ) {
		landscapeData = null;
		if (height < 0) {
			if (height < -.05f) {
				tile.SetTerrainType( TerrainType.Water.Id );
			} else {
				tile.SetTerrainType( TerrainType.ShallowWater.Id );
			}
			return false;
		}
		landscapeData = new( tile, rng, height );
		return true;
	}
}
