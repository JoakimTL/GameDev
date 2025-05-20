using Civlike.World.Generation;
using Civlike.World.TerrainTypes;
using Engine;
using Engine.Generation.Meshing;
using Engine.Structures;
using System;
using System.Drawing;
using System.Numerics;

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


public sealed class TectonicWorldTerrainGenerator : IWorldTerrainGenerator {

	public TectonicWorldTerrainGenerator( WorldGenerationParameters parameters ) {
		this.Parameters = parameters;
	}

	public WorldGenerationParameters Parameters { get; }

	public void GenerateTerrain( GlobeModel globe ) {
		GenerateLandmass( globe );
		SetTileTemperaturesAndPressure( globe );
		GenerateWinds( globe );
		for (int i = 0; i < 10; i++) {
			TweakWindDirectionAndWindPressure( globe );
		}
		SetUpwindDistancesFromOcean( globe );
		for (int i = 0; i < Parameters.MoistureLoops; i++)
			GeneratePrecipitation( globe );
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

	private void SetTileTemperaturesAndPressure( GlobeModel globe ) {
		float equatorTemperature = 30f;    // °C at 0° latitude
		float poleTemperature = -20f;   // °C at ±90° latitude
		float elevationTemperatureLapseRate = -6.5f; // °C/km
		float obliquityRad = (float) Parameters.ObliquityDegrees * float.Pi / 180f; // radians
		float revolutionsPerOrbit = (float) Parameters.RevolutionsPerOrbit;
		float seasonalAmp = 15f;

		float basePressure = 101325f; //Standard pressure at sea level in Pa
		float pressurePerMeter = 12f; //Pressure delta per meter elevation change in Pa
		float pressurePerKelvin = -100f; //Pressure delta per Kelvin temperature change in Pa

		for (uint i = 0; i < globe.FaceCount; i++) {
			var face = globe.Faces[ (int) i ];
			Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;

			float latitude = MathF.Asin( center.Y );

			float normLat = float.Abs( latitude ) / (float.Pi / 2);

			float tempCelsius = float.Lerp( equatorTemperature, poleTemperature, normLat );

			float elevationKm = face.State.PressureHeight / 1000f;

			float seasonalTemp = 0;
			for (float d = 0; d < revolutionsPerOrbit; d++) {
				float dayAngle = 2 * float.Pi * d / revolutionsPerOrbit;
				Vector3<float> sunDir = new Vector3<float>(
					float.Cos( dayAngle ),
					float.Sin( dayAngle ) * float.Sin( obliquityRad ),
					float.Sin( dayAngle ) * float.Cos( obliquityRad )
				).Normalize<Vector3<float>, float>();
				float insolation = center.Dot( sunDir );
				seasonalTemp += seasonalTemp * insolation;
			}

			float temperature = tempCelsius + (elevationKm * elevationTemperatureLapseRate) + seasonalTemp;

			face.State.SetTemperature( Temperature.FromCelsius( float.Lerp( equatorTemperature, poleTemperature, normLat ) ) );

			face.State.SetBaseWindPressure( new( basePressure + face.State.PressureHeight * pressurePerMeter + face.State.Temperature.Kelvin * pressurePerKelvin ) );
			face.State.SetWindPressure( new( basePressure + face.State.PressureHeight * pressurePerMeter + face.State.Temperature.Kelvin * pressurePerKelvin ) );
		}
	}

	private void GenerateWinds( GlobeModel globe ) {
		float tileSpacing = (float) globe.ApproximateTileLength;	// meters, adjust if you compute it exactly
		float avgWindSpeed = 8f;									// m/s, tune to taste
		float dt = tileSpacing / avgWindSpeed;						// seconds per tile-hop

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

			float dtheta = dt * 2 * (float) Parameters.RevolutionsPerSecond * -float.Abs(center.Y);

			// fast sin/cos:
			float cosTh = float.Cos( dtheta );
			float sinTh = float.Sin( dtheta );

			Vector3<float> vRot = direction * cosTh + Vector3<float>.UnitY.Cross( direction ) * sinTh;

			direction = (vRot - vRot.Dot( center ) * center).Normalize<Vector3<float>, float>();

			face.State.SetWindDirection( direction );

			//vectorToProject - vectorToProject.Dot( axisVector ) * axisVector
		}
	}

	private void TweakWindDirectionAndWindPressure( GlobeModel globe ) {
		float windPressureWeight = 0.2f;

		float[] newWindPressures = new float[ globe.FaceCount ];
		Vector3<float>[] newWindDirections = new Vector3<float>[ globe.FaceCount ];
		for (int i = 0; i < globe.FaceCount; i++) {
			newWindPressures[ i ] = globe.Faces[ i ].State.WindPressure.Pascal;
			newWindDirections[ i ] = globe.Faces[ i ].State.WindDirection;
		}

		for (int i = 0; i < globe.FaceCount; i++) {
			var face = globe.Faces[ i ];
			Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;

			Vector3<float> baseWind = face.State.WindDirection;
			Vector3<float> localWind = 0;

			foreach (var neighbourFace in face.Blueprint.Connections.Select( p => p.GetOther( face ) )) {
				var dir = (neighbourFace.Blueprint.GetCenter() - center).Normalize<Vector3<float>, float>();
				var rise = face.State.WindPressure.Pascal - neighbourFace.State.WindPressure.Pascal;
				var slope = rise / (float) globe.ApproximateTileLength;

				var alignment = float.Max( dir.Dot( baseWind ), 0 );

				localWind += dir * slope * windPressureWeight * alignment;
				var pressureTransfer = rise * windPressureWeight * alignment;
				newWindPressures[ i ] -= pressureTransfer;
				newWindPressures[ neighbourFace.Id ] += pressureTransfer;
			}

			newWindDirections[ i ] = (baseWind + localWind).Normalize<Vector3<float>, float>();
		}

		for (int i = 0; i < globe.FaceCount; i++) {
			var basePressure = globe.Faces[ i ].State.BaseWindPressure;
			globe.Faces[ i ].State.SetWindPressure( new( float.Max( newWindPressures[ i ], basePressure.Pascal ) ) );
			globe.Faces[ i ].State.SetWindDirection( newWindDirections[ i ] );
		}
	}

	private void SetUpwindDistancesFromOcean( GlobeModel globe ) {
		Queue<Face> unset = [];

	}

	private void GeneratePrecipitation( GlobeModel globe ) {
		for (uint i = 0; i < globe.FaceCount; i++) {
			var face = globe.Faces[ (int) i ];
			Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;
			var windSource = face.Blueprint.GetFaceInDirection( -face.State.WindDirection );
			float heightDifference = face.State.PressureHeight - windSource.State.PressureHeight;
			float heightSlope = heightDifference / (float) globe.ApproximateTileLength;

			if (windSource.State.Moisture > face.State.Moisture) {
				float carryoverMoisture = windSource.State.Moisture - face.State.Moisture;
				float expectedMoistureWithoutPrecipitation = face.State.Moisture + carryoverMoisture * 0.5f;
				float precipitationFromSlope = heightSlope * 0.5f * expectedMoistureWithoutPrecipitation;
				float upliftPrecipitation = float.Max( float.Min( precipitationFromSlope, carryoverMoisture ), 0 );
				float moisture = expectedMoistureWithoutPrecipitation - upliftPrecipitation;
				face.State.SetMoisture( moisture );
				face.State.SetPrecipitation( upliftPrecipitation );
			}

			//face.State.SetColor( (0, face.State.Moisture, face.State.Precipitation) );
			//face.State.SetColor( (float.Max( face.State.Temperature.Celsius / 50, 0 ), face.State.Pressure.Atmosphere, float.Max( (-face.State.Temperature.Celsius) / 20, 0 )) );
			face.State.SetColor( (face.State.WindPressure.Pascal / 1000 % 1, face.State.WindPressure.Pascal / 10000 % 1, face.State.WindPressure.Pascal / 100000 % 1) );
		}
	}
}

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