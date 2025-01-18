namespace Sandbox.Logic.World.Tiles;

public sealed class TileNeighboursModel( Tile tile ) {
	private readonly Tile _tile = tile;

	private readonly Edge[] _edges = new Edge[ 3 ];
	private Tile[]? _neighbours;

	public IReadOnlyList<Edge> Edges => _edges;
	internal void AddEdge( Edge edge ) {
		for (int i = 0; i < 3; i++)
			if (_edges[ i ] == null) {
				_edges[ i ] = edge;
				return;
			}
		throw new InvalidOperationException( "Tile cannot have more than 3 edges." );
	}

	public IReadOnlyList<Tile> GetNeighbours() {
		if (_neighbours is null) {
			_neighbours = new Tile[ 3 ];
			for (int i = 0; i < 3; i++)
				_neighbours[ i ] = _edges[ i ].Tiles[ 0 ] == _tile ? _edges[ i ].Tiles[ 1 ] : _edges[ i ].Tiles[ 0 ];
		}
		return _neighbours;
	}
}
