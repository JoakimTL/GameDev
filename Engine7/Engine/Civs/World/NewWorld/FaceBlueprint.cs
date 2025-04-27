using Engine;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World.NewWorld;

public sealed class FaceBlueprint {
	private readonly UnmanagedStaticContainer<PackedNormal> _vectorContainer;
	private readonly Connection[] _connections;

	public FaceBlueprint( UnmanagedStaticContainer<PackedNormal> vectorContainer, FaceIndices face ) {
		_vectorContainer = vectorContainer;
		this.Indices = face;
		_connections = new Connection[ 3 ];
	}

	public readonly FaceIndices Indices;

	public IReadOnlyList<Connection> Connections => _connections;

	public Vector3<float> VertexA => _vectorContainer.Get( Indices.VertexA ).Decode();
	public Vector3<float> VertexB => _vectorContainer.Get( Indices.VertexB ).Decode();
	public Vector3<float> VertexC => _vectorContainer.Get( Indices.VertexC ).Decode();

	public AABB<Vector3<float>> Bounds => AABB.Create( [ VertexA, VertexB, VertexC ] );

	internal void AddConnection( Connection connection ) {
		for (int i = 0; i < _connections.Length; i++)
			if (_connections[ i ] is null) {
				_connections[ i ] = connection;
				return;
			}
		throw new InvalidOperationException( "No empty connection slot available." );
	}
}

//public sealed class MutableFaceBlueprint {
//	private readonly Action<Connection> _addConnectionDelegate;

//	public MutableFaceBlueprint( Action<Connection> addConnectionDelegate ) {
//		_addConnectionDelegate = addConnectionDelegate;
//	}

//	public void AddConnection( Connection connection ) {
//		_addConnectionDelegate( connection );
//	}
//}
