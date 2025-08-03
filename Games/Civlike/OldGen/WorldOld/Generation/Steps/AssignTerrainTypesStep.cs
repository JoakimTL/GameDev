using Engine;
using OldGen.WorldOld.TerrainTypes;

namespace OldGen.WorldOld.Generation.Steps;

public sealed class AssignTerrainTypesStep : ITerrainGenerationProcessingStep {
	public string ProcessingStepMessage => "Assigning terrain types";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		GrasslandTerrain grasslandTerrain = TerrainTypeList.GetTerrainType<GrasslandTerrain>();
		OceanTerrain oceanTerrain = TerrainTypeList.GetTerrainType<OceanTerrain>();
		MountainTerrain mountainTerrain = TerrainTypeList.GetTerrainType<MountainTerrain>();
		ShorelineTerrain shorelineTerrain = TerrainTypeList.GetTerrainType<ShorelineTerrain>();
		FrozenDesertTerrain frozenDesertTerrain = TerrainTypeList.GetTerrainType<FrozenDesertTerrain>();
		WarmDesertTerrain warmDesertTerrain = TerrainTypeList.GetTerrainType<WarmDesertTerrain>();
		RainforestTerrain rainforestTerrain = TerrainTypeList.GetTerrainType<RainforestTerrain>();
		ForestTerrain forestTerrain = TerrainTypeList.GetTerrainType<ForestTerrain>();
		BoglandTerrain boglandTerrain = TerrainTypeList.GetTerrainType<BoglandTerrain>();

		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			if (face.State.Height < 0) {
				if (face.State.Height < -250) {
					face.State.SetTerrainType( oceanTerrain );
					continue;
				}
				face.State.SetTerrainType( shorelineTerrain );
				continue;
			}

			if (face.State.Ruggedness * face.State.LocalRelief > 300 || face.State.Gradient.Magnitude<Vector3<float>, float>() > 0.25f) {
				face.State.SetTerrainType( mountainTerrain );
				continue;
			}

			if (face.State.PrecipitationMm < 50) {
				if (face.State.Temperature.Celsius < 0) {
					face.State.SetTerrainType( frozenDesertTerrain );
					continue;
				}
				face.State.SetTerrainType( warmDesertTerrain );
				continue;
			}

			if (face.State.PrecipitationMm > 200 && face.State.Ruggedness * face.State.LocalRelief < 5) {
				face.State.SetTerrainType( boglandTerrain );
				continue;
			}

			if (face.State.PrecipitationMm > 3000) {
				//Need a way to define where vegetation should be!
				face.State.SetTerrainType( rainforestTerrain );
				continue;
			}

			face.State.SetTerrainType( grasslandTerrain );
			//if (face.State.PrecipitationMm > 1000) {
			//	face.State.SetTerrainType( forestTerrain );
			//	continue;
			//}
		}
	}
}