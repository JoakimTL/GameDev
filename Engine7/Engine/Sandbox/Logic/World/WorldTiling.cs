using Engine.Logging;
using Engine.Standard.Render.Meshing;

namespace Sandbox.Logic.World;

public sealed class WorldTiling {

	public const int Levels = 9;
	public const int RootLevel = 4;

	private readonly Icosphere _worldIcosphere;
	private readonly List<BaseTile> _rootTiles;
	public Icosphere WorldIcosphere => _worldIcosphere;
	public IReadOnlyList<BaseTile> Tiles => _rootTiles;

	public WorldTiling() {
		_worldIcosphere = new Icosphere( Levels, normalizeUpTo: RootLevel );
		this.LogLine( $"Icosphere created with {_worldIcosphere.Vertices.Count} vertices", Log.Level.VERBOSE );
		this.LogLine( $"Icosphere created with {_worldIcosphere.GetIndices( Levels - 1 ).Count / 3} faces", Log.Level.VERBOSE );
		var normalizedVectors = _worldIcosphere.Vertices.Select( v => v.Normalize<Vector3<double>, double>() ).ToList();
		//_worldIcosphere.GetIndices( 5 ).Count / 3;
		_rootTiles = CreateTiles();

		Dictionary<(uint, uint), NeighbouringTiles> tilesBySharedEdge = [];
		List<Tile> allTiles = [];
		foreach (BaseTile rootTile in _rootTiles) {
			AddTiles( rootTile, allTiles );
		}

		foreach (Tile tile in allTiles) {
			(uint, uint) edgeAB = GetEdge( tile.IndexA, tile.IndexB );
			(uint, uint) edgeBC = GetEdge( tile.IndexB, tile.IndexC );
			(uint, uint) edgeCA = GetEdge( tile.IndexC, tile.IndexA );
			if (tilesBySharedEdge.TryGetValue( edgeAB, out NeighbouringTiles? neighboursAB )) {
				neighboursAB.TileB = tile;
			} else {
				tilesBySharedEdge.Add( edgeAB, new() { TileA = tile } );
			}
			if (tilesBySharedEdge.TryGetValue( edgeBC, out NeighbouringTiles? neighboursBC )) {
				neighboursBC.TileB = tile;
			} else {
				tilesBySharedEdge.Add( edgeBC, new() { TileA = tile } );
			}
			if (tilesBySharedEdge.TryGetValue( edgeCA, out NeighbouringTiles? neighboursCA )) {
				neighboursCA.TileB = tile;
			} else {
				tilesBySharedEdge.Add( edgeCA, new() { TileA = tile } );
			}
		}

		foreach (var tile in tilesBySharedEdge) {
			if (tile.Value.TileB is null) {
				throw new InvalidOperationException( "Tile has no neighbour" );
			}
			tile.Value.TileA.AddNeighbour( tile.Value.TileB );
			tile.Value.TileB.AddNeighbour( tile.Value.TileA );
		}

		TerrainType.Initialize();
		Random r = new( 42 );
		Gradient3Noise coarseNoise = new( (r.Next(), r.Next(), r.Next()), 0.123f );
		Gradient3Noise fineNoise = new( (r.Next(), r.Next(), r.Next()), 0.00987f );
		List<TileTerrainGenerationLandscapeData> tileTerrainGenerationDatas = [];
		this.LogLine( $"Generating terrain data for {allTiles.Count} tiles...", Log.Level.VERBOSE );
		foreach (Tile tile in allTiles) {
			Vector3<float> tileTranslation = Vector
				.Average<Vector3<double>, double>( [ normalizedVectors[ (int) tile.IndexA ], normalizedVectors[ (int) tile.IndexB ], normalizedVectors[ (int) tile.IndexC ] ] )
				.CastSaturating<double, float>();
			float height = coarseNoise.Sample( tileTranslation ) + fineNoise.Sample( tileTranslation ) * 0.1f;
			tile.Height = height;
			if (!TileTerrainGenerator.GenerateTerrain( tile, r.NextSingle(), height, out TileTerrainGenerationLandscapeData? data ))
				continue;
			tile.TerrainGenData = data;
			tileTerrainGenerationDatas.Add( data );
		}

		List<BaseTile> tilesToGenerateFor = [];
		foreach (BaseTile rootTile in _rootTiles) {
			if (!NeighbourFound( rootTile, tilesToGenerateFor ))
				tilesToGenerateFor.Add( rootTile );
		}

		tilesToGenerateFor.AddRange( _rootTiles.Except( tilesToGenerateFor ) );

		this.LogLine( $"Tiles to generate for: {tilesToGenerateFor.Count}", Log.Level.VERBOSE );

		int tiled = 0;
		int lowestPossible = int.MaxValue;
		foreach (BaseTile rootTile in tilesToGenerateFor) {
			List<TileTerrainGenerationLandscapeData> tileTerrainGenerationDatasInRoot = rootTile
				.GetAllTiles()
				.Select( t => t.TerrainGenData )
				.OfType<TileTerrainGenerationLandscapeData>()
				.ToList();

			while (tileTerrainGenerationDatasInRoot.Count > 0) {
				int numberOfAvailable = 0;
				TileTerrainGenerationLandscapeData? next = null;
				foreach (TileTerrainGenerationLandscapeData tileData in tileTerrainGenerationDatasInRoot) {
					if (numberOfAvailable == lowestPossible)
						break;
					if (!tileData.Applied && (next is null || tileData.NumberOfAvailableTerrainTypes < numberOfAvailable)) {
						numberOfAvailable = tileData.NumberOfAvailableTerrainTypes;
						next = tileData;
					}
				}
				if (next is null)
					throw new InvalidOperationException( "No tile found" );
				lowestPossible = numberOfAvailable;
				next.ApplyTerrainType( ref lowestPossible );
				tileTerrainGenerationDatasInRoot.Remove( next );
			}
			tiled++;
			if (tiled % 50 == 0)
				this.LogLine( $"Generated {tiled}/{tilesToGenerateFor.Count}", Log.Level.VERBOSE );
		}

		//tileTerrainGenerationDatas.RemoveAll( t => t.Applied );
		//tiled = 0;

		//while (tileTerrainGenerationDatas.Count > 0) {
		//	int numberOfAvailable = 0;
		//	TileTerrainGenerationLandscapeData? next = null;
		//	foreach (TileTerrainGenerationLandscapeData tileData in tileTerrainGenerationDatas) {
		//		if (numberOfAvailable == lowestPossible)
		//			break;
		//		if (!tileData.Applied && (next is null || tileData.NumberOfAvailableTerrainTypes < numberOfAvailable)) {
		//			numberOfAvailable = tileData.NumberOfAvailableTerrainTypes;
		//			next = tileData;
		//		}
		//	}
		//	if (next is null)
		//		throw new InvalidOperationException( "No tile found" );
		//	lowestPossible = numberOfAvailable;
		//	next.ApplyTerrainType( ref lowestPossible );
		//	tileTerrainGenerationDatas.Remove( next );
		//	tiled++;
		//	//tileTerrainGenerationDatas.Remove( next );
		//	if (tiled % 1000 == 0) {
		//		this.LogLine( $"Remaining: {tileTerrainGenerationDatas.Count}", Log.Level.VERBOSE );
		//	}
	}

	private bool NeighbourFound( BaseTile rootTile, List<BaseTile> tilesToGenerateFor ) {
		foreach (var potentialNeighbour in tilesToGenerateFor)
			if (rootTile.VectorIndexA == potentialNeighbour.VectorIndexA
				|| rootTile.VectorIndexA == potentialNeighbour.VectorIndexB
				|| rootTile.VectorIndexA == potentialNeighbour.VectorIndexC
				|| rootTile.VectorIndexB == potentialNeighbour.VectorIndexA
				|| rootTile.VectorIndexB == potentialNeighbour.VectorIndexB
				|| rootTile.VectorIndexB == potentialNeighbour.VectorIndexC
				|| rootTile.VectorIndexC == potentialNeighbour.VectorIndexA
				|| rootTile.VectorIndexC == potentialNeighbour.VectorIndexB
				|| rootTile.VectorIndexC == potentialNeighbour.VectorIndexC) {
				return true;
			}
		return false;
	}

	private void AddTiles( BaseTile baseTile, List<Tile> list ) {
		if (baseTile.Tiles is not null) {
			list.AddRange( baseTile.Tiles );
			return;
		}
		if (baseTile.SubTiles is not null) {
			foreach (BaseTile subTile in baseTile.SubTiles) {
				AddTiles( subTile, list );
			}
		}
	}

	private (uint, uint) GetEdge( uint indexA, uint indexB ) => indexA < indexB
		? (indexA, indexB)
		: indexA != indexB
			? (indexB, indexA)
			: throw new InvalidOperationException( "Indices must be different" );

	private List<BaseTile> CreateTiles() {
		List<BaseTile> tiles = [];
		IReadOnlyList<uint> indices = _worldIcosphere.GetIndices( RootLevel );

		for (int i = 0; i < indices.Count; i += 3) {
			uint indexA = indices[ i ];
			uint indexB = indices[ i + 1 ];
			uint indexC = indices[ i + 2 ];

			tiles.Add( new BaseTile( indexA, indexB, indexC, RootLevel, GetSubTiles( indexA, indexB, indexC, RootLevel ) ) );
		}

		return tiles;
	}

	public List<BaseTile> GetSubTiles( uint indexA, uint indexB, uint indexC, int layer ) {
		IReadOnlyList<uint> indices = _worldIcosphere.GetSubdivision( indexA, indexB, indexC );
		List<BaseTile> tiles = [];

		for (int i = 0; i < indices.Count; i += 3) {
			uint subIndexA = indices[ i ];
			uint subIndexB = indices[ i + 1 ];
			uint subIndexC = indices[ i + 2 ];

			BaseTile tile;
			if (layer + 1 == _worldIcosphere.Subdivisions - 1) {
				tile = new BaseTile( subIndexA, subIndexB, subIndexC, layer, GetTiles( subIndexA, subIndexB, subIndexC ) );
			} else {
				tile = new BaseTile( subIndexA, subIndexB, subIndexC, layer, GetSubTiles( subIndexA, subIndexB, subIndexC, layer + 1 ) );
			}
			tiles.Add( tile );
		}

		return tiles;
	}

	private List<Tile> GetTiles( uint indexA, uint indexB, uint indexC ) {
		IReadOnlyList<uint> indices = _worldIcosphere.GetSubdivision( indexA, indexB, indexC );
		List<Tile> tiles = [];

		for (int i = 0; i < indices.Count; i += 3) {
			uint subIndexA = indices[ i ];
			uint subIndexB = indices[ i + 1 ];
			uint subIndexC = indices[ i + 2 ];

			tiles.Add( new( subIndexA, subIndexB, subIndexC ) );
		}

		return tiles;
	}


	//public IReadOnlyList<Tile> GetTilesForSubdivision(int subdivision) {
	//	return _tilesPerSubdivision[ subdivision ];
	//}

	public sealed class NeighbouringTiles {
		public Tile TileA { get; set; }
		public Tile TileB { get; set; }
	}
}
