namespace Sandbox.Logic.Old.OldWorld.Tiles;

public static class TileExtensions {
	public static Vector3<float> GetCenter( this ITile tile ) {
		return (tile.VectorA + tile.VectorB + tile.VectorC) / 3;
	}

	public static void FillSpan( this ITile tile, Span<Vector3<float>> span ) {
		if (span.Length != 3)
			throw new ArgumentException( "Span must have a length of 3", nameof( span ) );
		span[ 0 ] = tile.VectorA;
		span[ 1 ] = tile.VectorB;
		span[ 2 ] = tile.VectorC;
	}

	public static void FillSpan( this ITile tile, Span<int> span ) {
		if (span.Length != 3)
			throw new ArgumentException( "Span must have a length of 3", nameof( span ) );
		span[ 0 ] = tile.IndexA;
		span[ 1 ] = tile.IndexB;
		span[ 2 ] = tile.IndexC;
	}

	public static IReadOnlyList<Tile> GetAllTiles( this IContainingTile containingTile ) {
		List<Tile> tiles = [];
		foreach (ITile subTile in containingTile.SubTiles) {
			if (subTile is Tile tile) {
				tiles.Add( tile );
				continue;
			}
			if (subTile is IContainingTile compositeSubTile)
				tiles.AddRange( compositeSubTile.GetAllTiles() );
		}
		return tiles;
	}
	public static IReadOnlyList<Region> GetAllRegions( this IContainingTile containingTile ) {
		List<Region> regions = [];
		foreach (ITile subTile in containingTile.SubTiles) {
			if (subTile is Region region) {
				regions.Add( region );
				continue;
			}
			if (subTile is IContainingTile compositeSubTile)
				regions.AddRange( compositeSubTile.GetAllRegions() );
		}
		return regions;
	}
}