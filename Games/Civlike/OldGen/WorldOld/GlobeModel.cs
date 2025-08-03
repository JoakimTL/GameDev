using Civlike.Messages;
using Engine;
using Engine.Generation.Meshing;
using Engine.Modularity;
using Engine.Structures;

//TODO have a generationglobe and a usable globe. The separation between what is supposed to be used during gameplay and what is supposed to be only used during generation needs to be separated. The globe during generation has a lot more information per tile than during gameplay, at least initially

namespace OldGen.WorldOld;

public sealed class GlobeModel : DisposableIdentifiable {
	private readonly GlobeVertex[] _vertices;
	private readonly Face[] _faces;

	private readonly List<BoundedRenderCluster> _clusters;

	public GlobeModel( Guid globeId, WorldGenerationParameters parameters, IWorldTerrainGenerator generator ) {
		this.Id = globeId;
		this.Parameters = parameters;
		this.GlobeArea = 4 * double.Pi * parameters.GlobeRadius * parameters.GlobeRadius;

		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Generating globe icosphere" ) );
		Icosphere icosphere = new( parameters.Subdivisions );
		this._vertices = [ .. icosphere.Vertices.Select( p => new GlobeVertex( new PackedNormal( p ) ) ) ];

		OcTree<Face, float> faceTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
		OcTree<Edge, float> edgeTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );


		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Generating globe faces" ) );
		IReadOnlyList<uint> indices = icosphere.GetIndices();
		this._faces = new Face[ indices.Count / 3 ];
		this.TileArea = this.GlobeArea / this._faces.Length;
		this.ApproximateTileLength = parameters.GlobeRadius * Math.Sin( Math.PI / (10 * Math.Pow( 2, parameters.Subdivisions )) );
		Dictionary<EdgeIndices, Connection> connections = [];
		int id = 0;
		Span<EdgeIndices> currentEdges = stackalloc EdgeIndices[ 3 ];
		for (int i = 0; i < indices.Count; i += 3) {
			FaceIndices faceIndices = new( indices[ i ], indices[ i + 1 ], indices[ i + 2 ] );
			GlobeVertex[] faceVertices = [ this._vertices[ faceIndices.VertexA ], this._vertices[ faceIndices.VertexB ], this._vertices[ faceIndices.VertexC ] ];
			Face face = new( this, faceVertices, (uint) id );
			this._faces[ id ] = face;
			faceTree.Add( face );
			id++;

			currentEdges[ 0 ] = new EdgeIndices( faceIndices.VertexA, faceIndices.VertexB );
			currentEdges[ 1 ] = new EdgeIndices( faceIndices.VertexB, faceIndices.VertexC );
			currentEdges[ 2 ] = new EdgeIndices( faceIndices.VertexC, faceIndices.VertexA );

			for (int j = 0; j < currentEdges.Length; j++) {
				if (connections.TryGetValue( currentEdges[ j ], out Connection? connection )) {
					connection.SetFaceB( face );
					face.Blueprint.AddConnection( connection );
					continue;
				}
				Edge edge = new( this._vertices[ currentEdges[ j ].VertexA ], this._vertices[ currentEdges[ j ].VertexB ] );
				edgeTree.Add( edge );
				connection = new Connection( face, edge );
				connections[ currentEdges[ j ] ] = connection;
				edge.SetConnection( connection );
				face.Blueprint.AddConnection( connection );
			}
		}

		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Creating render clusters" ) );

		List<IReadOnlyBranch<Face, float>> faceBranches = [ .. faceTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
		List<IReadOnlyBranch<Edge, float>> edgeBranches = [ .. edgeTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
		this._clusters = new List<BoundedRenderCluster>( faceBranches.Count );
		foreach ((AABB<Vector3<float>> bounds, IReadOnlyBranch<Face, float> faces, IReadOnlyBranch<Edge, float>? edges) pair in BoundedRenderCluster.CreateClusterPairs( faceBranches, edgeBranches ))
			this._clusters.Add( new BoundedRenderCluster( (uint) this._clusters.Count, pair.bounds, [ .. pair.faces.Contents ], pair.edges?.Contents.ToList() ?? [] ) );

		if (this._clusters.Count > 0)
			this.ClusterBounds = Vector3<float>.Zero.CreateBounds( this._clusters[ 0 ].Bounds.GetLengths() * 0.5f );

		generator.GenerateTerrain( this.Parameters, this );

		foreach (Face face in this._faces)
			face.SetTransmission( true );
	}

	public Guid Id { get; }
	public WorldGenerationParameters Parameters { get; }
	public double GlobeArea { get; }
	public double TileArea { get; }
	public double ApproximateTileLength { get; }
	public uint VertexCount => (uint) this._vertices.Length;
	public uint FaceCount => (uint) this._faces.Length;

	public IReadOnlyList<Face> Faces => this._faces;

	public IReadOnlyList<BoundedRenderCluster> Clusters => this._clusters;

	public AABB<Vector3<float>> ClusterBounds { get; }

	protected override bool InternalDispose() {
		return true;
	}
}
