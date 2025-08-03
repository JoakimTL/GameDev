//using Engine.Structures;
//using Engine;

//namespace OldGen.World.GameplayState;

//public sealed class BoundedRenderCluster {

//	public event Action? FaceStateChangedInCluster;

//	private BoundedRenderCluster( uint id, AABB<Vector3<float>> bounds, IReadOnlyList<Face> faces, IReadOnlyList<Edge> edges ) {
//		this.Id = id;
//		this.Bounds = bounds;
//		this.Faces = faces;
//		this.Edges = edges;
//		foreach (Face face in faces)
//			face.FaceStateChanged += OnFaceStateChanged;
//	}

//	private void OnFaceStateChanged( Face face ) => FaceStateChangedInCluster?.Invoke();

//	public uint Id { get; }
//	public AABB<Vector3<float>> Bounds { get; }
//	public IReadOnlyList<Face> Faces { get; }
//	public IReadOnlyList<Edge> Edges { get; }


//	public static List<(AABB<Vector3<float>>, IReadOnlyBranch<Face, float>, IReadOnlyBranch<Edge, float>?)> CreateClusterPairs( IReadOnlyList<IReadOnlyBranch<Face, float>> faceClusters, IReadOnlyList<IReadOnlyBranch<Edge, float>> edgeClusters ) {
//		List<(AABB<Vector3<float>>, IReadOnlyBranch<Face, float>, IReadOnlyBranch<Edge, float>?)> result = [];

//		foreach (IReadOnlyBranch<Face, float> faceCluster in faceClusters) {
//			int pairs = result.Count;
//			foreach (IReadOnlyBranch<Edge, float> edgeCluster in edgeClusters)
//				if (faceCluster.BranchBounds == edgeCluster.BranchBounds)
//					result.Add( (faceCluster.BranchBounds, faceCluster, edgeCluster) );
//			if (result.Count == pairs)
//				result.Add( (faceCluster.BranchBounds, faceCluster, null) );
//		}

//		return result;
//	}

//	public sealed class Builder( AABB<Vector3<float>> bounds ) {
//		public List<Face> Faces { get; } = [];
//		public List<Edge> Edges { get; } = [];
//		public AABB<Vector3<float>> Bounds { get; } = bounds;

//		public bool HasFaces => this.Faces.Count > 0;

//		public BoundedRenderCluster Build( uint id ) {
//			return new BoundedRenderCluster( id, this.Bounds, this.Faces, this.Edges );
//		}

//	}
//}