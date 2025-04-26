using Engine;
using Engine.Standard.Render.Meshing;
using Engine.Structures;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public sealed class GlobeBlueprintModel : DisposableIdentifiable {
	private readonly UnmanagedStaticContainer<Vector3<float>> _vectorContainer;
	private readonly UnmanagedStaticContainer<FaceRenderModel> _faceRenderModelContainer;
	private readonly UnmanagedStaticContainer<EdgeRenderModel> _edgeRenderModelContainer;

	public GlobeBlueprintModel( Guid globeId, uint subdivisions ) {
		Id = globeId;
		Subdivisions = subdivisions;

		Icosphere icosphere = new( subdivisions );
		_vectorContainer = new( icosphere.Vertices );

		GenerateEdgesAndFaces( icosphere, out var faces, out var edges );

		_faceRenderModelContainer = new( faces );
		_edgeRenderModelContainer = new( edges );

		Clusters = CreateRenderClusters();

		ClusterBounds = Vector3<float>.Zero.CreateBounds( Clusters.First().Bounds.GetLengths() * 0.5f );
	}

	public Guid Id { get; }
	public uint Subdivisions { get; }
	public uint VertexCount => _vectorContainer.Count;
	public uint FaceCount => _faceRenderModelContainer.Count;
	public uint EdgeCount => _edgeRenderModelContainer.Count;
	public IReadOnlyList<BoundedRenderCluster> Clusters { get; }
	public AABB<Vector3<float>> ClusterBounds { get; }

	private IReadOnlyList<BoundedRenderCluster> CreateRenderClusters() {
		OcTree<FaceRenderModelLeaf, float> faceTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
		OcTree<EdgeRenderModelLeaf, float> edgeTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );

		for (uint i = 0; i < _faceRenderModelContainer.Count; i++)
			faceTree.Add( new( _vectorContainer, GetFace( i ) ) );

		for (uint i = 0; i < _edgeRenderModelContainer.Count; i++)
			edgeTree.Add( new( _vectorContainer, GetEdge( i ) ) );

		List<IReadOnlyBranch<FaceRenderModelLeaf, float>> faceBranches = [ .. faceTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
		List<IReadOnlyBranch<EdgeRenderModelLeaf, float>> edgeBranches = [ .. edgeTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];

		List<(AABB<Vector3<float>>, IReadOnlyBranch<FaceRenderModelLeaf, float>, IReadOnlyBranch<EdgeRenderModelLeaf, float>)> pairs = PairClusters( faceBranches, edgeBranches );

		List<BoundedRenderCluster> clusters = [];
		foreach ((AABB<Vector3<float>> bounds, IReadOnlyBranch<FaceRenderModelLeaf, float> faces, IReadOnlyBranch<EdgeRenderModelLeaf, float>? edges) pair in pairs)
			clusters.Add( new( (uint) clusters.Count, pair.bounds, [ .. pair.faces.Contents.Select( p => p.Id ) ], pair.edges is not null ? [ .. pair.edges.Contents.Select( p => p.Id ) ] : [] ) );
		return clusters.AsReadOnly();
	}

	private List<(AABB<Vector3<float>>, IReadOnlyBranch<FaceRenderModelLeaf, float>, IReadOnlyBranch<EdgeRenderModelLeaf, float>)> PairClusters( IReadOnlyList<IReadOnlyBranch<FaceRenderModelLeaf, float>> faceClusters, IReadOnlyList<IReadOnlyBranch<EdgeRenderModelLeaf, float>> edgeClusters ) {
		List<(AABB<Vector3<float>>, IReadOnlyBranch<FaceRenderModelLeaf, float>, IReadOnlyBranch<EdgeRenderModelLeaf, float>)> result = [];
		foreach (IReadOnlyBranch<FaceRenderModelLeaf, float> faceCluster in faceClusters) {
			int pairs = result.Count;
			foreach (IReadOnlyBranch<EdgeRenderModelLeaf, float> edgeCluster in edgeClusters)
				if (faceCluster.BranchBounds == edgeCluster.BranchBounds)
					result.Add( (faceCluster.BranchBounds, faceCluster, edgeCluster) );
			if (result.Count == pairs)
				result.Add( (faceCluster.BranchBounds, faceCluster, null) );
		}
		return result;
	}

	private void GenerateEdgesAndFaces( Icosphere icosphere, out IReadOnlyList<FaceRenderModel> faces, out IReadOnlyList<EdgeRenderModel> edges ) {
		IReadOnlyList<uint> indices = icosphere.GetIndices();
		List<FaceRenderModel> faceList = [];
		List<EdgeRenderModel> edgeList = [];
		HashSet<EdgeIndices> edgeSet = [];
		Span<EdgeIndices> edgeIndicesSpan = stackalloc EdgeIndices[ 3 ];
		for (int i = 0; i < indices.Count; i += 3) {
			FaceRenderModel face = new( indices[ i ], indices[ i + 1 ], indices[ i + 2 ] );
			faceList.Add( face );

			edgeIndicesSpan[ 0 ] = new( face.IndexA, face.IndexB );
			edgeIndicesSpan[ 1 ] = new( face.IndexB, face.IndexC );
			edgeIndicesSpan[ 2 ] = new( face.IndexC, face.IndexA );

			for (int j = 0; j < edgeIndicesSpan.Length; j++) {
				if (!edgeSet.Add( edgeIndicesSpan[ j ] ))
					continue;
				edgeList.Add( new EdgeRenderModel( edgeIndicesSpan[ j ].A, edgeIndicesSpan[ j ].B ) );
			}
		}

		faces = faceList;
		edges = edgeList;
	}

	public Vector3<float> GetVertex( uint index ) => _vectorContainer.Get( index );

	public FaceRenderModelWithId GetFace( uint index ) => new( index, _faceRenderModelContainer.Get( index ) );

	public FaceRenderModelWithIdAndVertices GetFaceWithVertices( uint index ) {
		FaceRenderModel face = _faceRenderModelContainer.Get( index );
		return new( index, face, GetVertex( face.IndexA ), GetVertex( face.IndexB ), GetVertex( face.IndexC ) );
	}

	public EdgeRenderModelWithId GetEdge( uint index ) => new( index, _edgeRenderModelContainer.Get( index ) );

	public EdgeRenderModelWithIdAndVertices GetEdgeWithVertices( uint index ) {
		EdgeRenderModel edge = _edgeRenderModelContainer.Get( index );
		return new( index, edge, GetVertex( edge.IndexA ), GetVertex( edge.IndexB ) );
	}

	protected override bool InternalDispose() {
		_vectorContainer.Dispose();
		_faceRenderModelContainer.Dispose();
		_edgeRenderModelContainer.Dispose();
		return true;
	}
}
