using Engine;

namespace Civlike.World.Geometry;

public sealed class ReadOnlyVertex {

	private readonly PackedNormal _packedNormal;

	private ReadOnlyVertex( int id, PackedNormal packedNormal ) {
		this.Id = id;
		this._packedNormal = packedNormal;
		this.ConnectedFaces = null!;
	}

	public Vector3<float> Vector => this._packedNormal.Decode();
	public IReadOnlyList<ReadOnlyFace> ConnectedFaces { get; private set; }
	public int Id { get; }

	internal sealed class Builder {
		public Builder(int id, PackedNormal packedNormal ) {
			this.Vertex = new ReadOnlyVertex( id, packedNormal );
		}

		public List<ReadOnlyFace> ConnectedFaces { get; } = [];

		public void Complete() {
			this.Vertex.ConnectedFaces = new List<ReadOnlyFace>( this.ConnectedFaces ).AsReadOnly();
		}

		public ReadOnlyVertex Vertex { get; }
	}
}