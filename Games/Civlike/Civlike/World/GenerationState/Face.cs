using Engine;

namespace Civlike.World.GenerationState;

public sealed class Face {
	private readonly Vertex[] _vertices;
	private readonly Edge[] _edges;
	private Face[]? _neighbours;
	private readonly Dictionary<Type, object> _properties;

	public Face( uint id, Vertex[] vertices, Edge[] edges ) {
		this.Id = id;
		this._vertices = vertices;
		this._edges = edges;
		_properties = [];
		foreach (Vertex vertex in vertices)
			vertex.AddFace( this );
		foreach (Edge edge in edges)
			edge.AddFace( this );
		this.TerrainType = TerrainTypeList.Default;
	}

	public uint Id { get; }
	public IReadOnlyList<Vertex> Vertices => this._vertices;
	public IReadOnlyList<Edge> Edges => this._edges;
	public IReadOnlyList<Face> Neighbours => this._neighbours ??= [ .. this._edges.Select( p => p.GetOther( this ) ) ];
	public TerrainTypeBase TerrainType { get; set; }

	public Vector3<float> VectorA => this._vertices[ 0 ].Vector;
	public Vector3<float> VectorB => this._vertices[ 1 ].Vector;
	public Vector3<float> VectorC => this._vertices[ 2 ].Vector;
	public Vector3<float> GetCenter() => (this.VectorA + this.VectorB + this.VectorC) / 3f;

	public T Get<T>() where T : new() {
		if (this._properties.TryGetValue( typeof( T ), out object? value ))
			return (T) value;
		T newValue = new();
		this._properties[ typeof( T ) ] = newValue;
		return newValue;
	}

	public override bool Equals( object? obj ) => obj is Face vertex && vertex == this;
	public override int GetHashCode() => this.Id.GetHashCode();

	public static bool operator ==( Face left, Face right ) => left.Id == right.Id;
	public static bool operator !=( Face left, Face right ) => !(left == right);

}