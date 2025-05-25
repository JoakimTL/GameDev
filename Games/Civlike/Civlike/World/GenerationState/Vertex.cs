using Engine;

namespace Civlike.World.GenerationState;
public sealed class Vertex( int id, PackedNormal vector ) {
	private readonly PackedNormal _packedNormal = vector;
	private readonly List<FaceBase> _neighbourhood = [];

	internal void AddFace( FaceBase face ) {
		if (this._neighbourhood.Contains( face ))
			throw new InvalidOperationException( "Face already connected to this vertex." );
		this._neighbourhood.Add( face );
	}

	public PackedNormal PackedNormal => this._packedNormal;
	public Vector3<float> Vector => this._packedNormal.Decode();

	public IReadOnlyList<FaceBase> ConnectedFaces => this._neighbourhood;

	public int Id { get; } = id;
	public float Height { get; set; }
	public float HeightFactor { get; set; } = 1;

	public override bool Equals( object? obj ) => obj is Vertex vertex && vertex == this;
	public override int GetHashCode() => unchecked((int) this._packedNormal.RawData);

	public static bool operator ==( Vertex left, Vertex right ) => left._packedNormal.RawData == right._packedNormal.RawData;
	public static bool operator !=( Vertex left, Vertex right ) => !(left == right);
}
