using Engine;
using Engine.Structures;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World.NewWorld;

public sealed class BoundedRenderCluster {

	public event Action? FaceStateChangedInCluster;

	public BoundedRenderCluster( uint id, AABB<Vector3<float>> bounds, IReadOnlyList<Face> faces, IReadOnlyList<Edge> edges ) {
		this.Id = id;
		this.Bounds = bounds;
		this.Faces = faces;
		this.Edges = edges;
		foreach (Face face in faces)
			face.FaceStateChanged += OnFaceStateChanged;
	}

	private void OnFaceStateChanged( Face face ) => FaceStateChangedInCluster?.Invoke();

	public uint Id { get; }
	public AABB<Vector3<float>> Bounds { get; }
	public IReadOnlyList<Face> Faces { get; }
	public IReadOnlyList<Edge> Edges { get; }


	public static List<(AABB<Vector3<float>>, IReadOnlyBranch<Face, float>, IReadOnlyBranch<Edge, float>?)> CreateClusterPairs( IReadOnlyList<IReadOnlyBranch<Face, float>> faceClusters, IReadOnlyList<IReadOnlyBranch<Edge, float>> edgeClusters ) {
		List<(AABB<Vector3<float>>, IReadOnlyBranch<Face, float>, IReadOnlyBranch<Edge, float>?)> result = [];

		foreach (IReadOnlyBranch<Face, float> faceCluster in faceClusters) {
			int pairs = result.Count;
			foreach (IReadOnlyBranch<Edge, float> edgeCluster in edgeClusters)
				if (faceCluster.BranchBounds == edgeCluster.BranchBounds)
					result.Add( (faceCluster.BranchBounds, faceCluster, edgeCluster) );
			if (result.Count == pairs)
				result.Add( (faceCluster.BranchBounds, faceCluster, null) );
		}

		return result;
	}
}
