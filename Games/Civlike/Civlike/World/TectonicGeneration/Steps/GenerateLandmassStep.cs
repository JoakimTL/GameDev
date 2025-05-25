using Civlike.World.GenerationState;
using Civlike.World.GenerationState.Steps;
using Civlike.World.NoiseProviders;
using Engine;

namespace Civlike.World.TectonicGeneration.Steps;

[Engine.Processing.Do<IGenerationStep>.After<CreateFacesStep>]
public sealed class GenerateLandmassStep : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	public override string StepDisplayName => "Generating landmasses";

	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		TectonicPlateGenerator tectonicRegionGenerator = new( new( globe.SeedProvider.Next() ), parameters.TectonicParameters.PlateCountBase + (int) float.Round( (globe.SeedProvider.NextSingle() * 2 - 1) * parameters.TectonicParameters.PlateCountVariance ), 0.01f, (float) (parameters.TectonicParameters.PlateHeight - parameters.TectonicParameters.PlateHeightVariance), (float) (parameters.TectonicParameters.PlateHeight + parameters.TectonicParameters.PlateHeightVariance) );
		Noise3 xShiftNoise = new( globe.SeedProvider.Next(), 11 );
		Noise3 yShiftNoise = new( globe.SeedProvider.Next(), 11 );
		Noise3 zShiftNoise = new( globe.SeedProvider.Next(), 11 );

		uint baseCoarseSeed = unchecked((uint) globe.SeedProvider.Next());
		float baseCoarseScale = 26;
		uint baseFineSeed = unchecked((uint) globe.SeedProvider.Next());
		float baseFineScale = 79;

		Noise3 coarseFaultNoise = new( globe.SeedProvider.Next(), 7 );
		Noise3 fineFaultNoise = new( globe.SeedProvider.Next(), 27 );

		Noise3 mountainExistingNoise = new( globe.SeedProvider.Next(), 14 );
		FiniteVoronoiNoise3 mountainRidgeNoise = new( new( globe.SeedProvider.Next() ), 0.03125f, 1 );

		Noise3 coarseRuggednessNoise = new( globe.SeedProvider.Next(), 4 );
		Noise3 fineRuggednessNoise = new( globe.SeedProvider.Next(), 19 );

		float[] seismicActivity = new float[ globe.Vertices.Count ];
		float[] ruggedness = new float[ globe.Vertices.Count ];
		ParallelProcessing.Range( globe.Vertices.Count, ( start, end, taskId ) => {
			List<(TectonicPlate plate, float gradient)> neighbourPlates = [];
			for (int i = start; i < end; i++) {
				Vertex vertex = globe.Vertices[ i ];
				Vector3<float> point = vertex.Vector;

				Vector3<float> shift = (new Vector3<float>( xShiftNoise.Noise( point ), yShiftNoise.Noise( point ), zShiftNoise.Noise( point ) ) * 2) - 1;
				Vector3<float> translation = point + (shift * 0.05f);

				TectonicPlate current = tectonicRegionGenerator.Get( translation, neighbourPlates, 23, 0.0001f );
				float localRuggedness = (coarseRuggednessNoise.Noise( point ) * 0.65f) + (fineRuggednessNoise.Noise( point ) * 0.35f);
				float currentHeight = current.Height + (((((Noise3.Noise( baseCoarseSeed + (uint) current.Id, baseCoarseScale, point ) * 0.65f) + (Noise3.Noise( baseFineSeed + (uint) current.Id, baseFineScale, point ) * 0.35f)) * 2) - 1) * (float) parameters.TectonicParameters.BaseHeightVariance);

				float otherAverageHeight = 0;
				float faultHeight = 0;
				float faultIntensity = 0;
				for (int n = 0; n < neighbourPlates.Count; n++) {
					(TectonicPlate other, float gradient) = neighbourPlates[ n ];
					//float gradientSq = gradient * gradient;

					float otherVariableHeight = ((((Noise3.Noise( baseCoarseSeed + (uint) other.Id, baseCoarseScale, point ) * 0.7f) + (Noise3.Noise( baseFineSeed + (uint) other.Id, baseFineScale, point ) * 0.3f)) * 2) - 1) * (float) parameters.TectonicParameters.BaseHeightVariance;
					otherAverageHeight += (other.Height + otherVariableHeight) * gradient;

					float faultMovement = current.GetFaultMovementDifference( other );
					float faultPresence = (coarseFaultNoise.Noise( point ) * 0.6f) + (fineFaultNoise.Noise( point ) * 0.4f);
					faultHeight += faultMovement * (float) parameters.TectonicParameters.FaultMaxHeight * faultPresence * gradient;

					faultIntensity += current.GetFaultReactionIntensity( other ) * float.Sqrt( gradient );
				}

				float mountainFactor = mountainRidgeNoise.BorderNoise( translation, 12 ) * mountainExistingNoise.Noise( point );
				float mountainHeight = mountainFactor * mountainFactor * (float) parameters.TectonicParameters.MountainHeight;

				float nHeight = currentHeight + otherAverageHeight + faultHeight + mountainHeight;

				vertex.Height = nHeight;
				seismicActivity[ vertex.Id ] = faultIntensity;
				ruggedness[ vertex.Id ] = localRuggedness;
			}
		} );

		ParallelProcessing.Range( globe.Faces.Count, ( start, end, taskId ) => {
			List<(TectonicPlate plate, float gradient)> neighbourPlates = [];
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.Faces[ i ] as Face<TectonicFaceState> ?? throw new InvalidCastException( $"Face at index {i} is not of type TectonicFaceState." );
				TectonicFaceState state = face.State;

				float height = 0;
				float faceSeismicActivity = 0;
				float faceRuggedness = 0;

				foreach (Vertex vertex in face.Vertices) {
					height += vertex.Height;
					faceSeismicActivity += seismicActivity[ vertex.Id ];
					faceRuggedness += ruggedness[ vertex.Id ];
				}

				height /= 3;
				faceSeismicActivity /= 3;
				faceRuggedness /= 3;

				state.BaselineValues.ElevationMean = height;
				state.BaselineValues.SeismicActivity = faceSeismicActivity;
				state.BaselineValues.RuggednessFactor = faceRuggedness;

				float stddev = 0;
				foreach (Vertex vertex in face.Vertices) {
					float heightDifference = vertex.Height - height;
					stddev += heightDifference * heightDifference;
				}
				stddev = float.Sqrt( stddev / 3 );

				state.BaselineValues.ElevationStandardDeviation = stddev;
			}
		} );
	}
}
