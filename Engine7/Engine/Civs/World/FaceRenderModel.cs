//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

/// <summary>
/// Contains the vector indices to render a triangle face for a corresponding tile.
/// </summary>
public readonly struct FaceRenderModel( uint indexA, uint indexB, uint indexC ) {
	public readonly uint IndexA = indexA;
	public readonly uint IndexB = indexB;
	public readonly uint IndexC = indexC;
}
