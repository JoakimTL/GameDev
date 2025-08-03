using Engine;
using Engine.Generation.Meshing;

namespace Civlike.World;

/// <summary>
/// Contains the readonly deterministic data for a globe of a certain number of subdivisions.
/// </summary>
public sealed class ReadOnlyGlobe {

	public Guid Id { get; }
	public int Subdivisions { get; }

	private readonly List<ReadOnlyFace> _faces;
	private readonly List<ReadOnlyEdge> _edges;
	private readonly List<ReadOnlyVertex> _vertices;
	private readonly List<BoundedRenderCluster> _clusters;

	internal ReadOnlyGlobe( int subdivisions ) {
		if (this.Subdivisions < 0)
			throw new ArgumentOutOfRangeException( nameof( subdivisions ), "Subdivisions must be greater than or equal to 0." );
		this.Id = Guid.NewGuid();
		this.Subdivisions = subdivisions;
		this._faces = [];
		this._edges = [];
		this._vertices = [];
		this._clusters = [];
		this.Faces = this._faces.AsReadOnly();
		this.Edges = this._edges.AsReadOnly();
		this.Vertices = this._vertices.AsReadOnly();
		this.Clusters = this._clusters.AsReadOnly();
	}

	public IReadOnlyList<ReadOnlyFace> Faces { get; }
	public IReadOnlyList<ReadOnlyEdge> Edges { get; }
	public IReadOnlyList<ReadOnlyVertex> Vertices { get; }
	public IReadOnlyList<BoundedRenderCluster> Clusters { get; }
	public AABB<Vector3<float>> ClusterBounds { get; private set; }

	internal void Generate() {
		Icosphere icosphere = new( (uint) this.Subdivisions );

		ReadOnlyVertex.Builder[] vertexBuilders = new ReadOnlyVertex.Builder[ icosphere.Vertices.Count ];
		for (int i = 0; i < icosphere.Vertices.Count; i++)
			vertexBuilders[ i ] = new ReadOnlyVertex.Builder( i, new PackedNormal( icosphere.Vertices[ i ] ) );

		IReadOnlyList<uint> indices = icosphere.GetIndices();

		ReadOnlyFace.Builder[] faceBuilders = new ReadOnlyFace.Builder[ indices.Count / 3 ];
		ReadOnlyEdge.Builder[] edgeBuilders = new ReadOnlyEdge.Builder[ indices.Count / 2 ];
		Dictionary<EdgeIndices, ReadOnlyEdge.Builder> edgeLookup = [];
		Span<EdgeIndices> currentEdges = stackalloc EdgeIndices[ 3 ];
		int edgeId = 0;

		for (int i = 0; i < indices.Count; i += 3) {
			int faceId = i / 3;
			int indexA = (int) indices[ i ];
			int indexB = (int) indices[ i + 1 ];
			int indexC = (int) indices[ i + 2 ];
			ReadOnlyVertex.Builder vertexBuilderA = vertexBuilders[ indexA ];
			ReadOnlyVertex.Builder vertexBuilderB = vertexBuilders[ indexB ];
			ReadOnlyVertex.Builder vertexBuilderC = vertexBuilders[ indexC ];

			ReadOnlyFace.Builder faceBuilder = new( faceId );
			faceBuilder.Vertices.Add( vertexBuilderA.Vertex );
			faceBuilder.Vertices.Add( vertexBuilderB.Vertex );
			faceBuilder.Vertices.Add( vertexBuilderC.Vertex );
			faceBuilders[ faceId ] = faceBuilder;

			vertexBuilderA.ConnectedFaces.Add( faceBuilder.Face );
			vertexBuilderB.ConnectedFaces.Add( faceBuilder.Face );
			vertexBuilderC.ConnectedFaces.Add( faceBuilder.Face );

			currentEdges[ 0 ] = new EdgeIndices( indexA, indexB );
			currentEdges[ 1 ] = new EdgeIndices( indexB, indexC );
			currentEdges[ 2 ] = new EdgeIndices( indexC, indexA );

			for (int j = 0; j < currentEdges.Length; j++) {
				if (!edgeLookup.TryGetValue( currentEdges[ j ], out ReadOnlyEdge.Builder? edgeBuilder )) {
					edgeBuilder = new( edgeId, vertexBuilders[ currentEdges[ j ].VertexA ].Vertex, vertexBuilders[ currentEdges[ j ].VertexB ].Vertex, faceBuilder.Face );
					edgeLookup[ currentEdges[ j ] ] = edgeBuilder;
					edgeBuilders[ edgeId++ ] = edgeBuilder;
				} else {
					edgeBuilder.Complete( faceBuilder.Face );
				}
				faceBuilder.Edges.Add( edgeBuilder.Edge );
			}


		}

		foreach (ReadOnlyVertex.Builder builder in vertexBuilders)
			builder.Complete();
		foreach (ReadOnlyFace.Builder builder in faceBuilders)
			builder.Complete();

		this._vertices.AddRange( vertexBuilders.Select( b => b.Vertex ) );
		this._faces.AddRange( faceBuilders.Select( b => b.Face ) );
		this._edges.AddRange( edgeBuilders.Select( b => b.Edge ) );
		this._clusters.AddRange( GetRenderClusters( this._edges, this._faces ) );

		if (this._clusters.Count > 0)
			this.ClusterBounds = Vector3<float>.Zero.CreateBounds( this._clusters[ 0 ].Bounds.GetLengths() * 0.5f );
	}

	public static BoundedRenderCluster[] GetRenderClusters( List<ReadOnlyEdge> edges, List<ReadOnlyFace> faces ) {
		int clustersPerAxis = 16; // This can be adjusted based on the desired granularity of the clusters
		int totalClusters = clustersPerAxis * clustersPerAxis * clustersPerAxis;
		int GetIndex( Vector3<int> xyz ) => xyz.X + xyz.Y * clustersPerAxis + xyz.Z * clustersPerAxis * clustersPerAxis;
		Vector3<int> TurnIntoXyz( Vector3<float> vector ) => vector.Add( 1 ).ScalarMultiply( clustersPerAxis / 2 ).CastSaturating<float, int>();
		AABB<Vector3<float>> baseBounds = AABB.Create<Vector3<float>>( [ 0, 2f / clustersPerAxis ] );
		Vector3<float> length = baseBounds.GetLengths() * clustersPerAxis; // Assuming uniform length for simplicity

		BoundedRenderCluster.Builder[] clusterBuilders = new BoundedRenderCluster.Builder[ totalClusters ];
		for (int x = 0; x < clustersPerAxis; x++)
			for (int y = 0; y < clustersPerAxis; y++)
				for (int z = 0; z < clustersPerAxis; z++) {
					Vector3<float> offset = new Vector3<float>( x, y, z ) - clustersPerAxis / 2;
					clusterBuilders[ GetIndex( (x, y, z) ) ] = new BoundedRenderCluster.Builder( baseBounds.MoveBy( offset.ScalarDivide( clustersPerAxis / 2 ) ) );
				}
		foreach (ReadOnlyEdge edge in edges) {
			Vector3<float> center = (edge.VertexA.Vector + edge.VertexB.Vector) / 2;
			Vector3<int> xyz = TurnIntoXyz( center );
			int index = GetIndex( xyz );
			clusterBuilders[ index ].Edges.Add( edge );
		}
		foreach (ReadOnlyFace face in faces) {
			Vector3<float> center = face.GetCenter();
			Vector3<int> xyz = TurnIntoXyz( center );
			int index = GetIndex( xyz );
			clusterBuilders[ index ].Faces.Add( face );
		}

		List<BoundedRenderCluster> clusters = [];

		for (int i = 0; i < clusterBuilders.Length; i++)
			if (clusterBuilders[ i ].HasFaces)
				clusters.Add( clusterBuilders[ i ].Build() );

		return [ .. clusters ];
	}

	private readonly struct EdgeIndices : IEquatable<EdgeIndices> {
		public int VertexA { get; }
		public int VertexB { get; }

		public EdgeIndices( int a, int b ) {
			if (a == b)
				throw new ArgumentException( "Edge cannot connect a vertex to itself." );

			if (a < b) {
				this.VertexA = a;
				this.VertexB = b;
			} else {
				this.VertexA = b;
				this.VertexB = a;
			}
		}

		public override int GetHashCode() => HashCode.Combine( this.VertexA, this.VertexB );
		public override bool Equals( object? obj ) => obj is EdgeIndices edge && Equals( edge );
		public override string ToString() => $"({this.VertexA}, {this.VertexB})";
		public bool Equals( EdgeIndices other ) => this.VertexA == other.VertexA && this.VertexB == other.VertexB;
		public static bool operator ==( EdgeIndices left, EdgeIndices right ) => left.Equals( right );
		public static bool operator !=( EdgeIndices left, EdgeIndices right ) => !(left == right);
	}
}