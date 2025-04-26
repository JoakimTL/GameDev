//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public readonly struct EdgeRenderModelWithId( uint id, EdgeRenderModel model ) {
	public readonly uint Id = id;
	public readonly EdgeRenderModel Model = model;
}
