using Engine;
using Engine.Generation.Meshing;
using Engine.Structures;

namespace Civlike.World.GenerationState;
public abstract class GeneratingGlobeBase {
	protected GeneratingGlobeBase( Type faceStateType ) {
		if (!faceStateType.IsClass || faceStateType.IsAbstract || !faceStateType.Resolve().HasParameterlessConstructor || !faceStateType.IsAssignableTo( typeof( FaceStateBase ) ))
			throw new ArgumentException( "faceStateType must be a non-abstract class with a parameterless constructor and must derive from FaceStateBase.", nameof( faceStateType ) );
		this.Vertices = [];
		this.Edges = [];
		this.Faces = [];
		this.FaceStateType = faceStateType;
	}

	public void Initialize( GlobeGeneratorParameterBase parameters ) {
		this.SeedProvider = new( parameters.GenerationSeed );
	}

	public Random SeedProvider { get; private set; } = null!;
	public Icosphere? Icosphere { get; private set; }
	public IReadOnlyList<Vertex> Vertices { get; private set; }
	public IReadOnlyList<Edge> Edges { get; private set; }
	public IReadOnlyList<FaceBase> Faces { get; private set; }

	public double Radius { get; private set; }
	public double Area => 4 * double.Pi * this.Radius * this.Radius;
	public double TileArea { get; private set; }
	public double TileLength { get; private set; }
	public Type FaceStateType { get; }

	public void SetIcosphere( Icosphere icosphere ) => this.Icosphere = icosphere;
	public void SetVertices( IReadOnlyList<Vertex> vertices ) => this.Vertices = vertices;
	public void SetEdges( IReadOnlyList<Edge> edges ) => this.Edges = edges;
	public void SetFaces( IReadOnlyList<FaceBase> faces ) {
		this.Faces = faces;
		OnFacesSet();
	}

	protected abstract void OnFacesSet();
	public void SetRadius( double radius ) => this.Radius = radius;
	public void SetTileArea( double tileArea ) => this.TileArea = tileArea;
	public void SetTileLength( double approximateTileLength ) => this.TileLength = approximateTileLength;
}