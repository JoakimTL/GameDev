//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

using Engine;

namespace Civlike.WorldOld.Generation.Steps;

public sealed class WindSmoothingStep : ITerrainGenerationProcessingStep {
	public string ProcessingStepMessage => "Smoothing winds";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		Vector3<float>[] newWindDirections = new Vector3<float>[ globe.FaceCount ];

		for (int i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ i ];

			Vector3<float> newWindDirection = face.State.WindDirection * 12;
			for (int ni = 0; ni < 3; ni++) {
				Face neighbour = face.Blueprint.Neighbours[ ni ];
				newWindDirection += neighbour.State.WindDirection;
			}
			newWindDirections[i] = newWindDirection.Normalize<Vector3<float>, float>();
		}

		for (int i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ i ];
			Vector3<float> newWindDirection = newWindDirections[ i ];
			face.State.SetWindDirection( newWindDirection );
		}
	}
}