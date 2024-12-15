namespace Sandbox.Logic.World;

public sealed class BaseTile {
	public uint VectorIndexA { get; }
	public uint VectorIndexB { get; }
	public uint VectorIndexC { get; }
	public int Layer { get; }
	public int Sublayers { get; }

	public Vector4<double> Color => GetColor();

	private Vector4<double> GetColor() {
		Vector4<double> color = Vector4<double>.AdditiveIdentity;
		if (_subTiles is not null) {
			foreach (BaseTile subTile in _subTiles)
				color += subTile.Color;
			color /= _subTiles.Count;
			return color;
		}

		if (_tiles is not null) {
			foreach (Tile tile in _tiles)
				color += tile.Color;
			color /= _tiles.Count;
			return color;
		}

		throw new InvalidOperationException( "No subtiles or tiles" );
	}

	//A basetile can be subdivided 4 times. The border always stays on the same edge.

	private readonly List<BaseTile>? _subTiles;
	private readonly List<Tile>? _tiles;

	public BaseTile( uint baseVectorIndexA, uint baseVectorIndexB, uint baseVectorIndexC, int layer, List<BaseTile> subTiles ) {
		this.VectorIndexA = baseVectorIndexA;
		this.VectorIndexB = baseVectorIndexB;
		this.VectorIndexC = baseVectorIndexC;
		this.Layer = layer;
		this._subTiles = subTiles;
		Sublayers = GetSubLayers( subTiles );
	}

	public BaseTile( uint baseVectorIndexA, uint baseVectorIndexB, uint baseVectorIndexC, int layer, List<Tile> tiles ) {
		this.VectorIndexA = baseVectorIndexA;
		this.VectorIndexB = baseVectorIndexB;
		this.VectorIndexC = baseVectorIndexC;
		this.Layer = layer;
		this._tiles = tiles;
		Sublayers = 1;
	}

	private int GetSubLayers( List<BaseTile> subTiles ) {
		if (subTiles.Count == 0)
			return 0;
		int max = 0;
		foreach (BaseTile subTile in subTiles)
			max = Math.Max( max, subTile.Sublayers );
		return max + 1;
	}

	internal IReadOnlyList<Tile> GetAllTiles() {
		if (_subTiles is not null) {
			List<Tile> tiles = [];
			foreach (BaseTile subTile in _subTiles) {
				tiles.AddRange( subTile.GetAllTiles() );
			}
			return tiles;
		}
		if (_tiles is not null)
			return _tiles;
		throw new InvalidOperationException( "No subtiles or tiles" );
	}

	public IReadOnlyList<BaseTile>? SubTiles => _subTiles;
	public IReadOnlyList<Tile>? Tiles => _tiles;
}
