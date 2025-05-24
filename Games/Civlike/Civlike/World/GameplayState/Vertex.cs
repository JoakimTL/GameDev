using Engine;

namespace Civlike.World.GameplayState;

public sealed class Vertex {
	private readonly PackedNormal _packedNormal;
	private Face[] _connectedFaces;

	private Vertex( PackedNormal vector, float height ) {
		this._packedNormal = vector;
		this.Height = height;
		this._connectedFaces = null!;
	}

	public Vector3<float> Vector => this._packedNormal.Decode();
	public IReadOnlyList<Face> ConnectedFaces => this._connectedFaces;

	public float Height { get; }

	public override bool Equals( object? obj ) => obj is Vertex vertex && vertex == this;
	public override int GetHashCode() => unchecked((int) this._packedNormal.RawData);

	public static bool operator ==( Vertex? left, Vertex? right ) => ReferenceEquals( left, right );
	public static bool operator !=( Vertex? left, Vertex? right ) => !(left == right);

	public sealed class Builder {
		public Builder( PackedNormal packedNormal, float height ) {
			this.Height = height;
			this.Vertex = new Vertex( packedNormal, Height );
		}

		public List<Face> ConnectedFaces { get; } = [];
		public float Height { get; }

		public void Complete() {
			this.Vertex._connectedFaces = [ .. this.ConnectedFaces ];
		}

		public Vertex Vertex { get; }

	}
}
