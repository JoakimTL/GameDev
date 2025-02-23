using Engine.Logging;
using Engine.Standard.Render.Meshing;
using ImageMagick;

namespace Sandbox.Logic.Old.OldWorld.Tiles.Generation;

public sealed class WorldTiling {

	public const int Levels = 8;
	public const int RootLevel = 4;

	private readonly Icosphere _worldIcosphere;
	private readonly List<CompositeTile> _rootTiles;
	public Icosphere WorldIcosphere => _worldIcosphere;
	public IReadOnlyList<CompositeTile> Tiles => _rootTiles;

	public WorldTiling() {
		_worldIcosphere = new Icosphere( Levels, normalizeUpTo: RootLevel );
		this.LogLine( $"Icosphere created with {_worldIcosphere.Vertices.Count} vertices", Log.Level.VERBOSE );
		this.LogLine( $"Icosphere created with {_worldIcosphere.GetIndices( Levels - 1 ).Count / 3} faces", Log.Level.VERBOSE );
		List<Vector3<float>> normalizedVectors = _worldIcosphere.Vertices.Select( v => v.Normalize<Vector3<float>, float>() ).ToList();
		//_worldIcosphere.GetIndices( 5 ).Count / 3;
		_rootTiles = CreateTiles();

		Dictionary<(int, int), NeighbouringTiles> tilesBySharedEdge = [];
		List<Tile> allTiles = [];
		foreach (CompositeTile rootTile in _rootTiles)
			AddTiles( rootTile, allTiles );

		foreach (Tile tile in allTiles) {
			(int, int) edgeAB = GetEdge( tile.IndexA, tile.IndexB );
			(int, int) edgeBC = GetEdge( tile.IndexB, tile.IndexC );
			(int, int) edgeCA = GetEdge( tile.IndexC, tile.IndexA );
			if (tilesBySharedEdge.TryGetValue( edgeAB, out NeighbouringTiles? neighboursAB ))
				neighboursAB.TileB = tile;
			else
				tilesBySharedEdge.Add( edgeAB, new() { TileA = tile } );
			if (tilesBySharedEdge.TryGetValue( edgeBC, out NeighbouringTiles? neighboursBC ))
				neighboursBC.TileB = tile;
			else
				tilesBySharedEdge.Add( edgeBC, new() { TileA = tile } );
			if (tilesBySharedEdge.TryGetValue( edgeCA, out NeighbouringTiles? neighboursCA ))
				neighboursCA.TileB = tile;
			else
				tilesBySharedEdge.Add( edgeCA, new() { TileA = tile } );
		}

		foreach (KeyValuePair<(int, int), NeighbouringTiles> tile in tilesBySharedEdge) {
			if (tile.Value.TileB is null)
				throw new InvalidOperationException( "Tile has no neighbour" );
			tile.Value.TileA.AddNeighbour( tile.Value.TileB );
			tile.Value.TileB.AddNeighbour( tile.Value.TileA );
		}

		bool assetsDir = Directory.Exists( "assets" );
		bool texturesDir = Directory.Exists( "assets\\textures" );
		bool fileExists = File.Exists( "assets\\textures\\earthHeightmapLow.png" );
		MagickImage image = new( "assets\\textures\\earthHeightmapLow.png" );
		float[] heights = image.GetPixels().Select( p => p.GetChannel( 0 ) * (1f / ushort.MaxValue) ).ToArray();
		int imageWidth = (int) image.Width;
		int imageHeight = heights.Length / imageWidth;
		image.Dispose();
		TerrainType.Initialize();
		Random r = new( 42 );
		Gradient3Noise coarseNoise = new( (r.Next(), r.Next(), r.Next()), (r.NextSingle() * 20 - 10, r.NextSingle() * 20 - 10, r.NextSingle() * 20 - 10), 0.1f );
		Gradient3Noise fineNoise = new( (r.Next(), r.Next(), r.Next()), (r.NextSingle() * 20 - 10, r.NextSingle() * 20 - 10, r.NextSingle() * 20 - 10), 0.01f );
		//for (float x = -1; x <= 1; x += 1) {
		//	for (float y = -1; y <= 1; y += 1) {
		//		for (float z = -1; z <= 1; z += 1) {
		//			this.LogLine( $"{x}, {y}, {z}: {coarseNoise.Sample( (x, y, z) )}", Log.Level.VERBOSE );
		//		}
		//	}
		//}
		//for (float y = -.5f; y <= .5f; y += .25f) {
		//	for (float z = -.5f; z <= .5f; z += .25f) {
		//		this.LogLine( $"{1}, {y}, {z}: {coarseNoise.Sample( (0.5f, y, z) )}", Log.Level.VERBOSE );
		//	}
		//}
		//this.LogLine( coarseNoise.GetData( (1, 0, 0) ).ToString() );
		//this.LogLine( coarseNoise.GetData( (-1, 0, 0) ).ToString() );
		//this.LogLine( coarseNoise.GetData( (0, 1, 0) ).ToString() );
		//this.LogLine( coarseNoise.GetData( (0, -1, 0) ).ToString() );
		//this.LogLine( coarseNoise.GetData( (0, 0, 1) ).ToString() );
		//this.LogLine( coarseNoise.GetData( (0, 0, -1) ).ToString() );

		List<TileTerrainGenerationLandscapeData> tileTerrainGenerationDatas = [];
		this.LogLine( $"Generating terrain data for {allTiles.Count} tiles...", Log.Level.VERBOSE );
		Span<Vector3<float>> tileVectors = stackalloc Vector3<float>[ 3 ];
		foreach (Tile tile in allTiles) {
			tile.FillSpan( tileVectors );
			Vector3<float> tileTranslation = tileVectors.Average<Vector3<float>, float>();
			float height = GetHeight( tileTranslation, heights, imageWidth, imageHeight );//coarseNoise.Sample( tileTranslation ) + fineNoise.Sample( tileTranslation ) * 0.1f;
			tile.Height = height;
			if (!TileTerrainGenerator.GenerateTerrain( tile, r.NextSingle(), height, out TileTerrainGenerationLandscapeData? data ))
				continue;
			tile.TerrainGenData = data;
			tileTerrainGenerationDatas.Add( data );
		}

		List<CompositeTile> tilesToGenerateFor = [];
		foreach (CompositeTile rootTile in _rootTiles)
			if (!NeighbourFound( rootTile, tilesToGenerateFor ))
				tilesToGenerateFor.Add( rootTile );

		tilesToGenerateFor.AddRange( _rootTiles.Except( tilesToGenerateFor ) );

		this.LogLine( $"Tiles to generate for: {tilesToGenerateFor.Count}", Log.Level.VERBOSE );

		int tiled = 0;
		int lowestPossible = int.MaxValue;
		foreach (CompositeTile rootTile in tilesToGenerateFor) {
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
			if (tiled % 200 == 0)
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

	private float GetHeight( Vector3<float> translation, float[] heights, int width, int height ) {
		Vector2<float> polar = translation.ToNormalizedPolar();
		Vector2<float> positivePolar = polar + (float.Pi, float.Pi / 2);
		Vector2<float> normalizedPolar = positivePolar.DivideEntrywise( (float.Pi * 2, float.Pi) );
		Vector2<float> imagePolar = normalizedPolar.MultiplyEntrywise( (width - 1, height - 1) );
		int x = width - 1 - (int) imagePolar.X;
		int y = height - 1 - (int) imagePolar.Y;
		return heights[ x + y * width ] * 2 - 0.01f;
	}

	private bool NeighbourFound( CompositeTile rootTile, List<CompositeTile> tilesToGenerateFor ) {
		foreach (CompositeTile potentialNeighbour in tilesToGenerateFor)
			if (rootTile.IndexA == potentialNeighbour.IndexA
				|| rootTile.IndexA == potentialNeighbour.IndexB
				|| rootTile.IndexA == potentialNeighbour.IndexC
				|| rootTile.IndexB == potentialNeighbour.IndexA
				|| rootTile.IndexB == potentialNeighbour.IndexB
				|| rootTile.IndexB == potentialNeighbour.IndexC
				|| rootTile.IndexC == potentialNeighbour.IndexA
				|| rootTile.IndexC == potentialNeighbour.IndexB
				|| rootTile.IndexC == potentialNeighbour.IndexC)
				return true;
		return false;
	}

	private void AddTiles( CompositeTile baseTile, List<Tile> list ) {
		list.AddRange( baseTile.GetAllTiles() );
	}

	private (int, int) GetEdge( int indexA, int indexB ) => indexA < indexB
		? (indexA, indexB)
		: indexA != indexB
			? (indexB, indexA)
			: throw new InvalidOperationException( "Indices must be different" );

	private List<CompositeTile> CreateTiles() {
		List<CompositeTile> tiles = [];
		IReadOnlyList<uint> indices = _worldIcosphere.GetIndices( RootLevel );

		for (int i = 0; i < indices.Count; i += 3) {
			int indexA = (int) indices[ i ];
			int indexB = (int) indices[ i + 1 ];
			int indexC = (int) indices[ i + 2 ];

			CompositeTile tile = new( _worldIcosphere, null, indexA, indexB, indexC, RootLevel );
			tile.SetSubTiles( GetSubTiles( tile, indexA, indexB, indexC ).ToArray() );
			tiles.Add( tile );

		}

		return tiles;
	}

	public List<IContainedTile> GetSubTiles( CompositeTile containingCompositeTile, int indexA, int indexB, int indexC ) {
		IReadOnlyList<uint> indices = _worldIcosphere.GetSubdivision( (uint) indexA, (uint) indexB, (uint) indexC );
		List<IContainedTile> tiles = [];
		uint nextLayer = containingCompositeTile.Layer + 1;

		if (nextLayer + 1 == _worldIcosphere.Subdivisions) {
			//We need to create regions here.

			for (int i = 0; i < indices.Count; i += 3) {
				int subIndexA = (int) indices[ i ];
				int subIndexB = (int) indices[ i + 1 ];
				int subIndexC = (int) indices[ i + 2 ];

				Region region = new( _worldIcosphere, containingCompositeTile, subIndexA, subIndexB, subIndexC, containingCompositeTile.Layer + 1 );
				region.SetSubTiles( GetTiles( region, subIndexA, subIndexB, subIndexC ).ToArray() );
				tiles.Add( region );
			}

			return tiles;
		}

		for (int i = 0; i < indices.Count; i += 3) {
			int subIndexA = (int) indices[ i ];
			int subIndexB = (int) indices[ i + 1 ];
			int subIndexC = (int) indices[ i + 2 ];

			CompositeTile tile = new( _worldIcosphere, containingCompositeTile, subIndexA, subIndexB, subIndexC, nextLayer );
			tile.SetSubTiles( GetSubTiles( tile, subIndexA, subIndexB, subIndexC ).ToArray() );
			tiles.Add( tile );
		}

		return tiles;
	}

	private List<Tile> GetTiles( Region containingRegion, int indexA, int indexB, int indexC ) {
		IReadOnlyList<uint> indices = _worldIcosphere.GetSubdivision( (uint) indexA, (uint) indexB, (uint) indexC );
		List<Tile> tiles = [];

		for (int i = 0; i < indices.Count; i += 3) {
			int subIndexA = (int) indices[ i ];
			int subIndexB = (int) indices[ i + 1 ];
			int subIndexC = (int) indices[ i + 2 ];

			tiles.Add( new( _worldIcosphere, containingRegion, subIndexA, subIndexB, subIndexC, containingRegion.Layer + 1 ) );
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
