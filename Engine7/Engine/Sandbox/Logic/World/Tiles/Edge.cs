namespace Sandbox.Logic.World.Tiles;

public sealed class Edge : IOcTreeLeaf<float> {

	public GlobeComponent Globe { get; }
	public readonly EdgeIndices EdgeIndices;

	private readonly Tile[] _tiles;

	public IReadOnlyList<Tile> Tiles => _tiles;

	public Edge( GlobeComponent globe, EdgeIndices edgeIndices ) {
		this.Globe = globe;
		this.EdgeIndices = edgeIndices;
		this._tiles = new Tile[ 2 ];
	}

	public AABB<Vector3<float>> Bounds => AABB.Create( [ Globe.Vertices[ EdgeIndices.A ], Globe.Vertices[ EdgeIndices.B ] ] );

	public Vector3<float> VectorA => Globe.Vertices[ EdgeIndices.A ];
	public Vector3<float> VectorB => Globe.Vertices[ EdgeIndices.B ];
	public Vector3<float> Normal => (_tiles[ 0 ].RenderModel.Normal + _tiles[ 1 ].RenderModel.Normal) / 2;

	internal void AddTile( Tile tile ) {
		if (_tiles[ 0 ] == null)
			_tiles[ 0 ] = tile;
		else if (_tiles[ 1 ] == null)
			_tiles[ 1 ] = tile;
		else
			throw new InvalidOperationException( "Edge cannot connect more than two tiles." );
	}
}