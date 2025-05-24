//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.WorldOld;

public readonly struct FaceIndices( uint vertexA, uint vertexB, uint vertexC ) {
	public readonly uint VertexA = vertexA;
	public readonly uint VertexB = vertexB;
	public readonly uint VertexC = vertexC;
}
