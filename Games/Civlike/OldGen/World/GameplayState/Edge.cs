//using Engine;
//using Engine.Structures;

//namespace OldGen.World.GameplayState;

//public sealed class Edge( Vertex vertexA, Vertex vertexB, Face faceA, Face faceB ) : IOcTreeLeaf<float> {
//	public Vertex VertexA { get; } = vertexA;
//	public Vertex VertexB { get; } = vertexB;
//	public Face FaceA { get; } = faceA;
//	public Face FaceB { get; } = faceB;

//	public Face GetOther( Face face ) {
//		if (face == this.FaceA)
//			return this.FaceB;
//		if (face == this.FaceB)
//			return this.FaceA;
//		throw new ArgumentException( "Face not connected to this edge." );
//	}

//	public Vector3<float> VectorA => this.VertexA.Vector;
//	public Vector3<float> VectorB => this.VertexB.Vector;
//	public Vector3<float> DisplayVectorA => this.VertexA.DisplayVector;
//	public Vector3<float> DisplayVectorB => this.VertexB.DisplayVector;


//	public static Vector3<float> GetNormal( Vector3<float> a, Vector3<float> b ) {
//		Vector3<float> right = (b - a).Cross( -a ).Normalize<Vector3<float>, float>();
//		Vector3<float> normal = (b - a).Cross( right ).Normalize<Vector3<float>, float>();
//		return normal;
//	}

//	public AABB<Vector3<float>> Bounds => AABB.Create( [ this.VectorA, this.VectorB ] );

//	public override bool Equals( object? obj ) => obj is Edge edge && edge == this;
//	public override int GetHashCode() => HashCode.Combine( this.VertexA, this.VertexB );

//	public static bool operator ==( Edge? left, Edge? right ) => ReferenceEquals( left, right );
//	public static bool operator !=( Edge? left, Edge? right ) => !(left == right);
//}
