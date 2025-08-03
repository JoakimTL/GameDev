//using Civlike;
//using Engine;

//namespace OldGen.World.GameplayState;

//public sealed class Vertex {
//	private readonly PackedNormal _packedNormal;
//	private Face[] _connectedFaces;

//	private Vertex( PackedNormal vector, float height, float heightFactor ) {
//		this._packedNormal = vector;
//		this.Height = height;
//		this.HeightFactor = heightFactor;
//		this._connectedFaces = null!;
//	}

//	public Vector3<float> Vector => this._packedNormal.Decode();
//	public Vector3<float> DisplayVector => this.Vector * this.HeightFactor;
//	public IReadOnlyList<Face> ConnectedFaces => this._connectedFaces;

//	public float Height { get; }
//	public float HeightFactor { get; }

//	public override bool Equals( object? obj ) => obj is Vertex vertex && vertex == this;
//	public override int GetHashCode() => unchecked((int) this._packedNormal.RawData);

//	public static bool operator ==( Vertex? left, Vertex? right ) => ReferenceEquals( left, right );
//	public static bool operator !=( Vertex? left, Vertex? right ) => !(left == right);

//	public sealed class Builder { //TODO handle water in a separate render pass?
//		public Builder( PackedNormal packedNormal, float height, double globeRadius/*, bool hasOcean*/ ) {
//			this.Height = height;
//			this.Vertex = new Vertex( packedNormal, this.Height, (float) ((height + globeRadius) / globeRadius) );
//		}

//		public List<Face> ConnectedFaces { get; } = [];
//		public float Height { get; }

//		public void Complete() {
//			this.Vertex._connectedFaces = [ .. this.ConnectedFaces ];
//		}

//		public Vertex Vertex { get; }

//	}
//}
