using Civlike.World.GenerationState;

namespace Civlike.World.TectonicGeneration.Steps;

[Engine.Processing.Do<IGenerationStep>.After<GenerateLandmassStep>]
public sealed class CreateOceansStep : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	public override string StepDisplayName => "Creating oceans";

	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		List<Face<TectonicFaceState>> belowSeaLevelFaces = globe.Faces.OfType<Face<TectonicFaceState>>().Where( f => f.State.BaselineValues.ElevationMean < 0 ).ToList();

		Queue<Face<TectonicFaceState>> newOceanFaces = [];
		for (int i = 0; i < parameters.TectonicParameters.OceanSeeds; i++) {
			int index = globe.SeedProvider.Next( belowSeaLevelFaces.Count );
			Face<TectonicFaceState> face = belowSeaLevelFaces[ index ];
			newOceanFaces.Enqueue( face );
			face.IsOcean = true;
			face.IsLand = false;
		}

		while ( newOceanFaces.TryDequeue(out Face<TectonicFaceState>? oceanFace )) {
			foreach (Face<TectonicFaceState> neighbour in oceanFace.Neighbours) {
				if (neighbour.IsOcean || neighbour.State.BaselineValues.ElevationMean >= 0)
					continue;
				neighbour.IsOcean = true;
				neighbour.IsLand = false;
				newOceanFaces.Enqueue( neighbour );
			}
		}
	}
}