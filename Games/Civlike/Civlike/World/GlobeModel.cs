using Civlike.World.Generation;
using Civlike.World.TerrainTypes;
using Engine;
using Engine.Generation.Meshing;
using Engine.Structures;
using System.Drawing;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.World;

public sealed class GlobeModel : DisposableIdentifiable {
	private readonly GlobeVertex[] _vertices;
	private readonly Face[] _faces;

	private readonly List<BoundedRenderCluster> _clusters;

	public GlobeModel( Guid globeId, uint subdivisions, IWorldTerrainGenerator generator ) {
		Id = globeId;
		Subdivisions = subdivisions;

		Icosphere icosphere = new( subdivisions );
		_vertices = [ .. icosphere.Vertices.Select( p => new GlobeVertex( new PackedNormal( p ) ) ) ];

		OcTree<Face, float> faceTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
		OcTree<Edge, float> edgeTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );

		var indices = icosphere.GetIndices();
		_faces = new Face[ indices.Count / 3 ];
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
	public uint Subdivisions { get; }
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


public sealed class TectonicWorldTerrainGenerator : IWorldTerrainGenerator {

	public TectonicWorldTerrainGenerator( WorldGenerationParameters parameters ) {
		this.Parameters = parameters;
	}

	public WorldGenerationParameters Parameters { get; }

	public void GenerateTerrain( GlobeModel globe ) {
		GenerateLandmass( globe );
		GenerateWinds( globe );
	}

	private void GenerateLandmass( GlobeModel globe ) {
		Random seedProvider = new( Parameters.GenerationSeed );
		TectonicPlateGenerator tectonicRegionGenerator = new( new( seedProvider.Next() ), seedProvider.Next( 65, 90 ), 0.01f, (float) (Parameters.PlateHeight - Parameters.PlateHeightVariance), (float) (Parameters.PlateHeight + Parameters.PlateHeightVariance) );
		Noise3 xShiftNoise = new( seedProvider.Next(), 11 );
		Noise3 yShiftNoise = new( seedProvider.Next(), 11 );
		Noise3 zShiftNoise = new( seedProvider.Next(), 11 );

		uint baseCoarseSeed = unchecked((uint) seedProvider.Next());
		float baseCoarseScale = 17;
		uint baseFineSeed = unchecked((uint) seedProvider.Next());
		float baseFineScale = 34;

		Noise3 coarseFaultNoise = new( seedProvider.Next(), 7 );
		Noise3 fineFaultNoise = new( seedProvider.Next(), 27 );

		Noise3 mountainExistingNoise = new( seedProvider.Next(), 6 );
		FiniteVoronoiNoise3 mountainRidgeNoise = new( new( seedProvider.Next() ), 0.125f, 1 );

		var grasslandTerrain = TerrainTypeList.GetTerrainType<GrasslandTerrain>();
		var oceanTerrain = TerrainTypeList.GetTerrainType<OceanTerrain>();
		var mountainTerrain = TerrainTypeList.GetTerrainType<MountainTerrain>();
		var shorelineTerrain = TerrainTypeList.GetTerrainType<ShorelineTerrain>();

		List<(TectonicPlate plate, float gradient)> neighbours = [];

		for (uint i = 0; i < globe.FaceCount; i++) {
			var face = globe.Faces[ (int) i ];
			Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;

			var shift = new Vector3<float>( xShiftNoise.Noise( center ), yShiftNoise.Noise( center ), zShiftNoise.Noise( center ) ) * 2 - 1;
			var translation = center + shift * 0.05f;

			TectonicPlate current = tectonicRegionGenerator.Get( translation, neighbours, 23, 0.0001f );
			float currentHeight = current.Height + ((Noise3.Noise( baseCoarseSeed + (uint) current.Id, baseCoarseScale, center ) * 0.7f + Noise3.Noise( baseFineSeed + (uint) current.Id, baseFineScale, center ) * 0.3f) * 2 - 1) * (float) Parameters.BaseHeightVariance;

			float otherAverageHeight = 0;
			float faultHeight = 0;
			float faultIntensity = 0;
			for (int n = 0; n < neighbours.Count; n++) {
				(TectonicPlate other, float gradient) = neighbours[ n ];
				//float gradientSq = gradient * gradient;

				var otherVariableHeight = ((Noise3.Noise( baseCoarseSeed + (uint) other.Id, baseCoarseScale, center ) * 0.7f + Noise3.Noise( baseFineSeed + (uint) other.Id, baseFineScale, center ) * 0.3f) * 2 - 1) * (float) Parameters.BaseHeightVariance;
				otherAverageHeight += (other.Height + otherVariableHeight) * gradient;

				float faultMovement = current.GetFaultMovementDifference( other );
				float faultPresence = coarseFaultNoise.Noise( center ) * 0.6f + fineFaultNoise.Noise( center ) * 0.4f;
				faultHeight += faultMovement * (float) Parameters.FaultMaxHeight * faultPresence * gradient;

				faultIntensity += current.GetFaultReactionIntensity( other ) * float.Sqrt( gradient );
			}

			float mountainFactor = mountainRidgeNoise.BorderNoise( translation, 12 ) * mountainExistingNoise.Noise( center );
			float mountainHeight = mountainFactor * mountainFactor * (float) Parameters.MountainHeight;

			bool isMountain = mountainFactor > 0.5f || faultHeight > 750;

			float nHeight = currentHeight + otherAverageHeight + faultHeight + mountainHeight;

			face.State.SetHeight( nHeight );

			face.State.SetTerrainType( nHeight > 0 ? (isMountain ? mountainTerrain : grasslandTerrain) : (nHeight < -200 ? oceanTerrain : shorelineTerrain) );

			if (nHeight < 0)
				face.State.SetMoisture( 1 );

			face.State.SetSeismicActivity( faultIntensity );

		}
	}

	private void GenerateWinds( GlobeModel globe ) {
		float tileSpacing = 20f;     // meters, adjust if you compute it exactly
		float avgWindSpeed = 8f;       // m/s, tune to taste
		float dt = tileSpacing / avgWindSpeed;  // seconds per tile-hop

		for (uint i = 0; i < globe.FaceCount; i++) {
			var face = globe.Faces[ (int) i ];
			Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;

			float latitude = MathF.Asin( center.Y );
			float longitude = MathF.Atan2( center.Z, center.X );

			Rotor3<float> rotor = Rotor3<float>.MultiplicativeIdentity;
			rotor = Rotor3.FromAxisAngle( Vector3<float>.UnitY, float.Pi * 3 / 2 - longitude ) * rotor;
			rotor = Rotor3.FromAxisAngle( rotor.Left, float.Pi * 3 / 2 - latitude ) * rotor;

			if (latitude < 0)
				rotor = Rotor3.FromAxisAngle( center, float.Pi ) * rotor;
			rotor = rotor.Normalize<Rotor3<float>, float>();
			if (float.Abs( latitude ) > float.Pi / 2 - float.Pi / 6)
				rotor = Rotor3.FromAxisAngle( center, float.Pi * 2 / 3 * float.Sign( latitude ) ) * rotor;
			else if (float.Abs( latitude ) > float.Pi / 2 - float.Pi / 6 * 2)
				rotor = Rotor3.FromAxisAngle( center, -float.Pi / 3 * float.Sign( latitude ) ) * rotor;
			else
				rotor = Rotor3.FromAxisAngle( center, float.Pi * 2 / 3 * float.CopySign( 1, latitude ) ) * rotor;

			var direction = rotor.Forward;

			Vector3<float> windDirection = direction;//direction - direction.Dot( center ) * center;

			float f = 2 * (float) Parameters.RotationRate * center.Y;
			float dtheta = dt * f;

			float cosTh = float.Cos( dtheta );
			float sinTh = float.Sin( dtheta );

			var v = windDirection;
			var vRot = v * cosTh + Vector3<float>.UnitY.Cross( v ) * sinTh;
			windDirection = vRot - vRot.Dot(center) * center;
			face.State.SetWindDirection( windDirection.Normalize<Vector3<float>, float>() );

			//vectorToProject - vectorToProject.Dot( axisVector ) * axisVector
		}
	}
}