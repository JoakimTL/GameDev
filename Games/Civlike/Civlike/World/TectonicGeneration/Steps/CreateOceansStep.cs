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
			foreach (NeighbouringFace neighbour in oceanFace.Neighbours) {
				Face<TectonicFaceState> neighbourFace = neighbour.Face as Face<TectonicFaceState> ?? throw new InvalidCastException( $"Neighbouring face {neighbour.Face.Id} is not of type TectonicFaceState." );
				if (neighbourFace.IsOcean || neighbourFace.State.BaselineValues.ElevationMean >= 0)
					continue;
				neighbourFace.IsOcean = true;
				neighbourFace.IsLand = false;
				newOceanFaces.Enqueue( neighbourFace );
			}
		}
	}
}