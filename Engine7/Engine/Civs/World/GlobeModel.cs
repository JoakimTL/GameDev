using Civs.World.Generation;
using Civs.World.TerrainTypes;
using Engine;
using Engine.Standard.Render.Meshing;
using Engine.Structures;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public sealed class GlobeModel : DisposableIdentifiable {
	private readonly UnmanagedStaticContainer<PackedNormal> _vectorContainer;
	private readonly Face[] _faces;

	private readonly List<BoundedRenderCluster> _clusters;

	public GlobeModel( Guid globeId, uint subdivisions, IWorldTerrainGenerator generator ) {
		Id = globeId;
		Subdivisions = subdivisions;

		Icosphere icosphere = new( subdivisions );
		_vectorContainer = new( [ .. icosphere.Vertices.Select( p => new PackedNormal( p ) ) ] );

		OcTree<Face, float> faceTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
		OcTree<Edge, float> edgeTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );

		var indices = icosphere.GetIndices();
		_faces = new Face[ indices.Count / 3 ];
		Dictionary<EdgeIndices, Connection> connections = [];
		Dictionary<uint, GlobeVertex> vertices = [];
		for (uint i = 0; i < _vectorContainer.Count; i++)
			vertices[ i ] = new( _vectorContainer, i );
		int id = 0;
		Span<EdgeIndices> currentEdges = stackalloc EdgeIndices[ 3 ];
		for (int i = 0; i < indices.Count; i += 3) {
			var faceIndices = new FaceIndices( indices[ i ], indices[ i + 1 ], indices[ i + 2 ] );
			GlobeVertex[] faceVertices = [ vertices[ faceIndices.VertexA ], vertices[ faceIndices.VertexB ], vertices[ faceIndices.VertexC ] ];
			Face face = new( faceVertices, (uint) id );
			_faces[ id ] = face;
			faceTree.Add( face );
			id++;

			currentEdges[ 0 ] = new EdgeIndices( faceIndices.VertexA, faceIndices.VertexB );
			currentEdges[ 1 ] = new EdgeIndices( faceIndices.VertexB, faceIndices.VertexC );
			currentEdges[ 2 ] = new EdgeIndices( faceIndices.VertexC, faceIndices.VertexA );

			for (int j = 0; j < currentEdges.Length; j++) {
				if (connections.TryGetValue( currentEdges[ j ], out var connection )) {
					connection.SetFaceB( face );
					face.Blueprint.AddConnection( connection );
					continue;
				}
				Edge edge = new( _vectorContainer, currentEdges[ j ] );
				edgeTree.Add( edge );
				connection = new Connection( face, edge );
				connections[ currentEdges[ j ] ] = connection;
				edge.SetConnection( connection );
				face.Blueprint.AddConnection( connection );
			}
		}

		List<IReadOnlyBranch<Face, float>> faceBranches = [ .. faceTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
		List<IReadOnlyBranch<Edge, float>> edgeBranches = [ .. edgeTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
		_clusters = new List<BoundedRenderCluster>( faceBranches.Count );
		foreach ((AABB<Vector3<float>> bounds, IReadOnlyBranch<Face, float> faces, IReadOnlyBranch<Edge, float>? edges) pair in BoundedRenderCluster.CreateClusterPairs( faceBranches, edgeBranches ))
			_clusters.Add( new BoundedRenderCluster( (uint) _clusters.Count, pair.bounds, [ .. pair.faces.Contents ], pair.edges?.Contents.ToList() ?? [] ) );

		if (_clusters.Count > 0)
			ClusterBounds = Vector3<float>.Zero.CreateBounds( _clusters[ 0 ].Bounds.GetLengths() * 0.5f );

		generator.GenerateTerrain( this );

		foreach (Face face in _faces)
			face.SetTransmission( true );
	}

	public Guid Id { get; }
	public uint Subdivisions { get; }
	public uint VertexCount => _vectorContainer.Count;
	public uint FaceCount => (uint) _faces.Length;

	public IReadOnlyList<Face> Faces => _faces;

	public IReadOnlyList<BoundedRenderCluster> Clusters => _clusters;

	public AABB<Vector3<float>> ClusterBounds { get; }

	protected override bool InternalDispose() {
		_vectorContainer.Dispose();
		return true;
	}
}


public interface IWorldTerrainGenerator {
	void GenerateTerrain( GlobeModel globe );
}

public sealed class ProceduralWorldTerrainGenerator : IWorldTerrainGenerator {

	public ProceduralWorldTerrainGenerator( WorldGenerationParameters parameters ) {
		this.Parameters = parameters;
	}

	public WorldGenerationParameters Parameters { get; }

	public void GenerateTerrain( GlobeModel globe ) {
		GenerateLandmass( Parameters.GenerationSeed, Parameters.MaxTerrainHeight, Parameters.Sealevel, globe );
	}

	private void GenerateLandmass( int seed, double maxTerrainHeight, double sealevel, GlobeModel globe ) {
		double maxSeaDepth = sealevel;
		double maxLandHeight = maxTerrainHeight - sealevel;

		Random seedProvider = new( seed );
		Noise3 coarseHeightNoise = new( seedProvider.Next(), 5 );
		Noise3 fineHeightNoise = new( seedProvider.Next(), 23 );
		Noise3 coarseHeightNoise2 = new( seedProvider.Next(), 3 );
		Noise3 fineHeightNoise2 = new( seedProvider.Next(), 17 );

		Noise3 landLargeNoise = new( seedProvider.Next(), 2 );
		Noise3 landFineNoise = new( seedProvider.Next(), 13 );
		Noise3 landTranslationStrengthNoise = new( seedProvider.Next(), 7 );

		FiniteVoronoiNoise3 voronoiTranslation = new( new( seedProvider.Next() ), 0.5f, 1 );
		FiniteVoronoiNoise3 voronoiRidgeNoise = new( new( seedProvider.Next() ), 0.25f, 1 );
		FiniteVoronoiNoise3 voronoiRidgeNoiseFine = new( new( seedProvider.Next() ), 0.0625f, 1 );

		var grasslandTerrain = TerrainTypeList.GetTerrainType<GrasslandTerrain>();
		var oceanTerrain = TerrainTypeList.GetTerrainType<OceanTerrain>();

		for (uint i = 0; i < globe.FaceCount; i++) {
			//https://www.youtube.com/watch?v=WumyfLEa6bU
			//https://iquilezles.org/articles/morenoise/
			var face = globe.Faces[ (int) i ];
			Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;

			Vector3<float> translation = voronoiTranslation.NoiseVector( center );
			Vector3<float> smallTranslation = translation * 0.05f;

			float lTs = landTranslationStrengthNoise.Noise( center );
			float landmassN = landLargeNoise.Noise( center + smallTranslation * lTs ) * 0.85f + landFineNoise.Noise( center + translation * lTs ) * 0.15f;
			landmassN = float.Sqrt( landmassN );
			bool isLand = landmassN > 0.73f;

			float n = (coarseHeightNoise.Noise( center ) * 0.9f + fineHeightNoise.Noise( center ) * 0.1f) * (coarseHeightNoise2.Noise( center ) * 0.7f + fineHeightNoise2.Noise( center ) * 0.3f);
			float rN = voronoiRidgeNoise.Noise( center ) * 0.85f + voronoiRidgeNoiseFine.Noise( center ) * 0.15f;
			n *= rN;

			float height = 0;
			if (isLand)
				height = (float) (n * landmassN * maxLandHeight);
			else
				height = (float) (-n * landmassN * maxSeaDepth);
			//face.State.SetHeight( height );
			face.State.SetTerrainType( height >= 0 ? grasslandTerrain : oceanTerrain );


		}
	}
}