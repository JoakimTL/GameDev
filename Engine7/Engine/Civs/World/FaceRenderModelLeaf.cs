using Engine;
using Engine.Structures;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public sealed class FaceRenderModelLeaf( UnmanagedStaticContainer<Vector3<float>> vectors, FaceRenderModelWithId face ) : IOcTreeLeaf<float> {
	private readonly UnmanagedStaticContainer<Vector3<float>> _vectors = vectors;
	private readonly FaceRenderModelWithId _face = face;

	public uint Id => _face.Id;

	public Vector3<float> VectorA => _vectors.Get( _face.Model.IndexA );
	public Vector3<float> VectorB => _vectors.Get( _face.Model.IndexB );
	public Vector3<float> VectorC => _vectors.Get( _face.Model.IndexC );

	public AABB<Vector3<float>> Bounds => AABB.Create( [ VectorA, VectorB, VectorC ] );
}
