using Engine.Standard.Render.Meshing;
using Sandbox.Logic.World.Tiles.Generation;

namespace Sandbox.Logic.World.Tiles;

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

public sealed class Tile( Icosphere icosphere, Region containingTile, int indexA, int indexB, int indexC, uint layer ) : IContainedTile {
	private readonly Icosphere _icosphere = icosphere;
	public int IndexA { get; } = indexA;
	public int IndexB { get; } = indexB;
	public int IndexC { get; } = indexC;
	public Vector3<float> VectorA => _icosphere.Vertices[ IndexA ];
	public Vector3<float> VectorB => _icosphere.Vertices[ IndexB ];
	public Vector3<float> VectorC => _icosphere.Vertices[ IndexC ];
	public uint RemainingLayers => 0;
	public uint Layer { get; } = layer;
	public Vector4<float> Color => GetColor();
	public ITile ContainingTile { get; } = containingTile;

	private readonly List<Tile> _neighbours = [];

	public IReadOnlyList<Tile> Neighbours => _neighbours;
	private Vector4<float> GetColor() {
		return Terrain?.Color ?? throw new InvalidOperationException( "No terrain type" );
		//return (Height > 0 ? Height : 0, Height < 0 ? -Height : 0, 0, 1);
	}

	public float Height { get; set; }
	public TerrainType? Terrain { get; private set; }
	public TileTerrainGenerationLandscapeData? TerrainGenData { get; set; }
	public void AddNeighbour( Tile neighbour ) {
		_neighbours.Add( neighbour );
	}
	public void SetTerrainType( int terrainTypeId ) {
		Terrain = TerrainType.Get( terrainTypeId );
	}
}