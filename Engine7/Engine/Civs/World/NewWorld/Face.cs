using Civs.World.TerrainTypes;
using Engine;
using Engine.Structures;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World.NewWorld;

public sealed class Face : IOcTreeLeaf<float> {
	private readonly uint _id;
	private readonly FaceBlueprint _blueprint;
	private readonly FaceState _state;

	private bool _transmitChanges;
	public event Action<Face>? FaceStateChanged;

	public Face( UnmanagedStaticContainer<PackedNormal> vectorContainer, FaceIndices face, uint id ) {
		_id = id;
		_blueprint = new( vectorContainer, face );
		_state = new FaceState( this, TerrainTypeList.Default.Id );
		_transmitChanges = false;
	}

	public FaceBlueprint Blueprint => _blueprint;
	public FaceState State => _state;

	public uint Id => _id;

	public AABB<Vector3<float>> Bounds => _blueprint.Bounds;

	internal void TriggerFaceStateChanged() {
		if (!_transmitChanges)
			return;
		FaceStateChanged?.Invoke( this );
	}

	internal void SetTransmission( bool enabled ) => _transmitChanges = enabled;

	//public static (Face, MutableFaceBlueprint) CreateFace( UnmanagedStaticContainer<PackedNormal> vectorContainer, FaceIndices face, uint id ) {

	//}
}
