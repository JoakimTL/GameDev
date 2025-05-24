namespace Civlike.World.GenerationState;

public readonly struct EdgeIndices : IEquatable<EdgeIndices> {
	public uint VertexA { get; }
	public uint VertexB { get; }

	public EdgeIndices( uint a, uint b ) {
		if (a == b)
			throw new ArgumentException( "Edge cannot connect a vertex to itself." );

		if (a < b) {
			this.VertexA = a;
			this.VertexB = b;
		} else {
			this.VertexA = b;
			this.VertexB = a;
		}
	}

	public override int GetHashCode() => HashCode.Combine( this.VertexA, this.VertexB );
	public override bool Equals( object? obj ) => obj is EdgeIndices edge && Equals( edge );
	public override string ToString() => $"({this.VertexA}, {this.VertexB})";
	public bool Equals( EdgeIndices other ) => this.VertexA == other.VertexA && this.VertexB == other.VertexB;
	public static bool operator ==( EdgeIndices left, EdgeIndices right ) => left.Equals( right );
	public static bool operator !=( EdgeIndices left, EdgeIndices right ) => !(left == right);
}