using Civlike.World.Generation;
using Civlike.World.TerrainTypes;
using Engine;
using Engine.Generation.Meshing;
using Engine.Structures;
using System;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.World;

public sealed class GlobeModel : DisposableIdentifiable {
	private readonly GlobeVertex[] _vertices;
	private readonly Face[] _faces;

	private readonly List<BoundedRenderCluster> _clusters;

	public GlobeModel( Guid globeId, WorldGenerationParameters parameters, IWorldTerrainGenerator generator ) {
		Id = globeId;
		this.Parameters = parameters;
		GlobeArea = 4 * double.Pi * parameters.GlobeRadius * parameters.GlobeRadius;

		Icosphere icosphere = new( parameters.Subdivisions );
		_vertices = [ .. icosphere.Vertices.Select( p => new GlobeVertex( new PackedNormal( p ) ) ) ];

		OcTree<Face, float> faceTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
		OcTree<Edge, float> edgeTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );

		var indices = icosphere.GetIndices();
		_faces = new Face[ indices.Count / 3 ];
		TileArea = GlobeArea / _faces.Length;
		ApproximateTileLength = Math.Sqrt( TileArea * 2 ) / 2f;
		Dictionary<EdgeIndices, Connection> connections = [];
		int id = 0;
		Span<EdgeIndices> currentEdges = stackalloc EdgeIndices[ 3 ];
		for (int i = 0; i < indices.Count; i += 3) {
			var faceIndices = new FaceIndices( indices[ i ], indices[ i + 1 ], indices[ i + 2 ] );
			GlobeVertex[] faceVertices = [ _vertices[ faceIndices.VertexA ], _vertices[ faceIndices.VertexB ], _vertices[ faceIndices.VertexC ] ];
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
				Edge edge = new( _vertices[ currentEdges[ j ].VertexA ], _vertices[ currentEdges[ j ].VertexB ] );
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
	public WorldGenerationParameters Parameters { get; }
	public double GlobeArea { get; }
	public double TileArea { get; }
	public double ApproximateTileLength { get; }
	public uint VertexCount => (uint) _vertices.Length;
	public uint FaceCount => (uint) _faces.Length;

	public IReadOnlyList<Face> Faces => _faces;

	public IReadOnlyList<BoundedRenderCluster> Clusters => _clusters;

	public AABB<Vector3<float>> ClusterBounds { get; }

	protected override bool InternalDispose() {
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
		GenerateLandmass( Parameters.GenerationSeed, 27500, Parameters.Sealevel, globe );
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
		var mountainTerrain = TerrainTypeList.GetTerrainType<MountainTerrain>();
		var shorelineTerrain = TerrainTypeList.GetTerrainType<ShorelineTerrain>();

		for (uint i = 0; i < globe.FaceCount; i++) {
			//https://www.youtube.com/watch?v=WumyfLEa6bU
			//https://iquilezles.org/articles/morenoise/
			var face = globe.Faces[ (int) i ];
			Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;

			Vector3<float> translationVector = voronoiTranslation.NoiseVector( center );
			Vector3<float> translation = center + translationVector * (landTranslationStrengthNoise.Noise( center ) * 0.1f);

			float ridgeN = voronoiRidgeNoise.Noise( translation ) * 0.65f + voronoiRidgeNoiseFine.Noise( translation ) * 0.35f;
			float n = landLargeNoise.Noise( center ) * 0.85f + landFineNoise.Noise( center ) * 0.15f;
			n += ridgeN;
			n *= 0.5f;
			n *= n;


			//n *= (coarseHeightNoise.Noise( center ) * 0.9f + fineHeightNoise.Noise( center ) * 0.1f) * (coarseHeightNoise2.Noise( center ) * 0.7f + fineHeightNoise2.Noise( center ) * 0.3f);
			//float rN = voronoiRidgeNoise.Noise( center ) * 0.85f + voronoiRidgeNoiseFine.Noise( center ) * 0.15f;
			//n *= rN;

			float height = (float) (n * maxTerrainHeight - maxSeaDepth);

			//face.State.SetHeight( height );
			face.State.SetTerrainType( height >= 0 ? (height > 1200 ? mountainTerrain : grasslandTerrain) : (height < -200 ? oceanTerrain : shorelineTerrain) );


		}
	}
}

public sealed record WorldGenerationProgressMessage( string ProgressMessage );

public readonly struct Temperature {
	public readonly float Kelvin;

	public Temperature( float kelvin ) {
		Kelvin = kelvin;
	}

	public float Celsius => Kelvin - 273.15f;
	public float Fahrenheit => Celsius * 9 / 5 + 32;

	public override int GetHashCode() => Kelvin.GetHashCode();
	public override bool Equals( object? obj ) => obj is Temperature temperature && temperature == this;
	public override string ToString() => $"{Celsius} °C";

	public static Temperature FromCelsius( float celsius ) => new( celsius + 273.15f );
	public static Temperature FromFahrenheit( float fahrenheit ) => new( (fahrenheit - 32) * 5 / 9 + 273.15f );

	public static explicit operator Temperature( float kelvin ) => new( kelvin );
	public static explicit operator Temperature( double kelvin ) => new( (float) kelvin );
	public static explicit operator float( Temperature temperature ) => temperature.Kelvin;
	public static bool operator ==( Temperature left, Temperature right ) => left.Kelvin == right.Kelvin;
	public static bool operator !=( Temperature left, Temperature right ) => !(left == right);
}

public readonly struct Pressure {
	public readonly float Pascal;
	public Pressure( float pascal ) {
		Pascal = pascal;
	}
	public float Bar => Pascal / 100000f;
	public float Atmosphere => Pascal / 101325f;
	public override int GetHashCode() => Pascal.GetHashCode();
	public override bool Equals( object? obj ) => obj is Pressure pressure && pressure == this;
	public override string ToString() => $"{Bar} bar";
	public static Pressure FromBar( float bar ) => new( bar * 100000f );
	public static Pressure FromAtmosphere( float atmosphere ) => new( atmosphere * 101325f );
	public static explicit operator Pressure( float pascal ) => new( pascal );
	public static explicit operator Pressure( double pascal ) => new( (float) pascal );
	public static explicit operator float( Pressure pressure ) => pressure.Pascal;
	public static bool operator ==( Pressure left, Pressure right ) => left.Pascal == right.Pascal;
	public static bool operator !=( Pressure left, Pressure right ) => !(left == right);
}