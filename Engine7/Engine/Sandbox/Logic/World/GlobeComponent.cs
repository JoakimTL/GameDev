using Engine.Module.Entities.Container;
using Engine.Standard.Render.Meshing;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Logic.World;

public sealed class GlobeComponent : ComponentBase, IInitializable {

	//TODO maybe a light version of composite pattern, where "Tiles" have components, such that we don't need many versions of the "same" class just to have different tile types.

	private readonly List<Vector3<float>> _vertices;

	private readonly OcTree<Tile, float> _tileTree;
	private readonly OcTree<Edge, float> _edgeTree;

	private readonly List<Tile> _tiles;
	private readonly Dictionary<EdgeIndices, Edge> _edges;

	public uint Layers { get; set; }
	public double SimulatedSurfaceArea { get; set; }
	public uint GeneratedLayers { get; private set; }
	public double GeneratedSurfaceArea { get; private set; }
	public IReadOnlyList<Vector3<float>> Vertices => _vertices;

	public IReadOnlyOcTree<Tile, float> TileTree => _tileTree;
	public IReadOnlyOcTree<Edge, float> EdgeTree => _edgeTree;
	public IReadOnlyList<Tile> Tiles => _tiles;
	public IReadOnlyDictionary<EdgeIndices, Edge> Edges => _edges;

	public GlobeComponent() {
		this.Layers = 0;

		_tileTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
		_edgeTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
		_vertices = [];
		_tiles = [];
		_edges = [];
	}

	public void Initialize() {
		if (Layers <= 0)
			throw new InvalidOperationException( "Cannot initialize globe with 0 layers." );
		if (SimulatedSurfaceArea <= 0)
			throw new InvalidOperationException( "Cannot initialize globe with 0 simulated surface area." );
		GeneratedLayers = Layers;
		GeneratedSurfaceArea = SimulatedSurfaceArea;
		Icosphere icosphere = new( GeneratedLayers );
		_vertices.AddRange( icosphere.Vertices );
		GenerateMosaic( icosphere );
	}

	private void GenerateMosaic( Icosphere icosphere ) {
		//Generate world tiles.
		//Here we can try to reduce how many tiles we have. Oceans for example might not need as much detail as land.

		var indices = icosphere.GetIndices( icosphere.Subdivisions - 1 );
		Random r = new();
		for (int i = 0; i < indices.Count; i += 3) {
			TriangleIndices triangle = new( (int) indices[ i ], (int) indices[ i + 1 ], (int) indices[ i + 2 ] );
			Tile tile = new( this, triangle, (r.NextSingle(), r.NextSingle(), r.NextSingle(), 1) );
			_tileTree.Add( tile );
			_tiles.Add( tile );

			Span<EdgeIndices> edges =
			[
				new( triangle.A, triangle.B ),
				new( triangle.B, triangle.C ),
				new( triangle.C, triangle.A ),
			];

			for (int j = 0; j < edges.Length; j++) {
				EdgeIndices edgeIndices = edges[ j ];
				if (!_edges.TryGetValue( edgeIndices, out Edge? edge )) {
					edge = new( this, edgeIndices );
					_edges[ edgeIndices ] = edge;
					_edgeTree.Add( edge );
				}
				edge.AddTile( tile );
				tile.AddEdge( edge );
			}
		}

		//Display octree branches as meshes. Only have them active when the camera is on the right side of the globe. Meaning when the center of the bounds dotted with the camera translation (globe is a origin) is positive.

	}
}
