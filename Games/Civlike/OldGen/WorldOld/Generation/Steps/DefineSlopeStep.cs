//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

using Engine;

namespace OldGen.WorldOld.Generation.Steps;

public sealed class DefineSlopeStep : ITerrainGenerationProcessingStep {
	public string ProcessingStepMessage => "Defining slopes";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;
			Vector3<float> gradient = 0;
			for (int ni = 0; ni < 3; ni++) {
				Face neighbour = face.Blueprint.Neighbours[ ni ];
				Vector3<float> diff = (neighbour.Blueprint.GetCenter() - center).Normalize<Vector3<float>, float>();
				float heightDiff = neighbour.State.Height - face.State.Height;
				gradient += diff * (heightDiff / (float) globe.ApproximateTileLength);
			}
			face.State.SetGradient( gradient );
		}
	}
}
