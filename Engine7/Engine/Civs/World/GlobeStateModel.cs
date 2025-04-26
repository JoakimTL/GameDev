//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

//TODO have the globes be generated using a separate module. That module will then communicate new and deleted globes and changes that occur on them such that all other modules can update their data. The globe module will really only update once a seldom terrain edit or something similar happens. Tile resources, cities, buildings, whatever else is handled by the logic module using tile ids.

namespace Civs.World;

public sealed class GlobeStateModel {

	public GlobeStateModel( Guid id, double radius, GlobeBlueprintModel renderModel ) {
		Id = id;
		Radius = radius;
		SurfaceArea = radius * radius * 4 * double.Pi;
		TileSurfaceArea = SurfaceArea / renderModel.FaceCount;
	}

	public Guid Id { get; }
	public double Radius { get; }
	public double SurfaceArea { get; }
	public double TileSurfaceArea { get; }

}

public sealed class GlobeModel {
	public Guid Id { get; }
	public GlobeStateModel State { get; }
	public GlobeBlueprintModel Blueprint { get; }

	private GlobeModel( Guid id, GlobeBlueprintModel blueprint, GlobeStateModel state ) {
		this.Id = id;
		this.Blueprint = blueprint;
		this.State = state;
	}

	public static GlobeModel Generate( WorldGenerationParameters parameters ) {
		Guid id = Guid.NewGuid();
		var blueprint = new GlobeBlueprintModel( id, parameters.Subdivisions );
		var state = new GlobeStateModel( id, parameters.GlobeRadius, blueprint );
		return new GlobeModel(
			id,
			blueprint,
			state
		);
	}
}
public sealed class WorldGenerationParameters {
	public WorldGenerationParameters( uint subdivisions, double globeRadius ) {
		if (subdivisions < 7)
			throw new ArgumentOutOfRangeException( nameof( subdivisions ), "Subdivision count must be at least 7." );
		if (subdivisions > 12)
			throw new ArgumentOutOfRangeException( nameof( subdivisions ), "Subdivision count must be at most 12." );
		if (globeRadius <= 1500000)
			throw new ArgumentOutOfRangeException( nameof( globeRadius ), "Radius must be greater than 1500000 meters." );
		Subdivisions = subdivisions;
		GlobeRadius = globeRadius;
	}

	public uint Subdivisions { get; }
	public double GlobeRadius { get; }
}
