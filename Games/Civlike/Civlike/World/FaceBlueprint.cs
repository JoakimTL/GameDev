using Engine;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.World;

public sealed class FaceBlueprint {
	private readonly Face _face;
	private readonly Connection[] _connections;
	private readonly GlobeVertex[] _vertices;
	//private readonly Face[] _allNeighbours;

	public FaceBlueprint( Face face, GlobeVertex[] vertices ) {
		if (vertices.Length != 3)
			throw new ArgumentException( "There must be exactly 3 vertices." );
		_face = face;
		_vertices = vertices;
		_connections = new Connection[ 3 ];
		//_allNeighbours = [ .. _vertices.SelectMany( p => p.ConnectedFaces ).Distinct().Where( p => p != face ) ]; ConnectedFaces not initialized yet!
	}

	public IReadOnlyList<Connection> Connections => _connections;
	public IReadOnlyList<GlobeVertex> Vertices => _vertices;
	//public IReadOnlyList<Face> AllNeighbours => _allNeighbours;

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

	public Face GetFaceInDirection(Vector3<float> normalizedDirection) {
		var center = GetCenter();
		float maxDot = float.MinValue;
		Face bestMatch = null!;
		foreach (var connection in _connections) {
			var face = connection.GetOther( _face );
			var diff = face.Blueprint.GetCenter() - center;
			var dot = diff.Dot( normalizedDirection );
			if (dot > maxDot) {
				maxDot = dot;
				if (dot > 0.99f)
					return face;
				bestMatch = face;
			}
		}
		if (bestMatch is null)
			throw new InvalidOperationException( "No face found in the given direction." );
		return bestMatch;
	}
}