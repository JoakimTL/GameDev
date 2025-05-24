//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.


//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.WorldOld.Generation.Steps;

public sealed class SetEvaporationRateStep : ITerrainGenerationProcessingStep {
	public string ProcessingStepMessage => "Setting evaporation rates";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		float E_max = 2000f; // mm/yr at the hottest, windiest equatorial tile
		float T_min = -10f;  // °C below which we assume almost no evaporation
		float T_max = 30f;   // °C at which we reach E_max
		//float windRef = 10f; // m/s – reference wind speed for a +50% boost

		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			FaceState state = face.State;
			if (state.Height >= 0)
				continue;

			float temperature = state.Temperature.Celsius;

			float fT = float.Max( (temperature - T_min) / (T_max - T_min), 0 );

			float fW = 1;

			state.SetEvaporationMm( E_max * fT * fW );
		}
	}
}
