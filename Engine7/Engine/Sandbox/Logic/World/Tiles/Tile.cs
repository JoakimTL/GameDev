using Sandbox.Logic.World.Tiles.Terrain;

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

	internal void SetTerrain( TerrainBase type ) {

	}
}



public sealed class TileDataModel {

}

public abstract class TileDataBase {
	
	public abstract string DataCode { get; }
	public abstract int DataCodeId { get; }
	public Tile Tile { get; private set; }

	protected TileDataBase() {
		Tile = null!;
	}

	internal void SetTile(Tile tile) {
		if (Tile is not null)
			throw new InvalidOperationException( "Tile already set." );
		Tile = tile;
	}
}

public sealed class TileData : TileDataBase {
	private const string _dataCode = "0000";
	private static readonly int _dataCodeId = _dataCode.ToIntCode();

	public override string DataCode => _dataCode;
	public override int DataCodeId => _dataCodeId;
	public TileData() : base() {
	}
}