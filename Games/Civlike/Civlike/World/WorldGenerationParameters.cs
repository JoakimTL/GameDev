//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

//TODO have the globes be generated using a separate module. That module will then communicate new and deleted globes and changes that occur on them such that all other modules can update their data. The globe module will really only update once a seldom terrain edit or something similar happens. Tile resources, cities, buildings, whatever else is handled by the logic module using tile ids.

namespace Civlike.World;

public sealed class WorldGenerationParameters {
	public WorldGenerationParameters( uint subdivisions, double globeRadius, int generationSeed, double sealevel, double baseHeightVariance, double plateHeight, double plateHeightVariance, double faultMaxHeight, double mountainHeight, double rotationRate, uint playerCount ) {
		if (subdivisions < 6)
			throw new ArgumentOutOfRangeException( nameof( subdivisions ), "Subdivision count must be at least 6." );
		if (subdivisions > 10)
			throw new ArgumentOutOfRangeException( nameof( subdivisions ), "Subdivision count must be at most 10." );
		if (globeRadius <= 1500000)
			throw new ArgumentOutOfRangeException( nameof( globeRadius ), "Radius must be greater than 1500000 meters." );
		Subdivisions = subdivisions;
		GlobeRadius = globeRadius;
		this.GenerationSeed = generationSeed;
		this.Sealevel = sealevel;
		this.FaultMaxHeight = faultMaxHeight;
		this.BaseHeightVariance = baseHeightVariance;
		this.PlateHeight = plateHeight;
		this.PlateHeightVariance = plateHeightVariance;
		this.MountainHeight = mountainHeight;
		this.RotationRate = rotationRate;
		this.PlayerCount = playerCount;
	}

	public uint Subdivisions { get; }
	public double GlobeRadius { get; }
	public int GenerationSeed { get; }
	public double Sealevel { get; }
	public double BaseHeightVariance { get; }
	public double PlateHeight { get; }
	public double PlateHeightVariance { get; }
	public double FaultMaxHeight { get; }
	public double MountainHeight { get; }
	public double RotationRate { get; }
	public uint PlayerCount { get; }
}
