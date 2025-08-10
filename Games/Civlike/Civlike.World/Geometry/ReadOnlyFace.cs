using Engine;
using Engine.Structures;

namespace Civlike.World.Geometry;

public sealed class ReadOnlyFace : IOcTreeLeaf<float> {

	private ReadOnlyFace( int id ) {
		this.Id = id;
		this.Vertices = null!;
		this.Edges = null!;
		this.Neighbours = null!;
	}

	public int Id { get; }

	public IReadOnlyList<ReadOnlyVertex> Vertices { get; private set; }
	public IReadOnlyList<ReadOnlyEdge> Edges { get; private set; }
	public IReadOnlyList<ReadOnlyFace> Neighbours { get; private set; }

	public AABB<Vector3<float>> Bounds => AABB.Create( [ this.Vertices[ 0 ].Vector, this.Vertices[ 1 ].Vector, this.Vertices[ 2 ].Vector ] );
	public Vector3<float> GetCenter() => (this.Vertices[ 0 ].Vector + this.Vertices[ 1 ].Vector + this.Vertices[ 2 ].Vector) / 3f;

	public override bool Equals( object? obj ) => obj is ReadOnlyFace vertex && vertex == this;
	public override int GetHashCode() => this.Id.GetHashCode();


	public static bool operator ==( ReadOnlyFace? left, ReadOnlyFace? right ) => ReferenceEquals( left, right );
	public static bool operator !=( ReadOnlyFace? left, ReadOnlyFace? right ) => !(left == right);

	internal sealed class Builder {
		public readonly List<ReadOnlyVertex> Vertices;
		public readonly List<ReadOnlyEdge> Edges;
		public readonly List<ReadOnlyFace> Neighbours;

		internal Builder( int id ) {
			this.Vertices = [];
			this.Edges = [];
			this.Neighbours = [];
			this.Face = new ReadOnlyFace( id );
		}

		public void Complete() {
			this.Face.Vertices = new List<ReadOnlyVertex>( this.Vertices ).AsReadOnly();
			this.Face.Edges = new List<ReadOnlyEdge>( this.Edges ).AsReadOnly();
			this.Face.Neighbours = new List<ReadOnlyFace>( this.Neighbours ).AsReadOnly();
		}

		public ReadOnlyFace Face { get; }
	}
}
