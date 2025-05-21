using Engine;
using Engine.Structures;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.World;

public sealed class Face : IOcTreeLeaf<float> {
	private readonly uint _id;
	private readonly FaceBlueprint _blueprint;
	private readonly FaceState _state;

	private bool _transmitChanges;
	public event Action<Face>? FaceStateChanged;

	public Face( GlobeModel globe, GlobeVertex[] faceVertices, uint id ) {
		this.Globe = globe;
		_id = id;
		_blueprint = new( this, faceVertices );
		_state = new FaceState( this );
		_transmitChanges = false;
		foreach (GlobeVertex vertex in faceVertices)
			vertex.AddFace( this );
	}

	public FaceBlueprint Blueprint => _blueprint;
	public FaceState State => _state;

	public uint Id => _id;

	public AABB<Vector3<float>> Bounds => _blueprint.Bounds;

	public GlobeModel Globe { get; }

	internal void TriggerFaceStateChanged() {
		if (!_transmitChanges)
			return;
		FaceStateChanged?.Invoke( this );
	}

	internal void SetTransmission( bool enabled ) => _transmitChanges = enabled;


	public override int GetHashCode() => unchecked((int)Id);

	public override bool Equals( object? obj ) => obj is Face face && Id == face.Id;

	//public static (Face, MutableFaceBlueprint) CreateFace( UnmanagedStaticContainer<PackedNormal> vectorContainer, FaceIndices face, uint id ) {

	//}
}
