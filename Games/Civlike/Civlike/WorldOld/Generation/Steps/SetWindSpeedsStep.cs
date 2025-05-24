//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

using Engine;

namespace Civlike.WorldOld.Generation.Steps;

public sealed class SetWindSpeedsStep : ITerrainGenerationProcessingStep {
	public string ProcessingStepMessage => "Setting wind speed";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		float pgfFactor = 0.0001f;
		float rougnessDampK = 0.5f;

		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;
			Vector3<float> pressureGradient = 0;
			for (int ni = 0; ni < 3; ni++) {
				Face neighbour = face.Blueprint.Neighbours[ ni ];
				Vector3<float> diff = (neighbour.Blueprint.GetCenter() - center).Normalize<Vector3<float>, float>();
				float pressureDiff = neighbour.State.DynamicPressure.Pascal - face.State.DynamicPressure.Pascal;
				pressureGradient += diff * (pressureDiff / (float) globe.ApproximateTileLength);
			}
			pressureGradient /= 3;

			float windSpeed = face.State.WindSpeed;
			float windPressure = face.State.DynamicPressure;

			float pgfMag = pressureGradient.Magnitude<Vector3<float>, float>();
			float pgSpeed = pgfMag * pgfFactor;

			float speed = windSpeed + pgSpeed;

			speed *= 1 - (rougnessDampK * float.Max( face.State.Ruggedness * face.State.LocalPressureRelief * 0.01f, 1 ));

			face.State.SetWindSpeed( float.Clamp( speed, 0, 30 ) );
		}
	}
}
