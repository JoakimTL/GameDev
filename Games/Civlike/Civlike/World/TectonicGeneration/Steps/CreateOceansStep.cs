using Civlike.World.GenerationState;

namespace Civlike.World.TectonicGeneration.Steps;

[Engine.Processing.Do<IGenerationStep>.After<GenerateLandmassStep>]
public sealed class CreateOceansStep : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	public override string StepDisplayName => "Creating oceans";

	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		List<Face> belowSeaLevelFaces = globe.Faces.Where( f => f.Get<TectonicFaceState>().BaselineValues.ElevationMean < 0 ).ToList();

		Queue<Face> newOceanFaces = [];
		for (int i = 0; i < parameters.TectonicParameters.OceanSeeds; i++) {
			int index = globe.SeedProvider.Next( belowSeaLevelFaces.Count );
			Face face = belowSeaLevelFaces[ index ];
			newOceanFaces.Enqueue( face );
			face.IsOcean = true;
			face.IsLand = false;
		}

		while ( newOceanFaces.TryDequeue(out Face? oceanFace )) {
			foreach (Face neighbour in oceanFace.Neighbours) {
				if (neighbour.IsOcean || neighbour.Get<TectonicFaceState>().BaselineValues.ElevationMean >= 0)
					continue;
				neighbour.IsOcean = true;
				neighbour.IsLand = false;
				newOceanFaces.Enqueue( neighbour );
			}
		}
	}
}