using Engine;
using Engine.Structures;

namespace Civlike.World;

public sealed class ReadOnlyEdge : IOcTreeLeaf<float> {

	private ReadOnlyEdge( int id, ReadOnlyVertex vertexA, ReadOnlyVertex vertexB, ReadOnlyFace faceA ) {
		this.Id = id;
		this.VertexA = vertexA;
		this.VertexB = vertexB;
		this.FaceA = faceA;
		this.FaceB = null!;
	}

	public int Id { get; }
	public ReadOnlyVertex VertexA { get; }
	public ReadOnlyVertex VertexB { get; }
	public ReadOnlyFace FaceA { get; }
	public ReadOnlyFace FaceB { get; private set; }

	public ReadOnlyFace GetOther( ReadOnlyFace face ) {
		if (face == this.FaceA)
			return this.FaceB;
		if (face == this.FaceB)
			return this.FaceA;
		throw new ArgumentException( "Face not connected to this edge." );
	}

	public static Vector3<float> GetNormal( Vector3<float> a, Vector3<float> b ) {
		Vector3<float> right = (b - a).Cross( -a ).Normalize<Vector3<float>, float>();
		Vector3<float> normal = (b - a).Cross( right ).Normalize<Vector3<float>, float>();
		return normal;
	}

	public AABB<Vector3<float>> Bounds => AABB.Create( [ this.VertexA.Vector, this.VertexB.Vector ] );


	public override bool Equals( object? obj ) => obj is ReadOnlyEdge edge && edge == this;
	public override int GetHashCode() => HashCode.Combine( this.VertexA, this.VertexB );

	public static bool operator ==( ReadOnlyEdge? left, ReadOnlyEdge? right ) => ReferenceEquals( left, right );
	public static bool operator !=( ReadOnlyEdge? left, ReadOnlyEdge? right ) => !(left == right);

	internal sealed class Builder {

		internal Builder( int id, ReadOnlyVertex vertexA, ReadOnlyVertex readOnlyVertex, ReadOnlyFace faceA ) {
			this.Edge = new ReadOnlyEdge( id, vertexA, readOnlyVertex, faceA );
		}

		public void Complete( ReadOnlyFace faceB ) {
			this.Edge.FaceB = faceB;
		}

		public ReadOnlyEdge Edge { get; }

	}
}