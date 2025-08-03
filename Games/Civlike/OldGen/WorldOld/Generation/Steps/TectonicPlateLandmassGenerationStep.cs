using Engine;
using OldGen.WorldOld.Generation.NoiseProviders;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace OldGen.WorldOld.Generation.Steps;
public sealed class TectonicPlateLandmassGenerationStep( Random seedProvider, int plateCount ) : ITerrainGenerationProcessingStep {
	public readonly Random SeedProvider = seedProvider;
	public readonly int PlateCount = plateCount;

	public string ProcessingStepMessage => "Generating tectonic plates";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		TectonicPlateGenerator tectonicRegionGenerator = new( new( this.SeedProvider.Next() ), this.PlateCount, 0.01f, (float) (parameters.PlateHeight - parameters.PlateHeightVariance), (float) (parameters.PlateHeight + parameters.PlateHeightVariance) );
		Noise3 xShiftNoise = new( this.SeedProvider.Next(), 11 );
		Noise3 yShiftNoise = new( this.SeedProvider.Next(), 11 );
		Noise3 zShiftNoise = new( this.SeedProvider.Next(), 11 );

		uint baseCoarseSeed = unchecked((uint) this.SeedProvider.Next());
		float baseCoarseScale = 26;
		uint baseFineSeed = unchecked((uint) this.SeedProvider.Next());
		float baseFineScale = 79;

		Noise3 coarseFaultNoise = new( this.SeedProvider.Next(), 7 );
		Noise3 fineFaultNoise = new( this.SeedProvider.Next(), 27 );

		Noise3 mountainExistingNoise = new( this.SeedProvider.Next(), 14 );
		FiniteVoronoiNoise3 mountainRidgeNoise = new( new( this.SeedProvider.Next() ), 0.03125f, 1 );

		Noise3 coarseRuggednessNoise = new( this.SeedProvider.Next(), 4 );
		Noise3 fineRuggednessNoise = new( this.SeedProvider.Next(), 19 );

		ParallelProcessing.ForFaces( globe, 1, ( start, end, taskId ) => {
			List<(TectonicPlate plate, float gradient)> neighbourPlates = [];
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
				Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;

				Vector3<float> shift = new Vector3<float>( xShiftNoise.Noise( center ), yShiftNoise.Noise( center ), zShiftNoise.Noise( center ) ) * 2 - 1;
				Vector3<float> translation = center + shift * 0.05f;

				TectonicPlate current = tectonicRegionGenerator.Get( translation, neighbourPlates, 23, 0.0001f );
				float ruggedness = coarseRuggednessNoise.Noise( center ) * 0.65f + fineRuggednessNoise.Noise( center ) * 0.35f;
				float currentHeight = current.Height + ((Noise3.Noise( baseCoarseSeed + (uint) current.Id, baseCoarseScale, center ) * 0.65f + Noise3.Noise( baseFineSeed + (uint) current.Id, baseFineScale, center ) * 0.35f) * 2 - 1) * (float) parameters.BaseHeightVariance;

				float otherAverageHeight = 0;
				float faultHeight = 0;
				float faultIntensity = 0;
				for (int n = 0; n < neighbourPlates.Count; n++) {
					(TectonicPlate other, float gradient) = neighbourPlates[ n ];
					//float gradientSq = gradient * gradient;

					float otherVariableHeight = ((Noise3.Noise( baseCoarseSeed + (uint) other.Id, baseCoarseScale, center ) * 0.7f + Noise3.Noise( baseFineSeed + (uint) other.Id, baseFineScale, center ) * 0.3f) * 2 - 1) * (float) parameters.BaseHeightVariance;
					otherAverageHeight += (other.Height + otherVariableHeight) * gradient;

					float faultMovement = current.GetFaultMovementDifference( other );
					float faultPresence = coarseFaultNoise.Noise( center ) * 0.6f + fineFaultNoise.Noise( center ) * 0.4f;
					faultHeight += faultMovement * (float) parameters.FaultMaxHeight * faultPresence * gradient;

					faultIntensity += current.GetFaultReactionIntensity( other ) * float.Sqrt( gradient );
				}

				float mountainFactor = mountainRidgeNoise.BorderNoise( translation, 12 ) * mountainExistingNoise.Noise( center );
				float mountainHeight = mountainFactor * mountainFactor * (float) parameters.MountainHeight;

				float nHeight = currentHeight + otherAverageHeight + faultHeight + mountainHeight;

				face.State.SetHeight( nHeight );

				face.State.SetSeismicActivity( faultIntensity );

				face.State.SetRuggedness( coarseRuggednessNoise.Noise( center ) * 0.55f + fineRuggednessNoise.Noise( center ) * 0.45f );
			}
		} );
	}
}
