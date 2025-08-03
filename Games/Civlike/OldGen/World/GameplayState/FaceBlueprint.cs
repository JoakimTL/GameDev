//using Engine;

//namespace OldGen.World.GameplayState;

//public sealed class FaceBlueprint( Face owner, Vertex[] vertices, Edge[] edges, Face[] neighbours ) {
//	private readonly Vertex[] _vertices = vertices;
//	private readonly Edge[] _edges = edges;
//	private readonly Face[] _neighbours = neighbours;

//	public Face Owner { get; } = owner;
//	public IReadOnlyList<Vertex> Vertices => this._vertices;
//	public IReadOnlyList<Edge> Edges => this._edges;
//	public IReadOnlyList<Face> Neighbours => this._neighbours;

//	public Vector3<float> VectorA => this._vertices[ 0 ].Vector;
//	public Vector3<float> VectorB => this._vertices[ 1 ].Vector;
//	public Vector3<float> VectorC => this._vertices[ 2 ].Vector;
//	public Vector3<float> DisplayVectorA => this._vertices[ 0 ].DisplayVector;
//	public Vector3<float> DisplayVectorB => this._vertices[ 1 ].DisplayVector;
//	public Vector3<float> DisplayVectorC => this._vertices[ 2 ].DisplayVector;

//	public AABB<Vector3<float>> Bounds => AABB.Create( [ this.VectorA, this.VectorB, this.VectorC ] );

//	public Vector3<float> GetCenter() => (this.VectorA + this.VectorB + this.VectorC) / 3f;
//}