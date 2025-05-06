using Engine;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public sealed class FaceBlueprint {
	private readonly Connection[] _connections;
	private readonly GlobeVertex[] _vertices;

	public FaceBlueprint( GlobeVertex[] vertices ) {
		if (vertices.Length != 3)
			throw new ArgumentException( "There must be exactly 3 vertices." );
		_vertices = vertices;
		_connections = new Connection[ 3 ];
	}

	public IReadOnlyList<Connection> Connections => _connections;

	public Vector3<float> VectorA => _vertices[ 0 ].Vector;
	public Vector3<float> VectorB => _vertices[ 1 ].Vector;
	public Vector3<float> VectorC => _vertices[ 2 ].Vector;

	public GlobeVertex GetVertex( uint index ) {
		if (index >= _vertices.Length)
			throw new ArgumentOutOfRangeException( nameof( index ), "Index out of range." );
		return _vertices[ index ];
	}

	public AABB<Vector3<float>> Bounds => AABB.Create( [ VectorA, VectorB, VectorC ] );

	internal void AddConnection( Connection connection ) {
		for (int i = 0; i < _connections.Length; i++)
			if (_connections[ i ] is null) {
				_connections[ i ] = connection;
				return;
			}
		throw new InvalidOperationException( "No empty connection slot available." );
	}

	public Vector3<float> GetCenter() => (VectorA + VectorB + VectorC) / 3f;
}