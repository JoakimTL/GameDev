using Engine.Generation.Meshing;

namespace Civlike.World.GenerationState;
public abstract class GeneratingGlobeBase {
	protected GeneratingGlobeBase() {
		this.Vertices = [];
		this.Edges = [];
		this.Faces = [];
	}

	public void Initialize( GlobeGeneratorParameterBase parameters ) {
		this.SeedProvider = new( parameters.GenerationSeed );
	}

	public Random SeedProvider { get; private set; } = null!;
	public Icosphere? Icosphere { get; private set; }
	public IReadOnlyList<Vertex> Vertices { get; private set; }
	public IReadOnlyList<Edge> Edges { get; private set; }
	public IReadOnlyList<Face> Faces { get; private set; }

	public double Radius { get; private set; }
	public double Area => 4 * double.Pi * this.Radius * this.Radius;
	public double TileArea { get; private set; }
	public double ApproximateTileLength { get; private set; }

	public void SetIcosphere( Icosphere icosphere ) => this.Icosphere = icosphere;
	public void SetVertices( IReadOnlyList<Vertex> vertices ) => this.Vertices = vertices;
	public void SetEdges( IReadOnlyList<Edge> edges ) => this.Edges = edges;
	public void SetFaces( IReadOnlyList<Face> faces ) => this.Faces = faces;

	public void SetRadius( double radius ) => this.Radius = radius;
	public void SetTileArea( double tileArea ) => this.TileArea = tileArea;
	public void SetApproximateTileLength( double approximateTileLength ) => this.ApproximateTileLength = approximateTileLength;
}