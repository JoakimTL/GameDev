using Civs.World.Generation;
using Engine;
using Engine.Standard.Render.Meshing;
using Engine.Structures;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace Civs.World;

public sealed unsafe class Globe : DisposableIdentifiable {

	private readonly IcosphereVectorContainer _icosphereVectorContainer;
	private readonly List<Tile> _tiles;
	private readonly List<BoundedTileEdgeCluster> _clusters;

	public double SimulatedSurfaceArea { get; }

	public Globe( uint subdivisions, double surfaceArea ) {
		SimulatedSurfaceArea = surfaceArea;
		Icosphere icosphere = new( subdivisions );
		_icosphereVectorContainer = new( icosphere.Vertices );

		_tiles = [];
		_clusters = [];
		OcTree<Tile, float> tileTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
		OcTree<Edge, float> edgeTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );

		IReadOnlyList<uint> indices = icosphere.GetIndices( icosphere.Subdivisions - 1 );
		Dictionary<EdgeIndices, Edge> _edges = [];
		Random random = new( 0 );
		for (int i = 0; i < indices.Count; i += 3) {
			TileIndices tileIndices = new( indices[ i ], indices[ i + 1 ], indices[ i + 2 ] );
			Tile tile = new( _icosphereVectorContainer, surfaceArea, (uint) _tiles.Count, tileIndices, (random.NextSingle(), random.NextSingle(), random.NextSingle(), 1) );
			_tiles.Add( tile );
			tileTree.Add( tile );

			Span<EdgeIndices> edges =
			[
				new( tileIndices.A, tileIndices.B ),
				new( tileIndices.B, tileIndices.C ),
				new( tileIndices.C, tileIndices.A )
			];

			for (int j = 0; j < edges.Length; j++) {
				EdgeIndices edgeIndices = edges[ j ];
				if (!_edges.TryGetValue( edgeIndices, out Edge? edge )) {
					edge = new( _icosphereVectorContainer, edgeIndices );
					_edges[ edgeIndices ] = edge;
					edgeTree.Add( edge );
				}
				edge.AddTile( tile );
				tile.AddEdge( edge );
			}
		}

		List<IReadOnlyBranch<Tile, float>> tileBranches = [ .. tileTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
		List<IReadOnlyBranch<Edge, float>> edgeBranches = [ .. edgeTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
		List<(IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>)> pairs = PairClusters( tileBranches, edgeBranches );

		foreach ((IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>) pair in pairs)
			_clusters.Add( new( pair.Item1.BranchBounds, [ .. pair.Item1.Contents ], [ .. pair.Item2.Contents ] ) );

		GenerateLandmass( 42 );

		GenerateTerrain();

	}

	private void GenerateLandmass( int seed ) {
		Random seedProvider = new( seed );
		Noise3 coarseHeighNoise = new( seedProvider.Next(), 5 );
		Noise3 fineHeightNoise = new( seedProvider.Next(), 23 );
		Noise3 landLargeNoise = new( seedProvider.Next(), 3 );
		Noise3 landFineNoise = new( seedProvider.Next(), 13 );
		Noise3 warpNoise = new( seedProvider.Next(), 2.3f );
		foreach (var tile in Tiles) {
			var center = tile.Bounds.GetCenter();
			var warpDirection = warpNoise.Noise( center ) * float.Pi * 2;
			var warpOffset = new Vector3<float>( MathF.Cos( warpDirection ), MathF.Sin( warpDirection ), 0 );
			var warpedCenter = center + new Vector3<float>( warpNoise.Noise( center ) * 2 - 1, warpNoise.Noise( center ) * 2 - 1, warpNoise.Noise( center ) * 2 - 1 ) * 0.1f;
			var landmass = (landLargeNoise.Noise( center ) * 0.75f + landFineNoise.Noise( center ) * 0.25f) > 0.7;
			var n = coarseHeighNoise.Noise( center ) * 0.85f + fineHeightNoise.Noise( center ) * 0.15f;
			n *= n;
			n *= coarseHeighNoise.Noise( warpedCenter ) * fineHeightNoise.Noise( warpedCenter );
			tile.Color = (landmass ? 0.5f : 0, n, n, 1);
			//tile.Height = n > 0.2f;
		}


	}

	private void GenerateTerrain() {
		foreach (var tile in Tiles) {

		}
	}

	private List<(IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>)> PairClusters( IReadOnlyList<IReadOnlyBranch<Tile, float>> tileClusters, IReadOnlyList<IReadOnlyBranch<Edge, float>> edgeClusters ) {
		List<(IReadOnlyBranch<Tile, float>, IReadOnlyBranch<Edge, float>)> result = [];
		foreach (IReadOnlyBranch<Tile, float> tileCluster in tileClusters)
			foreach (IReadOnlyBranch<Edge, float> edgeCluster in edgeClusters)
				if (tileCluster.BranchBounds == edgeCluster.BranchBounds)
					result.Add( (tileCluster, edgeCluster) );
		return result;
	}

	public IReadOnlyList<Tile> Tiles => _tiles;
	public IReadOnlyList<BoundedTileEdgeCluster> Clusters => _clusters;

	protected override bool InternalDispose() {
		_icosphereVectorContainer.Dispose();
		return true;
	}
}
