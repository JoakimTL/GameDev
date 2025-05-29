using Engine;
using Engine.Structures;
using System.Linq;

namespace Civlike.World.GenerationState;

public abstract class FaceBase : IOcTreeLeaf<float> {
	private readonly Vertex[] _vertices;
	private readonly Edge[] _edges;

	public FaceBase( uint id, GeneratingGlobeBase globe, Vertex[] vertices, Edge[] edges ) {
		this.Id = id;
		this.Globe = globe;
		this._vertices = vertices;
		this._edges = edges;
		foreach (Vertex vertex in vertices)
			vertex.AddFace( this );
		foreach (Edge edge in edges)
			edge.AddFace( this );
		this.TerrainType = TerrainTypeList.Default;
		Center = (this.VectorA + this.VectorB + this.VectorC) / 3f;
		CenterNormalized = Center.Normalize<Vector3<float>, float>();
		this.LatitudeRads = float.Asin( this.Center.Y );
		this.LatitudeRadsAbs = float.Abs( this.LatitudeRads );
		this.LatitudeDegrees = float.RadiansToDegrees( this.LatitudeRads );
		this.LatitudeDegreesAbs = float.Abs( this.LatitudeDegrees );
		this.LatitudeSin = float.Sin( this.LatitudeRads );
		this.LatitudeCos = float.Cos( this.LatitudeRads );
		this.LatitudeTan = float.Tan( this.LatitudeRads );
		Bounds = AABB.Create( [ this.VectorA, this.VectorB, this.VectorC ] );
	}

	public uint Id { get; }
	public GeneratingGlobeBase Globe { get; }

	public IReadOnlyList<Vertex> Vertices => this._vertices;
	public IReadOnlyList<Edge> Edges => this._edges;

	public TerrainTypeBase TerrainType { get; set; }

	public Vector3<float> VectorA => this._vertices[ 0 ].Vector;
	public Vector3<float> VectorB => this._vertices[ 1 ].Vector;
	public Vector3<float> VectorC => this._vertices[ 2 ].Vector;
	public Vector3<float> Center { get; }
	public Vector3<float> CenterNormalized { get; }

	public float LatitudeRads { get; }
	public float LatitudeDegrees { get; }
	public float LatitudeSin { get; }
	public float LatitudeCos { get; }
	public float LatitudeTan { get; }
	public float LatitudeRadsAbs { get; }
	public float LatitudeDegreesAbs { get; }

	public bool IsOcean { get; set; }
	public bool IsLand { get; set; } = true;

	public AABB<Vector3<float>> Bounds { get; }

	public abstract void Apply( GameplayState.Face.Builder builder );

	public override bool Equals( object? obj ) => obj is FaceBase vertex && vertex == this;
	public override int GetHashCode() => this.Id.GetHashCode();

	public static bool operator ==( FaceBase left, FaceBase right ) => left.Id == right.Id;
	public static bool operator !=( FaceBase left, FaceBase right ) => !(left == right);
}
