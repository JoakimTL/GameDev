namespace Sandbox.Logic.World;

//public sealed class TileSubdivision {

//	public IReadOnlyList<TileSubdivision>? Subdivisions { get; }
//	public IReadOnlyList<Tile>? Tiles { get; }

//	public TileSubdivision(uint level, Span<uint> indices) {
//		if (level > 1) {
//			Subdivisions = CreateSubdivision( level - 1 );
//		} else {
//			Tiles = CreateTiles();
//		}
//	}

//	private IReadOnlyList<TileSubdivision> CreateSubdivision( uint level ) {
//		var subdivisions = new List<TileSubdivision>();
//		for (uint i = 0; i < 4; i++) {
//			subdivisions.Add( new( level ) );
//		}
//		return subdivisions;
//	}

//	private IReadOnlyList<Tile> CreateTiles() {
//		 var tiles = new List<Tile>();
//	}
//}

public sealed class Tile( uint indexA, uint indexB, uint indexC ) : Identifiable {
	public uint IndexA { get; } = indexA;
	public uint IndexB { get; } = indexB;
	public uint IndexC { get; } = indexC;
	public float Height { get; set; }

	private readonly List<Tile> _neighbours = [];
	public IReadOnlyList<Tile> Neighbours => _neighbours;
	public Vector4<double> Color => GetColor();
	private Vector4<double> GetColor() {
		return (Height > 0 ? Height : 0, Height < 0 ? -Height : 0, 0, 1);//Terrain?.Color ?? throw new InvalidOperationException( "No terrain type" );
	}

	public TerrainType? Terrain { get; private set; }
	public TileTerrainGenerationLandscapeData? TerrainGenData { get; set; }
	public void AddNeighbour( Tile neighbour ) {
		_neighbours.Add( neighbour );
	}
	public void SetTerrainType( int terrainTypeId ) {
		Terrain = TerrainType.Get( terrainTypeId );
	}
}
