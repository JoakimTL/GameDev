//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

using Engine;

namespace Civs.World;

public readonly struct FaceRenderModelWithIdAndVertices( uint id, FaceRenderModel model, Vector3<float> vertexA, Vector3<float> vertexB, Vector3<float> vertexC ) {
	public readonly uint Id = id;
	public readonly FaceRenderModel Model = model;
	public readonly Vector3<float> VertexA = vertexA;
	public readonly Vector3<float> VertexB = vertexB;
	public readonly Vector3<float> VertexC = vertexC;
}
