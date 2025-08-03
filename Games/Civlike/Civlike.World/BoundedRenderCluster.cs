using Engine;
using Engine.Structures;

namespace Civlike.World;

public sealed class BoundedRenderCluster {

	private BoundedRenderCluster( AABB<Vector3<float>> bounds, IReadOnlyList<ReadOnlyFace> faces, IReadOnlyList<ReadOnlyEdge> edges ) {
		this.Bounds = bounds;
		this.Faces = faces;
		this.Edges = edges;
	}

	public AABB<Vector3<float>> Bounds { get; }
	public IReadOnlyList<ReadOnlyFace> Faces { get; }
	public IReadOnlyList<ReadOnlyEdge> Edges { get; }

	public static List<(AABB<Vector3<float>>, IReadOnlyBranch<ReadOnlyFace, float>, IReadOnlyBranch<ReadOnlyEdge, float>?)> CreateClusterPairs( IReadOnlyList<IReadOnlyBranch<ReadOnlyFace, float>> faceClusters, IReadOnlyList<IReadOnlyBranch<ReadOnlyEdge, float>> edgeClusters ) {
		List<(AABB<Vector3<float>>, IReadOnlyBranch<ReadOnlyFace, float>, IReadOnlyBranch<ReadOnlyEdge, float>?)> result = [];

		foreach (IReadOnlyBranch<ReadOnlyFace, float> faceCluster in faceClusters) {
			int pairs = result.Count;
			foreach (IReadOnlyBranch<ReadOnlyEdge, float> edgeCluster in edgeClusters)
				if (faceCluster.BranchBounds == edgeCluster.BranchBounds)
					result.Add( (faceCluster.BranchBounds, faceCluster, edgeCluster) );
			if (result.Count == pairs)
				result.Add( (faceCluster.BranchBounds, faceCluster, null) );
		}

		return result;
	}

	internal sealed class Builder( AABB<Vector3<float>> bounds ) {
		public List<ReadOnlyFace> Faces { get; } = [];
		public List<ReadOnlyEdge> Edges { get; } = [];
		public AABB<Vector3<float>> Bounds { get; } = bounds;

		public bool HasFaces => this.Faces.Count > 0;

		public BoundedRenderCluster Build() {
			return new BoundedRenderCluster( this.Bounds, this.Faces, this.Edges );
		}

	}
}

public sealed class BoundedTileCluster {
	public BoundedTileCluster( BoundedRenderCluster cluster, IReadOnlyList<Tile> tiles ) {
		this.Cluster = cluster;
		this.Tiles = tiles;
	}

	public BoundedRenderCluster Cluster { get; }
	public IReadOnlyList<Tile> Tiles { get; }
}