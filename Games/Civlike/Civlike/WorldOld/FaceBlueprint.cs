using Engine;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civlike.WorldOld;

public sealed class FaceBlueprint {
	private readonly Face _face;
	private readonly Connection[] _connections;
	private readonly GlobeVertex[] _vertices;
	private Face[]? _neighbours;

	public FaceBlueprint( Face face, GlobeVertex[] vertices ) {
		if (vertices.Length != 3)
			throw new ArgumentException( "There must be exactly 3 vertices." );
		this._face = face;
		this._vertices = vertices;
		this._connections = new Connection[ 3 ];
		//_allNeighbours = [ .. _vertices.SelectMany( p => p.ConnectedFaces ).Distinct().Where( p => p != face ) ]; ConnectedFaces not initialized yet!
	}

	public IReadOnlyList<Connection> Connections => this._connections;
	public IReadOnlyList<GlobeVertex> Vertices => this._vertices;
	public IReadOnlyList<Face> Neighbours => this._neighbours ??= [ .. this._connections.Select( p => p.GetOther( this._face ) ) ];

	public Vector3<float> VectorA => this._vertices[ 0 ].Vector;
	public Vector3<float> VectorB => this._vertices[ 1 ].Vector;
	public Vector3<float> VectorC => this._vertices[ 2 ].Vector;

	public GlobeVertex GetVertex( uint index ) {
		if (index >= this._vertices.Length)
			throw new ArgumentOutOfRangeException( nameof( index ), "Index out of range." );
		return this._vertices[ index ];
	}

	public AABB<Vector3<float>> Bounds => AABB.Create( [ this.VectorA, this.VectorB, this.VectorC ] );

	internal void AddConnection( Connection connection ) {
		for (int i = 0; i < this._connections.Length; i++)
			if (this._connections[ i ] is null) {
				this._connections[ i ] = connection;
				return;
			}
		throw new InvalidOperationException( "No empty connection slot available." );
	}

	public Vector3<float> GetCenter() => (this.VectorA + this.VectorB + this.VectorC) / 3f;

	public Face GetFaceInDirection(Vector3<float> normalizedDirection) {
		Vector3<float> center = GetCenter();
		float maxDot = float.MinValue;
		Face bestMatch = null!;
		foreach (Connection connection in this._connections) {
			Face face = connection.GetOther( this._face );
			Vector3<float> diff = face.Blueprint.GetCenter() - center;
			float dot = diff.Dot( normalizedDirection );
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