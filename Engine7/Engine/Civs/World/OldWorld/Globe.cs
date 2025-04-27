//using Civs.World.Generation;
//using Engine;
//using Engine.Standard.Render.Meshing;
//using Engine.Structures;

//namespace Civs.World;

//public sealed unsafe class Globe : DisposableIdentifiable {

//	public Guid Id { get; }

//	private readonly UnmanagedStaticContainer<Vector3<float>> _icosphereVectorContainer;
//	private readonly List<Tile> _tiles;
//	private readonly List<BoundedTileEdgeCluster> _clusters;

//	public AABB<Vector3<float>> ClusterBounds { get; }
//	public double SimulatedSurfaceArea { get; }
//	public double Radius { get; }

//	public Globe( uint subdivisions, double surfaceArea, double heightspanMeters, double sealevel, double radius ) {
//		Id = Guid.NewGuid();
//		SimulatedSurfaceArea = surfaceArea;
//		Radius = radius - heightspanMeters;
//		Icosphere icosphere = new( subdivisions );
//		_icosphereVectorContainer = new( icosphere.Vertices );

//		_tiles = [];
//		_clusters = [];
//		OcTree<Tile, float> tileTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
//		OcTree<Edge, float> edgeTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );

//		IReadOnlyList<uint> indices = icosphere.GetIndices();
//		Dictionary<EdgeIndices, Edge> _edges = [];
//		for (int i = 0; i < indices.Count; i += 3) {
//			TileIndices tileIndices = new( indices[ i ], indices[ i + 1 ], indices[ i + 2 ] );
//			Tile tile = new( _icosphereVectorContainer, surfaceArea, (uint) _tiles.Count, tileIndices );
//			_tiles.Add( tile );
//			tileTree.Add( tile );

//			Span<EdgeIndices> edges =
//			[
//				new( tileIndices.A, tileIndices.B ),
//				new( tileIndices.B, tileIndices.C ),
//				new( tileIndices.C, tileIndices.A )
//			];

//			for (int j = 0; j < edges.Length; j++) {
//				EdgeIndices edgeIndices = edges[ j ];
//				if (!_edges.TryGetValue( edgeIndices, out Edge? edge )) {
//					edge = new( _icosphereVectorContainer, edgeIndices );
//					_edges[ edgeIndices ] = edge;
//					edgeTree.Add( edge );
//				}
//				edge.AddTile( tile );
//				tile.AddEdge( edge );
//			}
//		}

//		List<IReadOnlyBranch<Tile, float>> tileBranches = [ .. tileTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
//		List<IReadOnlyBranch<Edge, float>> edgeBranches = [ .. edgeTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
//		List<(IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>)> pairs = PairClusters( tileBranches, edgeBranches );

//		foreach ((IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>) pair in pairs)
//			_clusters.Add( new( _clusters.Count, pair.Item1.BranchBounds, [ .. pair.Item1.Contents ], [ .. pair.Item2.Contents ] ) );

//		ClusterBounds = new Vector3<float>().CreateBounds( _clusters.First().Bounds.GetLengths() * 0.5f );

//		GenerateLandmass( 42, heightspanMeters, sealevel );

//		GenerateTerrain();

//	}

//	private void GenerateLandmass( int seed, double heightspanMeters, double sealevel ) {
//		double maxDepth = sealevel;
//		double maxHeight = heightspanMeters - sealevel;

//		Random seedProvider = new( seed );
//		Noise3 coarseHeightNoise = new( seedProvider.Next(), 5 );
//		Noise3 fineHeightNoise = new( seedProvider.Next(), 23 );
//		Noise3 coarseHeightNoise2 = new( seedProvider.Next(), 3 );
//		Noise3 fineHeightNoise2 = new( seedProvider.Next(), 17 );

//		Noise3 landLargeNoise = new( seedProvider.Next(), 2 );
//		Noise3 landFineNoise = new( seedProvider.Next(), 13 );
//		Noise3 landTranslationStrengthNoise = new( seedProvider.Next(), 7 );

//		FiniteVoronoiNoise3 voronoiTranslation = new( new( seedProvider.Next() ), 0.5f, 1 );
//		FiniteVoronoiNoise3 voronoiRidgeNoise = new( new( seedProvider.Next() ), 0.25f, 1 );
//		FiniteVoronoiNoise3 voronoiRidgeNoiseFine = new( new( seedProvider.Next() ), 0.0625f, 1 );
//		//double minPresentHeight = float.MaxValue;
//		//double maxPresentHeight = float.MinValue;
//		foreach (Tile tile in Tiles) {
//			//https://www.youtube.com/watch?v=WumyfLEa6bU
//			//https://iquilezles.org/articles/morenoise/
//			Vector3<float> center = tile.Bounds.GetCenter();
//			Vector3<float> translation = voronoiTranslation.NoiseVector( center );
//			Vector3<float> smallTranslation = translation * 0.05f;
//			float lTs = landTranslationStrengthNoise.Noise( center );
//			float landmassN = (landLargeNoise.Noise( center + smallTranslation * lTs ) * 0.85f + landFineNoise.Noise( center + translation * lTs ) * 0.15f);
//			landmassN = float.Sqrt( landmassN );
//			bool isLand = landmassN > 0.73f;

//			float n = (coarseHeightNoise.Noise( center ) * 0.9f + fineHeightNoise.Noise( center ) * 0.1f) * (coarseHeightNoise2.Noise( center ) * 0.7f + fineHeightNoise2.Noise( center ) * 0.3f);
//			float rN = voronoiRidgeNoise.Noise( center ) * 0.85f + voronoiRidgeNoiseFine.Noise( center ) * 0.15f;
//			n *= rN;

//			if (isLand) {
//				tile.Height = n * landmassN * maxHeight;
//			} else {
//				tile.Height = -n * landmassN * maxDepth;
//			}

//			//if (tile.Height < minPresentHeight)
//			//	minPresentHeight = tile.Height;
//			//if (tile.Height > maxPresentHeight)
//			//	maxPresentHeight = tile.Height;

//			//tile.VectorPush = 1 + (float) ((tile.Height + maxDepth) / Radius);
//		}

//		//double heightDiff = maxPresentHeight - minPresentHeight;

//		//foreach (Tile tile in Tiles) {
//		//	float h = (float) ((tile.Height - minPresentHeight) / heightDiff);
//		//	tile.Color = (tile.Height >= 0 ? 1 : 0, h, h, 1);
//		//}


//	}

//	private void GenerateTerrain() {
//		foreach (Tile tile in Tiles) {
//			tile.Terrain = tile.Height >= 0 ? TerrainTypes.GetTerrainType<GrasslandTerrain>() : TerrainTypes.GetTerrainType<OceanTerrain>();
//		}
//	}

//	private List<(IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>)> PairClusters( IReadOnlyList<IReadOnlyBranch<Tile, float>> tileClusters, IReadOnlyList<IReadOnlyBranch<Edge, float>> edgeClusters ) {
//		List<(IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>)> result = [];
//		foreach (IReadOnlyBranch<Tile, float> tileCluster in tileClusters)
//			foreach (IReadOnlyBranch<Edge, float> edgeCluster in edgeClusters)
//				if (tileCluster.BranchBounds == edgeCluster.BranchBounds)
//					result.Add( (tileCluster, edgeCluster) );
//		return result;
//	}

//	public IReadOnlyList<Tile> Tiles => _tiles;
//	public IReadOnlyList<BoundedTileEdgeCluster> Clusters => _clusters;

//	protected override bool InternalDispose() {
//		_icosphereVectorContainer.Dispose();
//		return true;
//	}
//}
