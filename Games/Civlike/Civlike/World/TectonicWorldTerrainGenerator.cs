using Civlike.World.Generation;
using Civlike.World.TerrainTypes;
using Engine;
using Engine.Logging;
using Engine.Modularity;
using Engine.Structures;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.World;

public sealed class TectonicWorldTerrainGenerator : IWorldTerrainGenerator {

	public TectonicWorldTerrainGenerator( WorldGenerationParameters parameters ) {
		this.Parameters = parameters;
	}

	public WorldGenerationParameters Parameters { get; }

	public void GenerateTerrain( GlobeModel globe ) {
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Generating landmasses!" ) );
		GenerateLandmass( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Defining local relief!" ) );
		DefineLocalRelief( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Setting temperatures!" ) );
		SetTileTemperaturesAndPressure( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Generating winds!" ) );
		GenerateWinds( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Tweaking wind directions!" ) );
		TweakWindDirectionAndWindPressure( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Calculating distances from bodies of water!" ) );
		SetDistancesFromOcean( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Setting evaporation rates!" ) );
		SetEvaporationRate( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Adding precipitation!" ) );
		GeneratePrecipitation( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Assigning terrain types!" ) );
		AssignTerrainTypes( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Completed world generation!" ) );

		//for (uint i = 0; i < globe.FaceCount; i++) {
		//	Face face = globe.Faces[ (int) i ];
		//	//face.State.SetColor( (distanceFromOcean > 0 ? 1f : 0f, distanceFromOcean / ((float) globe.ApproximateTileLength * 4) % 1, distanceFromOcean / ((float) globe.ApproximateTileLength * 50) % 1) );
		//	//face.State.SetColor( (float.Max( face.State.Temperature.Celsius / 50, 0 ), face.State.AtmosphericPressure.Atmosphere, float.Max( (-face.State.Temperature.Celsius) / 20, 0 )) );
		//	//face.State.SetColor( (face.State.AtmosphericPressure.Pascal / 1000 % 1, face.State.AtmosphericPressure.Pascal / 10000 % 1, face.State.AtmosphericPressure.Pascal / (Pressure.FromAtmosphere(1) + 1) % 1) );
		//	//face.State.SetColor( (face.State.WindPressure.Pascal / 1000 % 1, face.State.WindPressure.Pascal / 10000 % 1, face.State.WindPressure.Pascal / 100000 % 1) );
		//	//face.State.SetColor( (face.State.AbsoluteHumidityMm / 100 % 1, face.State.AbsoluteHumidityMm / 1000 % 1, face.State.AbsoluteHumidityMm / 10000 % 1) );
		//	face.State.SetColor( (face.State.PrecipitationMm / 100 % 1, face.State.PrecipitationMm / 1000 % 1, face.State.PrecipitationMm / 10000 % 1) );
		//}
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

		Noise3 coarseRuggednessNoise = new( seedProvider.Next(), 4 );
		Noise3 fineRuggednessNoise = new( seedProvider.Next(), 19 );

		ParallelForFaces( globe, 1, ( start, end, taskId ) => {
			List<(TectonicPlate plate, float gradient)> neighbourPlates = [];
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
				Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;

				Vector3<float> shift = new Vector3<float>( xShiftNoise.Noise( center ), yShiftNoise.Noise( center ), zShiftNoise.Noise( center ) ) * 2 - 1;
				Vector3<float> translation = center + shift * 0.05f;

				TectonicPlate current = tectonicRegionGenerator.Get( translation, neighbourPlates, 23, 0.0001f );
				float currentHeight = current.Height + ((Noise3.Noise( baseCoarseSeed + (uint) current.Id, baseCoarseScale, center ) * 0.7f + Noise3.Noise( baseFineSeed + (uint) current.Id, baseFineScale, center ) * 0.3f) * 2 - 1) * (float) Parameters.BaseHeightVariance;

				float otherAverageHeight = 0;
				float faultHeight = 0;
				float faultIntensity = 0;
				for (int n = 0; n < neighbourPlates.Count; n++) {
					(TectonicPlate other, float gradient) = neighbourPlates[ n ];
					//float gradientSq = gradient * gradient;

					float otherVariableHeight = ((Noise3.Noise( baseCoarseSeed + (uint) other.Id, baseCoarseScale, center ) * 0.7f + Noise3.Noise( baseFineSeed + (uint) other.Id, baseFineScale, center ) * 0.3f) * 2 - 1) * (float) Parameters.BaseHeightVariance;
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

				face.State.SetSeismicActivity( faultIntensity );

				face.State.SetRuggedness( coarseRuggednessNoise.Noise( center ) * 0.55f + fineRuggednessNoise.Noise( center ) * 0.45f );

			}
		} );
	}

	private void DefineLocalRelief( GlobeModel globe ) {
		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];

			float minHeight = face.State.Height;
			float maxHeight = face.State.Height;
			float minPressureHeight = face.State.PressureHeight;
			float maxPressureHeight = face.State.PressureHeight;

			foreach (Face neighbourFace in face.Blueprint.Neighbours) {
				var height = neighbourFace.State.Height;
				var pressureHeight = neighbourFace.State.PressureHeight;
				if (height < minHeight)
					minHeight = height;
				if (height > maxHeight)
					maxHeight = height;
				if (pressureHeight < minPressureHeight)
					minPressureHeight = pressureHeight;
				if (pressureHeight > maxPressureHeight)
					maxPressureHeight = pressureHeight;
			}

			face.State.SetLocalRelief( maxHeight - minHeight );
			face.State.SetLocalPressureRelief( maxPressureHeight - minPressureHeight );
		}
	}

	private void SetTileTemperaturesAndPressure( GlobeModel globe ) {
		const float P0 = 101325.0f;   // sea-level standard pressure, Pa
		const float g = 9.80665f;    // gravity, m/s²
		const float M = 0.0289644f;    // molar mass of dry air, kg/mol
		const float R0 = 8.3144598f;    // universal gas constant, J/(mol·K)

		float equatorTemperature = 40f;    // °C at 0° latitude
		float poleTemperature = -25f;   // °C at ±90° latitude
		float elevationTemperatureLapseRate = -6.5f; // °C/km
		float obliquityRad = (float) Parameters.ObliquityDegrees * float.Pi / 180f; // radians
		float revolutionsPerOrbit = (float) Parameters.RevolutionsPerOrbit;
		float seasonalAmp = 15f;

		float basePressure = 101325f; //Standard pressure at sea level in Pa
		float pressurePerMeter = 12f; //Pressure delta per meter elevation change in Pa
		float pressurePerKelvin = -100f; //Pressure delta per Kelvin temperature change in Pa

		ParallelForFaces( globe, 1, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
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
					seasonalTemp += seasonalTemp * insolation * seasonalAmp;
				}

				float temperature = tempCelsius + (elevationKm * elevationTemperatureLapseRate) + seasonalTemp;

				face.State.SetTemperature( Temperature.FromCelsius( float.Lerp( equatorTemperature, poleTemperature, normLat ) ) );

				face.State.SetAtmosphericPressure( P0 * float.Exp( -(g * face.State.PressureHeight * M) / (R0 * face.State.Temperature.Kelvin) ) );
				face.State.SetBaseWindPressure( basePressure + face.State.PressureHeight * pressurePerMeter + face.State.Temperature.Kelvin * pressurePerKelvin );
				face.State.SetWindPressure( basePressure + face.State.PressureHeight * pressurePerMeter + face.State.Temperature.Kelvin * pressurePerKelvin );
			}
		} );
	}

	private void GenerateWinds( GlobeModel globe ) {
		float tileSpacing = (float) globe.ApproximateTileLength;    // meters, adjust if you compute it exactly
		float avgWindSpeed = 8f;                                    // m/s, tune to taste
		float dt = tileSpacing / avgWindSpeed;                      // seconds per tile-hop

		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
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

			Vector3<float> direction = rotor.Forward;

			float dtheta = dt * 2 * (float) Parameters.RevolutionsPerSecond * -float.Abs( center.Y );

			// fast sin/cos:
			float cosTh = float.Cos( dtheta );
			float sinTh = float.Sin( dtheta );

			Vector3<float> vRot = direction * cosTh + Vector3<float>.UnitY.Cross( direction ) * sinTh;

			direction = (vRot - vRot.Dot( center ) * center).Normalize<Vector3<float>, float>();

			face.State.SetWindDirection( direction );

			//vectorToProject - vectorToProject.Dot( axisVector ) * axisVector
		}
	}

	private readonly struct TileNeighbourPrecomputation( Vector3<float> diff ) {
		public readonly Vector3<float> Diff = diff;
	}

	private void TweakWindDirectionAndWindPressure( GlobeModel globe ) {
		unsafe {
			int windPasses = 1500;
			int reservedThreads = 1; // reserve some threads for other tasks
			int taskCount = Math.Max( Environment.ProcessorCount - reservedThreads, 1 );
			float* windPressureDeltas = (float*) NativeMemory.Alloc( globe.FaceCount * (uint) taskCount * sizeof( float ) );
			Vector3<float>* newWindDirections = (Vector3<float>*) NativeMemory.Alloc( globe.FaceCount * (nuint) sizeof( Vector3<float> ) );
			TileNeighbourPrecomputation* precomputedNeighbours = (TileNeighbourPrecomputation*) NativeMemory.Alloc( globe.FaceCount * 3 * (nuint) sizeof( TileNeighbourPrecomputation ) );
			ParallelForFaces( globe, reservedThreads, ( start, end, taskId ) => {
				for (int i = start; i < end; i++) {
					Face face = globe.Faces[ i ];
					Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;
					for (int n = 0; n < 3; n++) {
						Face neighbourFace = face.Blueprint.Neighbours[ n ];
						Vector3<float> diff = neighbourFace.Blueprint.GetCenter() - center;
						precomputedNeighbours[ i * 3 + n ] = new TileNeighbourPrecomputation( diff.Normalize<Vector3<float>, float>() );
					}
				}
			} );
			for (int i = 0; i < windPasses; i++) {
				if (i % (windPasses / 10) == 0)
					MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Tweaking wind directions! ({(((double) i * 100) / windPasses):N1}%)" ) );
				TweakWindDirectionAndWindPressurePass( globe, windPressureDeltas, newWindDirections, precomputedNeighbours );
			}
			NativeMemory.Free( windPressureDeltas );
			NativeMemory.Free( newWindDirections );
			NativeMemory.Free( precomputedNeighbours );
		}
	}

	private unsafe void TweakWindDirectionAndWindPressurePass( GlobeModel globe, float* windPressureDeltas, Vector3<float>* newWindDirections, TileNeighbourPrecomputation* precomputedNeighbours ) {
		float diffusionWeight = 0.05f;      // general smoothing
		float advectionWeight = 0.175f;     // directional pressure‐driven flow
		int reservedThreads = 1; // reserve some threads for other tasks
		float invApproximateTileLength = .5f / (float) globe.ApproximateTileLength;
		uint faceCount = globe.FaceCount;

		ParallelForFaces( globe, reservedThreads, ( start, end, taskId ) => {
			uint offset = (uint) taskId * faceCount;
			float* localWindPressureDeltas = windPressureDeltas + offset;
			System.Runtime.CompilerServices.Unsafe.InitBlock( localWindPressureDeltas, 0, faceCount * sizeof( float ) );
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
				FaceState state = face.State;

				Vector3<float> baseWind = state.WindDirection;
				Vector3<float> localWind = default;

				float facePressure = state.WindPressure.Pascal + state.BaseWindPressure.Pascal * 0.5f;

				int faceBaseIndex = i * 3;
				IReadOnlyList<Face> neighbours = face.Blueprint.Neighbours;
				//int neighbourCount = neighbours.Count;
				for (int n = 0; n < 3; n++) {
					Face neighbourFace = neighbours[ n ];
					FaceState neighbourState = neighbourFace.State;
					Vector3<float> dir = precomputedNeighbours[ faceBaseIndex + n ].Diff;
					float rise = facePressure - neighbourState.WindPressure.Pascal - neighbourState.BaseWindPressure.Pascal * 0.5f;
					float slope = rise * invApproximateTileLength;

					float alignment = float.Max( dir.Dot( baseWind ), -0.25f );

					float pressureDelta = rise * (advectionWeight * alignment + diffusionWeight);
					localWind += dir * slope * advectionWeight * alignment;
					localWindPressureDeltas[ i ] -= pressureDelta;
					localWindPressureDeltas[ neighbourFace.Id ] += pressureDelta;
				}

				Vector3<float> combinedWind = baseWind + localWind;
				newWindDirections[ i ] = combinedWind.MagnitudeSquared() > 0.00001f
					? combinedWind.Normalize<Vector3<float>, float>()
					: baseWind;
			}
		} );

		int taskCount = Math.Max( Environment.ProcessorCount - 1, 1 );
		ParallelForFaces( globe, reservedThreads, ( start, end, taskId ) => {
			IReadOnlyList<Face> faces = globe.Faces;
			for (int i = start; i < end; i++) {
				FaceState state = faces[ i ].State;
				float windPressure = state.WindPressure.Pascal;
				for (int j = 0; j < taskCount; j++) {
					windPressure += windPressureDeltas[ j * faceCount + i ];
				}
				state.SetWindPressure( windPressure );
				state.SetWindDirection( newWindDirections[ i ] );
			}
		} );
	}

	private static void ParallelForFaces( GlobeModel globe, int reservedThreads, Action<int, int, int> parallelizedTask ) {
		int taskCount = Math.Max( Environment.ProcessorCount - reservedThreads, 1 );
		int facesPerTask = (int) Math.Ceiling( (double) globe.FaceCount / taskCount );
		Task[] tasks = new Task[ taskCount ];

		for (int t = 0; t < taskCount; t++) {
			int start = t * facesPerTask;
			int end = Math.Min( start + facesPerTask, (int) globe.FaceCount );
			int taskId = t;
			tasks[ t ] = Task.Run( () => parallelizedTask( start, end, taskId ) );
		}
		Task.WaitAll( tasks );
	}

	private void SetDistancesFromOcean( GlobeModel globe ) {
		SetLinearDistancesFromOcean( globe );
		SetUpwindDistancesFromOcean( globe );
	}

	private void SetLinearDistancesFromOcean( GlobeModel globe ) {
		Queue<Face> linearQueue = [];
		Queue<Face> unsetIdQueue = [];
		Dictionary<uint, int> linearAttempts = [];
		HashSet<uint> enqueuedLinear = [];

		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			if (face.State.Height >= 0) {
				if (face.Blueprint.Connections.Any( p => p.GetOther( face ).State.Height < 0 )) {
					if (enqueuedLinear.Add( face.Id ))
						linearQueue.Enqueue( face );
				}
			} else {
				face.State.SetLinearDistanceFromOcean( 0 );
			}
		}

		int linearAttemptLimit = 16; // Lowered and now enforced

		Face[] neighbours = new Face[ 3 ];
		while (linearQueue.TryDequeue( out Face? face )) {
			int tries = linearAttempts.GetValueOrDefault( face.Id );
			if (tries > linearAttemptLimit) {
				this.LogLine( $"[SetDistancesFromOcean] Linear attempt limit hit for face {face.Id}" );
				continue;
			}
			linearAttempts[ face.Id ] = tries + 1;

			var connections = face.Blueprint.Connections;
			float lowestDistance = face.State.LinearDistanceFromOcean;
			bool foundLower = false;
			bool undefinedNeighbour = false;

			for (int i = 0; i < 3; i++) {
				Face neighbour = connections[ i ].GetOther( face );
				neighbours[ i ] = neighbour;
				float neighbourDistance = neighbour.State.LinearDistanceFromOcean;
				if (neighbourDistance < lowestDistance + (float) globe.ApproximateTileLength) {
					lowestDistance = neighbourDistance + (float) globe.ApproximateTileLength;
					foundLower = true;
				}
				if (neighbourDistance == float.PositiveInfinity)
					undefinedNeighbour = true;
			}

			if (!foundLower) {
				if (undefinedNeighbour && enqueuedLinear.Add( face.Id ))
					linearQueue.Enqueue( face );
				continue;
			}

			face.State.SetLinearDistanceFromOcean( lowestDistance );

			for (int i = 0; i < 3; i++) {
				Face neighbour = neighbours[ i ];
				if (neighbour.State.LinearDistanceFromOcean > lowestDistance + (float) globe.ApproximateTileLength) {
					if (enqueuedLinear.Add( neighbour.Id ))
						linearQueue.Enqueue( neighbour );
				}
			}
		}
	}

	private void SetUpwindDistancesFromOcean( GlobeModel globe ) {
		Queue<Face> setQueue = [];
		Queue<Face> unsetIdQueue = [];
		Dictionary<uint, int> attempts = [];
		HashSet<uint> enqueuedUpwind = [];

		// Initialize unsetIdQueue for land faces
		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			if (face.State.Height >= 0) {
				unsetIdQueue.Enqueue( face );
			} else {
				face.State.SetUpwindDistanceFromOcean( 0 );
			}
		}

		int attemptLimit = 64; // Lowered from 128
		this.LogLine( $"Land: {((double) unsetIdQueue.Count / globe.FaceCount * 100):N2}% {unsetIdQueue.Count} tiles" );

		bool HandleFaceUpwind( Face face, Queue<Face> queue, Dictionary<uint, int> attempts, int attemptLimit, HashSet<uint> enqueuedUpwind ) {
			if (face.State.UpwindDistanceFromOcean != float.PositiveInfinity)
				return true;
			Face upwindFace = face.Blueprint.GetFaceInDirection( -face.State.WindDirection );
			if (upwindFace.State.UpwindDistanceFromOcean == float.PositiveInfinity) {
				int numberOfAttempts = attempts.GetValueOrDefault( face.Id );
				if (numberOfAttempts > attemptLimit)
					return true;
				attempts[ face.Id ] = numberOfAttempts + 1;
				return false;
			}
			Vector3<float> positionDiff = (face.Blueprint.GetCenter() - upwindFace.Blueprint.GetCenter()).Normalize<Vector3<float>, float>();
			float aligment = 2 - float.Max( upwindFace.State.WindDirection.Dot( positionDiff ), 0 );
			face.State.SetUpwindDistanceFromOcean( upwindFace.State.UpwindDistanceFromOcean + (float) globe.ApproximateTileLength * aligment );
			Face nextFace = face.Blueprint.GetFaceInDirection( face.State.WindDirection );
			if (enqueuedUpwind.Add( nextFace.Id )) {
				queue.Enqueue( nextFace );
			}
			return true;
		}

		while (unsetIdQueue.TryDequeue( out Face? face )) {
			if (!HandleFaceUpwind( face, setQueue, attempts, attemptLimit, enqueuedUpwind ))
				unsetIdQueue.Enqueue( face );

			while (setQueue.TryDequeue( out face ))
				HandleFaceUpwind( face, setQueue, attempts, attemptLimit, enqueuedUpwind );
		}
	}

	private void SetEvaporationRate( GlobeModel globe ) {
		float E_max = 2000f;   // mm/yr at the hottest, windiest equatorial tile
		float T_min = -10f;      // °C below which we assume almost no evaporation
		float T_max = 30f;     // °C at which we reach E_max
		float windRef = 10f;     // m/s – reference wind speed for a +50% boost

		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			FaceState state = face.State;
			if (state.Height >= 0)
				continue;

			float temperature = state.Temperature.Celsius;

			float fT = float.Max( (temperature - T_min) / (T_max - T_min), 0 );

			float fW = 1;

			state.SetEvaporationMm( E_max * fT * fW );
		}
	}

	private void GeneratePrecipitation( GlobeModel globe ) {
		int reservedThreads = 1;
		var landFaces = globe.Faces.Where( p => p.State.Height >= 0 ).ToList();
		var facesByUpwindDistance = landFaces.OrderBy( p => p.State.UpwindDistanceFromOcean ).ToList();
		var facesByLinearDistance = landFaces.OrderBy( p => p.State.LinearDistanceFromOcean ).ToList();
		unsafe {
			float* moistureCapacityMm = (float*) NativeMemory.Alloc( globe.FaceCount * sizeof( float ) );
			float* precipitationDeltaMm = (float*) NativeMemory.Alloc( globe.FaceCount * sizeof( float ) );
			int taskCount = Math.Max( Environment.ProcessorCount - reservedThreads, 1 );
			float* humidityDeltaMm = (float*) NativeMemory.Alloc( globe.FaceCount * (uint) taskCount * sizeof( float ) );

			for (int i = 0; i < globe.FaceCount; i++)
				moistureCapacityMm[ i ] = globe.Faces[ i ].State.GetMoistureCapacityMm();

			TileNeighbourPrecomputation* precomputedNeighbours = (TileNeighbourPrecomputation*) NativeMemory.Alloc( globe.FaceCount * 3 * (nuint) sizeof( TileNeighbourPrecomputation ) );
			ParallelForFaces( globe, reservedThreads, ( start, end, taskId ) => {
				for (int i = start; i < end; i++) {
					Face face = globe.Faces[ i ];
					Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;
					for (int n = 0; n < 3; n++) {
						Face neighbourFace = face.Blueprint.Neighbours[ n ];
						Vector3<float> diff = neighbourFace.Blueprint.GetCenter() - center;
						precomputedNeighbours[ i * 3 + n ] = new TileNeighbourPrecomputation( diff.Normalize<Vector3<float>, float>() );
					}
				}
			} );

			int passes = (int) double.Ceiling( Parameters.RevolutionsPerOrbit );
			for (int i = 0; i < passes; i++) {
				if (i % (passes / 10) == 0)
					MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Adding precipitation! ({(((double) i * 100) / Parameters.RevolutionsPerOrbit):N1}%)" ) );
				GeneratePrecipitationPass( globe, facesByUpwindDistance, facesByLinearDistance, precomputedNeighbours, moistureCapacityMm, precipitationDeltaMm, humidityDeltaMm );
			}

			NativeMemory.Free( moistureCapacityMm );
			NativeMemory.Free( precipitationDeltaMm );
			NativeMemory.Free( humidityDeltaMm );
			NativeMemory.Free( precomputedNeighbours );

			float* newPrecipitationMm = (float*) NativeMemory.Alloc( globe.FaceCount * sizeof( float ) );

			for (int i = 0; i < 8; i++)
				BlurPrecipitatePass( globe, newPrecipitationMm );

			NativeMemory.Free( newPrecipitationMm );
		}
	}

	private unsafe void GeneratePrecipitationPass( GlobeModel globe, List<Face> facesByUpwindDistance, List<Face> facesByLinearDistance, TileNeighbourPrecomputation* precomputedNeighbours, float* moistureCapacityMm, float* precipitationDeltaMm, float* humidityDeltaMm ) {
		int reservedThreads = 1;
		int taskCount = Math.Max( Environment.ProcessorCount - reservedThreads, 1 );
		//float windSpeed = 12f * 60 * 24;
		//float diffuseWindSpeed = 4f * 60 * 24;
		float windFactor = 0.6f;
		float diffuseFactor = 0.4f;
		float bumpRainK = 0.00125f;    // tuning: mm of rain per mm of incoming moisture per meter of bumpy local relief

		uint faceCount = globe.FaceCount;

		System.Runtime.CompilerServices.Unsafe.InitBlock( precipitationDeltaMm, 0, globe.FaceCount * sizeof( float ) );

		ParallelFor( facesByUpwindDistance.Count, reservedThreads, ( start, end, taskId ) => {
			uint offset = (uint) taskId * faceCount;
			float* localHumidityDeltaMm = humidityDeltaMm + offset;
			System.Runtime.CompilerServices.Unsafe.InitBlock( localHumidityDeltaMm, 0, faceCount * sizeof( float ) );
			for (int i = start; i < end; i++) {
				Face face = facesByUpwindDistance[ i ];
				uint faceId = face.Id;

				float incomingMm = 0;
				float outgoingToOcean = 0;
				for (int ni = 0; ni < 3; ni++) {
					Face neighbour = face.Blueprint.Neighbours[ ni ];
					var diff = -precomputedNeighbours[ faceId * 3 + ni ].Diff;

					var dot = diff.Dot( neighbour.State.WindDirection );
					var score = float.Max( dot, 0 );

					float neighbourHumidity = float.Max( neighbour.State.AbsoluteHumidityMm, neighbour.State.EvaporationMm / (float) Parameters.RevolutionsPerOrbit );
					float incomingFromNeighbour = neighbourHumidity * score * windFactor;//windSpeed / (float) globe.ApproximateTileLength;
					incomingMm += incomingFromNeighbour;
					localHumidityDeltaMm[ neighbour.Id ] -= incomingFromNeighbour * 0.92f;

					if (neighbour.State.Height >= 0)
						continue;

					outgoingToOcean += float.Min( float.Max( -dot + .5f, 0 ), 1 );
				}
				outgoingToOcean = float.Min( outgoingToOcean, 1 );

				incomingMm *= 1 - outgoingToOcean;

				float moistureCapacity = moistureCapacityMm[ faceId ];
				float rainCap = float.Max( (incomingMm + face.State.AbsoluteHumidityMm) - moistureCapacity, 0 ) * 0.5f;

				float rainTerrain = (incomingMm - rainCap) * face.State.Ruggedness * face.State.LocalPressureRelief * bumpRainK;

				float rainMm = float.Min( rainCap + rainTerrain, incomingMm );

				localHumidityDeltaMm[ faceId ] += incomingMm - rainMm;
				precipitationDeltaMm[ faceId ] += rainMm;
			}
		} );

		ParallelFor( facesByUpwindDistance.Count, reservedThreads, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = facesByUpwindDistance[ i ];
				float humidity = face.State.AbsoluteHumidityMm;
				for (int t = 0; t < taskCount; t++)
					humidity += humidityDeltaMm[ t * faceCount + face.Id ];
				face.State.SetAbsoluteHumidityMm( humidity );
			}
		} );

		ParallelFor( facesByLinearDistance.Count, reservedThreads, ( start, end, taskId ) => {
			uint offset = (uint) taskId * faceCount;
			float* localHumidityDeltaMm = humidityDeltaMm + offset;
			System.Runtime.CompilerServices.Unsafe.InitBlock( localHumidityDeltaMm, 0, faceCount * sizeof( float ) );
			for (int i = start; i < end; i++) {
				Face face = facesByLinearDistance[ i ];
				uint faceId = face.Id;

				float incomingMm = 0;
				for (int ni = 0; ni < 3; ni++) {
					Face neighbour = face.Blueprint.Neighbours[ ni ];
					var diff = -precomputedNeighbours[ faceId * 3 + ni ].Diff;

					var dot = diff.Dot( neighbour.State.WindDirection );
					var score = (dot + 1) * 0.25f + 0.5f;

					float neighbourHumidity = float.Max( neighbour.State.AbsoluteHumidityMm, neighbour.State.EvaporationMm / (float) Parameters.RevolutionsPerOrbit );
					float incomingFromNeighbour = neighbourHumidity * score * diffuseFactor;//diffuseWindSpeed / (float) globe.ApproximateTileLength;
					incomingMm += incomingFromNeighbour;
					localHumidityDeltaMm[ neighbour.Id ] -= incomingFromNeighbour * 0.92f;
				}

				float moistureCapacity = moistureCapacityMm[ faceId ];
				float rainCap = float.Max( (incomingMm + face.State.AbsoluteHumidityMm) - moistureCapacity, 0 ) * 0.5f;

				float rainTerrain = (incomingMm - rainCap) * face.State.Ruggedness * face.State.LocalPressureRelief * bumpRainK;

				float rainMm = float.Min( rainCap + rainTerrain, incomingMm );

				localHumidityDeltaMm[ faceId ] += incomingMm - rainMm;
				precipitationDeltaMm[ faceId ] += rainMm;
			}
		} );

		ParallelFor( facesByUpwindDistance.Count, reservedThreads, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = facesByUpwindDistance[ i ];
				uint faceId = face.Id;
				float humidity = face.State.AbsoluteHumidityMm;
				for (int t = 0; t < taskCount; t++)
					humidity += humidityDeltaMm[ t * faceCount + face.Id ];
				var capacity = moistureCapacityMm[ face.Id ];
				var rainCap = float.Max( humidity - capacity, 0 );

				face.State.SetAbsoluteHumidityMm( humidity - rainCap );

				float outgoingToOcean = 0;
				for (int ni = 0; ni < 3; ni++) {
					Face neighbour = face.Blueprint.Neighbours[ ni ];
					if (neighbour.State.Height >= 0)
						continue;
					var diff = -precomputedNeighbours[ faceId * 3 + ni ].Diff;

					var dot = float.Min( float.Max( diff.Dot( neighbour.State.WindDirection ) + .5f, 0 ), 1 );

					outgoingToOcean += rainCap * dot;
				}
				face.State.SetPrecipitationMm( face.State.PrecipitationMm + precipitationDeltaMm[ face.Id ] + float.Max( rainCap - outgoingToOcean, 0 ) );
			}
		} );
	}

	private unsafe void BlurPrecipitatePass( GlobeModel globe, float* newPrecipitationMm ) {
		int reservedThreads = 1;

		Unsafe.InitBlock( newPrecipitationMm, 0, globe.FaceCount * sizeof( float ) );

		ParallelForFaces( globe, reservedThreads, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
				uint faceId = face.Id;
				float precipitationMm = face.State.PrecipitationMm;
				for (int ni = 0; ni < 3; ni++) {
					Face neighbour = face.Blueprint.Neighbours[ ni ];
					precipitationMm += neighbour.State.PrecipitationMm;
				}
				newPrecipitationMm[ faceId ] = precipitationMm / 4;
			}
		} );

		ParallelForFaces( globe, reservedThreads, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
				uint faceId = face.Id;
				float precipitationMm = newPrecipitationMm[ faceId ];
				face.State.SetPrecipitationMm( precipitationMm );
			}
		} );
	}

	private static void ParallelFor( int count, int reservedThreads, Action<int, int, int> parallelizedTask ) {
		int taskCount = Math.Max( Environment.ProcessorCount - reservedThreads, 1 );
		int facesPerTask = (int) Math.Ceiling( (double) count / taskCount );
		Task[] tasks = new Task[ taskCount ];

		for (int t = 0; t < taskCount; t++) {
			int start = t * facesPerTask;
			int end = Math.Min( start + facesPerTask, count );
			int taskId = t;
			tasks[ t ] = Task.Run( () => parallelizedTask( start, end, taskId ) );
		}
		Task.WaitAll( tasks );
	}

	private void AssignTerrainTypes( GlobeModel globe ) {
		GrasslandTerrain grasslandTerrain = TerrainTypeList.GetTerrainType<GrasslandTerrain>();
		OceanTerrain oceanTerrain = TerrainTypeList.GetTerrainType<OceanTerrain>();
		MountainTerrain mountainTerrain = TerrainTypeList.GetTerrainType<MountainTerrain>();
		ShorelineTerrain shorelineTerrain = TerrainTypeList.GetTerrainType<ShorelineTerrain>();
		FrozenDesertTerrain frozenDesertTerrain = TerrainTypeList.GetTerrainType<FrozenDesertTerrain>();
		WarmDesertTerrain warmDesertTerrain = TerrainTypeList.GetTerrainType<WarmDesertTerrain>();
		RainforestTerrain rainforestTerrain = TerrainTypeList.GetTerrainType<RainforestTerrain>();
		ForestTerrain forestTerrain = TerrainTypeList.GetTerrainType<ForestTerrain>();
		BoglandTerrain boglandTerrain = TerrainTypeList.GetTerrainType<BoglandTerrain>();

		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			if (face.State.Height < 0) {
				if (face.State.Height < -250) {
					face.State.SetTerrainType( oceanTerrain );
					continue;
				}
				face.State.SetTerrainType( shorelineTerrain );
				continue;
			}

			if (face.State.Ruggedness * face.State.LocalRelief > 100) {
				face.State.SetTerrainType( mountainTerrain );
				continue;
			}

			if (face.State.PrecipitationMm < 50) {
				if (face.State.Temperature.Celsius < 0) {
					face.State.SetTerrainType( frozenDesertTerrain );
					continue;
				}
				face.State.SetTerrainType( warmDesertTerrain );
				continue;
			}

			if (face.State.PrecipitationMm > 200 && face.State.Ruggedness * face.State.LocalRelief < 5) {
				face.State.SetTerrainType( boglandTerrain );
				continue;
			}

			if (face.State.PrecipitationMm > 3000) {
				//Need a way to define where vegetation should be!
				face.State.SetTerrainType( rainforestTerrain );
				continue;
			}

			face.State.SetTerrainType( grasslandTerrain );
			//if (face.State.PrecipitationMm > 1000) {
			//	face.State.SetTerrainType( forestTerrain );
			//	continue;
			//}
		}
	}
}