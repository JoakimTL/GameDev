//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

using Civlike.Messages;
using Engine;
using Engine.Modularity;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Civlike.WorldOld.Generation.Steps;

public sealed class GeneratePrecipitationStep : ITerrainGenerationProcessingStep {
	private readonly struct TileNeighbourPrecomputation( Vector3<float> diff ) {
		public readonly Vector3<float> Diff = diff;
	}

	public string ProcessingStepMessage => "Generating precipitation";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		int reservedThreads = 1;
		List<Face> landFaces = [ .. globe.Faces.Where( p => p.State.Height >= 0 ) ];
		List<Face> facesByUpwindDistance = [ .. landFaces.OrderBy( p => p.State.UpwindDistanceFromOcean ) ];
		List<Face> facesByLinearDistance = [ .. landFaces.OrderBy( p => p.State.LinearDistanceFromOcean ) ];
		unsafe {
			float* moistureCapacityMm = (float*) NativeMemory.Alloc( globe.FaceCount * sizeof( float ) );
			float* precipitationDeltaMm = (float*) NativeMemory.Alloc( globe.FaceCount * sizeof( float ) );
			int taskCount = Math.Max( Environment.ProcessorCount - reservedThreads, 1 );
			float* humidityDeltaMm = (float*) NativeMemory.Alloc( globe.FaceCount * (uint) taskCount * sizeof( float ) );

			for (int i = 0; i < globe.FaceCount; i++)
				moistureCapacityMm[ i ] = globe.Faces[ i ].State.GetMoistureCapacityMm();

			TileNeighbourPrecomputation* precomputedNeighbours = (TileNeighbourPrecomputation*) NativeMemory.Alloc( globe.FaceCount * 3 * (nuint) sizeof( TileNeighbourPrecomputation ) );
			ParallelProcessing.ForFaces( globe, reservedThreads, ( start, end, taskId ) => {
				for (int i = start; i < end; i++) {
					Face face = globe.Faces[ i ];
					Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;
					for (int n = 0; n < 3; n++) {
						Face neighbourFace = face.Blueprint.Neighbours[ n ];
						Vector3<float> diff = neighbourFace.Blueprint.GetCenter() - center;
						precomputedNeighbours[ (i * 3) + n ] = new TileNeighbourPrecomputation( diff.Normalize<Vector3<float>, float>() );
					}
				}
			} );

			int passes = (int) double.Ceiling( parameters.RevolutionsPerOrbit );
			for (int i = 0; i < passes; i++) {
				if (i % (passes / 50) == 0)
					MessageBus.PublishAnonymously( new WorldGenerationStepProgressPercentMessage( i * (100 / parameters.RevolutionsPerOrbit) ) );
				GeneratePrecipitationPass( parameters, globe, facesByUpwindDistance, facesByLinearDistance, precomputedNeighbours, moistureCapacityMm, precipitationDeltaMm, humidityDeltaMm );
			}

			NativeMemory.Free( moistureCapacityMm );
			NativeMemory.Free( precipitationDeltaMm );
			NativeMemory.Free( humidityDeltaMm );
			NativeMemory.Free( precomputedNeighbours );

			float* newPrecipitationMm = (float*) NativeMemory.Alloc( globe.FaceCount * sizeof( float ) );

			for (int i = 0; i < 8; i++)
				BlurPrecipitatePass( parameters, globe, newPrecipitationMm );

			NativeMemory.Free( newPrecipitationMm );
		}
	}

	private unsafe void GeneratePrecipitationPass( WorldGenerationParameters parameters, GlobeModel globe, List<Face> facesByUpwindDistance, List<Face> facesByLinearDistance, TileNeighbourPrecomputation* precomputedNeighbours, float* moistureCapacityMm, float* precipitationDeltaMm, float* humidityDeltaMm ) {
		int reservedThreads = 1;
		int taskCount = Math.Max( Environment.ProcessorCount - reservedThreads, 1 );
		//float windSpeed = 12f * 60 * 24;
		//float diffuseWindSpeed = 4f * 60 * 24;
		float windFactor = 0.6f;
		float diffuseFactor = 0.4f;
		float bumpRainK = 0.00024414062f;    // tuning: mm of rain per mm of incoming moisture per meter of bumpy local relief

		uint faceCount = globe.FaceCount;

		Unsafe.InitBlock( precipitationDeltaMm, 0, globe.FaceCount * sizeof( float ) );

		ParallelProcessing.For( facesByUpwindDistance.Count, reservedThreads, ( start, end, taskId ) => {
			uint offset = (uint) taskId * faceCount;
			float* localHumidityDeltaMm = humidityDeltaMm + offset;
			Unsafe.InitBlock( localHumidityDeltaMm, 0, faceCount * sizeof( float ) );
			for (int i = start; i < end; i++) {
				Face face = facesByUpwindDistance[ i ];
				uint faceId = face.Id;

				float incomingMm = 0;
				float outgoingToOcean = 0;
				for (int ni = 0; ni < 3; ni++) {
					Face neighbour = face.Blueprint.Neighbours[ ni ];
					Vector3<float> diff = -precomputedNeighbours[ (faceId * 3) + ni ].Diff;

					float dot = diff.Dot( neighbour.State.WindDirection );
					float score = float.Max( dot, 0 );

					float neighbourHumidity = float.Max( neighbour.State.AbsoluteHumidityMm, neighbour.State.EvaporationMm / (float) parameters.RevolutionsPerOrbit );
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
				float rainCap = float.Max( incomingMm + face.State.AbsoluteHumidityMm - moistureCapacity, 0 ) * 0.5f;

				float rainTerrain = (incomingMm - rainCap) * face.State.Ruggedness * face.State.LocalPressureRelief * bumpRainK;

				float rainMm = float.Min( rainCap + rainTerrain, incomingMm );

				localHumidityDeltaMm[ faceId ] += incomingMm - rainMm;
				precipitationDeltaMm[ faceId ] += rainMm;
			}
		} );

		ParallelProcessing.For( facesByUpwindDistance.Count, reservedThreads, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = facesByUpwindDistance[ i ];
				float humidity = face.State.AbsoluteHumidityMm;
				for (int t = 0; t < taskCount; t++)
					humidity += humidityDeltaMm[ (t * faceCount) + face.Id ];
				face.State.SetAbsoluteHumidityMm( humidity );
			}
		} );

		ParallelProcessing.For( facesByLinearDistance.Count, reservedThreads, ( start, end, taskId ) => {
			uint offset = (uint) taskId * faceCount;
			float* localHumidityDeltaMm = humidityDeltaMm + offset;
			Unsafe.InitBlock( localHumidityDeltaMm, 0, faceCount * sizeof( float ) );
			for (int i = start; i < end; i++) {
				Face face = facesByLinearDistance[ i ];
				uint faceId = face.Id;

				float incomingMm = 0;
				for (int ni = 0; ni < 3; ni++) {
					Face neighbour = face.Blueprint.Neighbours[ ni ];
					Vector3<float> diff = -precomputedNeighbours[ (faceId * 3) + ni ].Diff;

					float dot = diff.Dot( neighbour.State.WindDirection );
					float score = ((dot + 1) * 0.25f) + 0.5f;

					float neighbourHumidity = float.Max( neighbour.State.AbsoluteHumidityMm, neighbour.State.EvaporationMm / (float) parameters.RevolutionsPerOrbit );
					float incomingFromNeighbour = neighbourHumidity * score * diffuseFactor;//diffuseWindSpeed / (float) globe.ApproximateTileLength;
					incomingMm += incomingFromNeighbour;
					localHumidityDeltaMm[ neighbour.Id ] -= incomingFromNeighbour * 0.92f;
				}

				float moistureCapacity = moistureCapacityMm[ faceId ];
				float rainCap = float.Max( incomingMm + face.State.AbsoluteHumidityMm - moistureCapacity, 0 ) * 0.5f;

				float rainTerrain = (incomingMm - rainCap) * face.State.Ruggedness * face.State.LocalPressureRelief * bumpRainK;

				float rainMm = float.Min( rainCap + rainTerrain, incomingMm );

				localHumidityDeltaMm[ faceId ] += incomingMm - rainMm;
				precipitationDeltaMm[ faceId ] += rainMm;
			}
		} );

		ParallelProcessing.For( facesByUpwindDistance.Count, reservedThreads, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = facesByUpwindDistance[ i ];
				uint faceId = face.Id;
				float humidity = face.State.AbsoluteHumidityMm;
				for (int t = 0; t < taskCount; t++)
					humidity += humidityDeltaMm[ (t * faceCount) + face.Id ];
				float capacity = moistureCapacityMm[ face.Id ];
				float rainCap = float.Max( humidity - capacity, 0 );

				face.State.SetAbsoluteHumidityMm( humidity - rainCap );

				float outgoingToOcean = 0;
				for (int ni = 0; ni < 3; ni++) {
					Face neighbour = face.Blueprint.Neighbours[ ni ];
					if (neighbour.State.Height >= 0)
						continue;
					Vector3<float> diff = -precomputedNeighbours[ (faceId * 3) + ni ].Diff;

					float dot = float.Min( float.Max( diff.Dot( neighbour.State.WindDirection ) + .5f, 0 ), 1 );

					outgoingToOcean += rainCap * dot;
				}
				face.State.SetPrecipitationMm( face.State.PrecipitationMm + precipitationDeltaMm[ face.Id ] + float.Max( rainCap - outgoingToOcean, 0 ) );
			}
		} );
	}

	private unsafe void BlurPrecipitatePass( WorldGenerationParameters parameters, GlobeModel globe, float* newPrecipitationMm ) {
		int reservedThreads = 1;

		Unsafe.InitBlock( newPrecipitationMm, 0, globe.FaceCount * sizeof( float ) );

		ParallelProcessing.ForFaces( globe, reservedThreads, ( start, end, taskId ) => {
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

		ParallelProcessing.ForFaces( globe, reservedThreads, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
				uint faceId = face.Id;
				float precipitationMm = newPrecipitationMm[ faceId ];
				face.State.SetPrecipitationMm( precipitationMm );
			}
		} );
	}
}
