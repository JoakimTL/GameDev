using Engine;
using Engine.Structures;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.World;

public sealed class Edge : IOcTreeLeaf<float> {
	private readonly GlobeVertex _vertexA;
	private readonly GlobeVertex _vertexB;
	private Connection? _connection;

	public Edge( GlobeVertex vertexA, GlobeVertex vertexB ) {
		this._vertexA = vertexA;
		this._vertexB = vertexB;
	}

	public Connection Connection => _connection ?? throw new InvalidOperationException( "No connection exists for this edge." );

	public Vector3<float> VertexA => _vertexA.Vector;
	public Vector3<float> VertexB => _vertexB.Vector;

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
