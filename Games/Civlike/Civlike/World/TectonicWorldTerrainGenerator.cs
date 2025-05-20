using Civlike.World.Generation;
using Civlike.World.TerrainTypes;
using Engine;
using Engine.Logging;
using Engine.Modularity;

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
		int windPasses = 150;
		for (int i = 0; i < windPasses; i++) {
			MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Tweaking wind directions! ({i + 1}/{windPasses})" ) );
			TweakWindDirectionAndWindPressure( globe );
		}
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Calculating distances from bodies of water!" ) );
		SetDistancesFromOcean( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Adding precipitation!" ) );
		for (int i = 0; i < Parameters.MoistureLoops; i++)
			GeneratePrecipitation( globe );
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( "Completed world generation!" ) );
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

			face.State.SetRuggedness( coarseRuggednessNoise.Noise( center ) * 0.55f + fineRuggednessNoise.Noise( center ) * 0.45f );

			if (i % 10000 == 0)
				MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Generating landmasses! ({((double) i / globe.FaceCount * 100):N2}%)" ) );

		}
	}

	private void DefineLocalRelief( GlobeModel globe ) {
		for (uint i = 0; i < globe.FaceCount; i++) {
			var face = globe.Faces[ (int) i ];

			float minHeight = face.State.Height;
			float maxHeight = face.State.Height;

			foreach (var neighbourFace in face.Blueprint.Connections.Select( p => p.GetOther( face ) )) {
				if (neighbourFace.State.Height < minHeight)
					minHeight = neighbourFace.State.Height;
				if (neighbourFace.State.Height > maxHeight)
					maxHeight = neighbourFace.State.Height;
			}

			face.State.SetLocalRelief( maxHeight - minHeight );
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

			if (i % 10000 == 0)
				MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Setting temperatures! ({((double) i / globe.FaceCount * 100):N2}%)" ) );
		}
	}

	private void GenerateWinds( GlobeModel globe ) {
		float tileSpacing = (float) globe.ApproximateTileLength;    // meters, adjust if you compute it exactly
		float avgWindSpeed = 8f;                                    // m/s, tune to taste
		float dt = tileSpacing / avgWindSpeed;                      // seconds per tile-hop

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

	private void TweakWindDirectionAndWindPressure( GlobeModel globe ) {
		float windPressureWeight = 0.3f;

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

	private void SetDistancesFromOcean( GlobeModel globe ) {
		// Returns true if the face should be removed from the hashset
		bool HandleFaceUpwind( Face face, Queue<Face> queue, Dictionary<uint, int> attempts, int attemptLimit ) {
			if (face.State.UpwindDistanceFromOcean != float.PositiveInfinity)
				return true;
			var upwindFace = face.Blueprint.GetFaceInDirection( -face.State.WindDirection );
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
			queue.Enqueue( face.Blueprint.GetFaceInDirection( face.State.WindDirection ) );
			return true;
		}

		Queue<Face> linearQueue = [];
		Queue<Face> setQueue = [];
		Queue<Face> unsetIdQueue = [];
		Dictionary<uint, int> attempts = [];

		for (uint i = 0; i < globe.FaceCount; i++) {
			var face = globe.Faces[ (int) i ];
			if (face.State.Height >= 0) {
				unsetIdQueue.Enqueue( face );
				if (face.Blueprint.Connections.Any( p => p.GetOther( face ).State.Height < 0 )) {
					linearQueue.Enqueue( face );
				}
			} else {
				face.State.SetLinearDistanceFromOcean( 0 );
				face.State.SetUpwindDistanceFromOcean( 0 );
			}
		}

		int attemptLimit = 128;//		unset.Count / 2;
		this.LogLine( $"Land: {((double) unsetIdQueue.Count / globe.FaceCount * 100):N2}% {unsetIdQueue.Count} tiles" );

		Face[] neighbours = new Face[ 3 ];
		while (linearQueue.TryDequeue( out Face? face )) {
			for (int i = 0; i < 3; i++)
				neighbours[ i ] = face.Blueprint.Connections[ i ].GetOther( face );
			float lowestDistance = face.State.LinearDistanceFromOcean;
			bool foundLower = false;
			bool undefinedNeighbour = false;
			foreach (var neighbour in neighbours) {
				if (neighbour.State.LinearDistanceFromOcean < lowestDistance + (float) globe.ApproximateTileLength) {
					lowestDistance = neighbour.State.LinearDistanceFromOcean + (float) globe.ApproximateTileLength;
					foundLower = true;
				}
				if (neighbour.State.LinearDistanceFromOcean == float.PositiveInfinity)
					undefinedNeighbour = true;
			}
			if (!foundLower) {
				if (undefinedNeighbour)
					linearQueue.Enqueue( face );
				continue;
			}
			face.State.SetLinearDistanceFromOcean( lowestDistance );
			foreach (var neighbour in neighbours)
				if (neighbour.State.LinearDistanceFromOcean > lowestDistance + (float) globe.ApproximateTileLength)
					linearQueue.Enqueue( neighbour );
		}

		while (unsetIdQueue.TryDequeue( out Face? face )) {
			if (!HandleFaceUpwind( face, setQueue, attempts, attemptLimit ))
				unsetIdQueue.Enqueue( face );

			while (setQueue.TryDequeue( out face ))
				HandleFaceUpwind( face, setQueue, attempts, attemptLimit );
		}
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

			float distanceFromOcean = float.Min( face.State.LinearDistanceFromOcean * 2, face.State.UpwindDistanceFromOcean );
			face.State.SetColor( (distanceFromOcean > 0 ? 1f : 0f, distanceFromOcean / ((float) globe.ApproximateTileLength * 4) % 1, distanceFromOcean / ((float) globe.ApproximateTileLength * 50) % 1) );
			//face.State.SetColor( (float.Max( face.State.Temperature.Celsius / 50, 0 ), face.State.Pressure.Atmosphere, float.Max( (-face.State.Temperature.Celsius) / 20, 0 )) );
			//face.State.SetColor( (face.State.WindPressure.Pascal / 1000 % 1, face.State.WindPressure.Pascal / 10000 % 1, face.State.WindPressure.Pascal / 100000 % 1) );
		}
	}
}
