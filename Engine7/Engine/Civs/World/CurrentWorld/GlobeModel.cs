////TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

////TODO have the globes be generated using a separate module. That module will then communicate new and deleted globes and changes that occur on them such that all other modules can update their data. The globe module will really only update once a seldom terrain edit or something similar happens. Tile resources, cities, buildings, whatever else is handled by the logic module using tile ids.


////TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

////TODO have the globes be generated using a separate module. That module will then communicate new and deleted globes and changes that occur on them such that all other modules can update their data. The globe module will really only update once a seldom terrain edit or something similar happens. Tile resources, cities, buildings, whatever else is handled by the logic module using tile ids.

//namespace Civs.World.CurrentWorld;

//public sealed class GlobeModel {
//	public Guid Id { get; }
//	public GlobeStateModel State { get; }
//	public GlobeBlueprintModel Blueprint { get; }

//	private GlobeModel( Guid id, GlobeBlueprintModel blueprint, GlobeStateModel state ) {
//		this.Id = id;
//		this.Blueprint = blueprint;
//		this.State = state;
//	}

//	public static GlobeModel Generate( WorldGenerationParameters parameters ) {
//		Guid id = Guid.NewGuid();
//		var blueprint = new GlobeBlueprintModel( id, parameters.Subdivisions );
//		var state = new GlobeStateModel( id, parameters.GlobeRadius, parameters.GenerationSeed, parameters.MaxTerrainHeight, parameters.Sealevel, blueprint );
//		return new GlobeModel(
//			id,
//			blueprint,
//			state
//		);
//	}
//}
