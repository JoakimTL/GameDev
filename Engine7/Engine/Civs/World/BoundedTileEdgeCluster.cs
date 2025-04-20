using Engine;

namespace Civs.World;

public sealed class BoundedTileEdgeCluster( AABB<Vector3<float>> bounds, IReadOnlyList<Tile> tiles, IReadOnlyList<Edge> edges ) {
	public AABB<Vector3<float>> Bounds { get; } = bounds;
	public IReadOnlyList<Tile> Tiles { get; } = tiles;
	public IReadOnlyList<Edge> Edges { get; } = edges;
}
