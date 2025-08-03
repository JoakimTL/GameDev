//TODO have a generationglobe and a usable globe. The separation between what is supposed to be used during gameplay and what is supposed to be only used during generation needs to be separated. The globe during generation has a lot more information per tile than during gameplay, at least initially

namespace OldGen.WorldOld;

public interface IWorldTerrainGenerator {
	void GenerateTerrain( WorldGenerationParameters parameters, GlobeModel globe );
}
