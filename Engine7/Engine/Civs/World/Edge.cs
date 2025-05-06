using Engine;
using Engine.Structures;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public sealed class Edge : IOcTreeLeaf<float> {
	private readonly UnmanagedStaticContainer<PackedNormal> _vectorContainer;
	private readonly EdgeIndices _edge;
	private Connection? _connection;

	public Edge( UnmanagedStaticContainer<PackedNormal> vectorContainer, EdgeIndices edge ) {
		_vectorContainer = vectorContainer;
		this._edge = edge;
	}

	public Connection Connection => _connection ?? throw new InvalidOperationException( "No connection exists for this edge." );

	public Vector3<float> VertexA => _vectorContainer.Get( _edge.VertexA ).Decode();
	public Vector3<float> VertexB => _vectorContainer.Get( _edge.VertexB ).Decode();

	public static Vector3<float> GetNormal( Vector3<float> a, Vector3<float> b ) {
		Vector3<float> right = (b - a).Cross( -a ).Normalize<Vector3<float>, float>();
		Vector3<float> normal = (b - a).Cross( right ).Normalize<Vector3<float>, float>();
		return normal;
	}

	public AABB<Vector3<float>> Bounds => AABB.Create( [ VertexA, VertexB ] );

	internal void SetConnection( Connection connection ) {
		if (_connection is not null)
			throw new InvalidOperationException( "Connection already set." );
		_connection = connection;
	}

}
