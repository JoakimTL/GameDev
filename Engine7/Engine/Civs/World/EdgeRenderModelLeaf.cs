using Engine;
using Engine.Structures;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public sealed class EdgeRenderModelLeaf( UnmanagedStaticContainer<Vector3<float>> vectors, EdgeRenderModelWithId face ) : IOcTreeLeaf<float> {
	private readonly UnmanagedStaticContainer<Vector3<float>> _vectors = vectors;
	private readonly EdgeRenderModelWithId _edge = face;

	public uint Id => _edge.Id;

	public Vector3<float> VectorA => _vectors.Get( _edge.Model.IndexA );
	public Vector3<float> VectorB => _vectors.Get( _edge.Model.IndexB );

	public AABB<Vector3<float>> Bounds => AABB.Create( [ VectorA, VectorB ] );
}
