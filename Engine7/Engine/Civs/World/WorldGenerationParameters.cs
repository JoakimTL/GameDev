//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

//TODO have the globes be generated using a separate module. That module will then communicate new and deleted globes and changes that occur on them such that all other modules can update their data. The globe module will really only update once a seldom terrain edit or something similar happens. Tile resources, cities, buildings, whatever else is handled by the logic module using tile ids.

namespace Civs.World;

public sealed class WorldGenerationParameters {
	public WorldGenerationParameters( uint subdivisions, double globeRadius, int generationSeed, double maxTerrainHeight, double sealevel ) {
		if (subdivisions < 6)
			throw new ArgumentOutOfRangeException( nameof( subdivisions ), "Subdivision count must be at least 6." );
		if (subdivisions > 10)
			throw new ArgumentOutOfRangeException( nameof( subdivisions ), "Subdivision count must be at most 10." );
		if (globeRadius <= 1500000)
			throw new ArgumentOutOfRangeException( nameof( globeRadius ), "Radius must be greater than 1500000 meters." );
		if (maxTerrainHeight <= sealevel) {
			throw new ArgumentOutOfRangeException( nameof( maxTerrainHeight ), "Max terrain height must be greater than sealevel." );
		}
		Subdivisions = subdivisions;
		GlobeRadius = globeRadius;
		this.GenerationSeed = generationSeed;
		this.MaxTerrainHeight = maxTerrainHeight;
		this.Sealevel = sealevel;
	}

	public uint Subdivisions { get; }
	public double GlobeRadius { get; }
	public int GenerationSeed { get; }
	public double MaxTerrainHeight { get; }
	public double Sealevel { get; }
}
