using Engine;

namespace Civlike.World.GameplayState;

public sealed class Globe {
	private readonly Vertex[] _vertices;
	private readonly Face[] _faces;
	private readonly BoundedRenderCluster[] _clusters;

	public Globe( Guid globeId, Vertex[] vertices, Face[] faces, BoundedRenderCluster[] clusters, double radius, double tileArea, double approximateTileLength) {
		this.Id = globeId;
		this._vertices = vertices;
		this._faces = faces;
		this._clusters = clusters;
		this.Radius = radius;
		this.TileArea = tileArea;
		this.ApproximateTileLength = approximateTileLength;
		if (clusters.Length > 0)
			this.ClusterBounds = Vector3<float>.Zero.CreateBounds( this._clusters[ 0 ].Bounds.GetLengths() * 0.5f );
	}

	public IReadOnlyList<Vertex> Vertices => this._vertices;
	public IReadOnlyList<Face> Faces => this._faces;
	public IReadOnlyList<BoundedRenderCluster> Clusters => this._clusters;
	public AABB<Vector3<float>> ClusterBounds { get; }
	public uint VertexCount => (uint) this._vertices.Length;
	public uint FaceCount => (uint) this._faces.Length;

	public Guid Id { get; }
	public double Radius { get;  }
	public double Area => 4 * double.Pi * this.Radius * this.Radius;
	public double TileArea { get; }
	public double ApproximateTileLength { get; }
}
