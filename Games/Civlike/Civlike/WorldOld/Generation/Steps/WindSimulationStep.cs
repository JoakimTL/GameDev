using Civlike.Messages;
using Engine;
using Engine.Modularity;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.WorldOld.Generation.Steps;

public sealed class WindSimulationStep( int passes ) : ITerrainGenerationProcessingStep {
	private readonly struct TileNeighbourPrecomputation( Vector3<float> diff ) {
		public readonly Vector3<float> Diff = diff;
	}

	public int Passes { get; } = passes;
	public string ProcessingStepMessage => "Simulating winds";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		unsafe {
			int reservedThreads = 1; // reserve some threads for other tasks
			int taskCount = Math.Max( Environment.ProcessorCount - reservedThreads, 1 );
			float* windPressureDeltas = (float*) NativeMemory.Alloc( globe.FaceCount * (uint) taskCount * sizeof( float ) );
			Vector3<float>* newWindDirections = (Vector3<float>*) NativeMemory.Alloc( globe.FaceCount * (nuint) sizeof( Vector3<float> ) );
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
			for (int i = 0; i < this.Passes; i++) {
				if (i % (this.Passes / 80) == 0)
					MessageBus.PublishAnonymously( new WorldGenerationStepProgressPercentMessage( i * 100d / this.Passes ) );
				TweakWindDirectionAndDynamicPressurePass( parameters, globe, windPressureDeltas, newWindDirections, precomputedNeighbours );
			}
			NativeMemory.Free( windPressureDeltas );
			NativeMemory.Free( newWindDirections );
			NativeMemory.Free( precomputedNeighbours );
		}
	}

	private unsafe void TweakWindDirectionAndDynamicPressurePass( WorldGenerationParameters parameters, GlobeModel globe, float* windPressureDeltas, Vector3<float>* newWindDirections, TileNeighbourPrecomputation* precomputedNeighbours ) {
		float diffusionWeight = 0.05f;      // general smoothing
		float advectionWeight = 0.175f;     // directional pressure‐driven flow
		int reservedThreads = 1; // reserve some threads for other tasks
		float invApproximateTileLength = .5f / (float) globe.ApproximateTileLength;
		uint faceCount = globe.FaceCount;

		ParallelProcessing.ForFaces( globe, reservedThreads, ( start, end, taskId ) => {
			uint offset = (uint) taskId * faceCount;
			float* localWindPressureDeltas = windPressureDeltas + offset;
			Unsafe.InitBlock( localWindPressureDeltas, 0, faceCount * sizeof( float ) );
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
				FaceState state = face.State;

				Vector3<float> baseWind = state.WindDirection;
				Vector3<float> localWind = default;

				float facePressure = state.DynamicPressure.Pascal + (state.BaseDynamicPressure.Pascal * 0.5f);

				int faceBaseIndex = i * 3;
				IReadOnlyList<Face> neighbours = face.Blueprint.Neighbours;
				//int neighbourCount = neighbours.Count;
				for (int n = 0; n < 3; n++) {
					Face neighbourFace = neighbours[ n ];
					FaceState neighbourState = neighbourFace.State;
					Vector3<float> dir = precomputedNeighbours[ faceBaseIndex + n ].Diff;
					float rise = facePressure - neighbourState.DynamicPressure.Pascal - (neighbourState.BaseDynamicPressure.Pascal * 0.5f);
					float slope = rise * invApproximateTileLength;

					float alignment = float.Max( dir.Dot( baseWind ), -0.25f );

					float pressureDelta = rise * ((advectionWeight * alignment) + diffusionWeight);
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
		ParallelProcessing.ForFaces( globe, reservedThreads, ( start, end, taskId ) => {
			IReadOnlyList<Face> faces = globe.Faces;
			for (int i = start; i < end; i++) {
				FaceState state = faces[ i ].State;
				float windPressure = state.DynamicPressure.Pascal;
				for (int j = 0; j < taskCount; j++)
					windPressure += windPressureDeltas[ (j * faceCount) + i ];
				state.SetDynamicPressure( windPressure );
				state.SetWindDirection( newWindDirections[ i ] );
			}
		} );
	}
}
