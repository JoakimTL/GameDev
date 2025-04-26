//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

/// <summary>
/// Contains the vector indices to render a line.
/// </summary>
public readonly struct EdgeRenderModel( uint indexA, uint indexB ) {
	public readonly uint IndexA = indexA;
	public readonly uint IndexB = indexB;
}
