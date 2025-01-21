namespace Sandbox.Logic.World.Tiles;

public sealed class Tile : IOcTreeLeaf<float> {
	public GlobeComponent Globe { get; }
	public readonly TileRenderModel RenderModel;
	private readonly TileNeighboursModel _neighbours;

	public Tile( GlobeComponent globe, TriangleIndices indices, Vector4<float> color ) {
		this.Globe = globe;
		this.RenderModel = new( this, indices, color );
		this._neighbours = new( this );
		Area = RenderModel.Area * Globe.GeneratedSurfaceArea / (4 * double.Pi);
	}

	public IReadOnlyList<Edge> Edges => _neighbours.Edges;
	public IReadOnlyList<Tile> Neighbours => _neighbours.GetNeighbours();
	public AABB<Vector3<float>> Bounds => RenderModel.GetBounds();
	public double Area { get; }

	internal void AddEdge( Edge edge ) {
		_neighbours.AddEdge( edge );
	}
}
