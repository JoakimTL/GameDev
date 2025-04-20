namespace Civs.World;

public readonly struct EdgeIndices : IEquatable<EdgeIndices> {
	public uint A { get; }
	public uint B { get; }

	public EdgeIndices( uint a, uint b ) {
		if (a == b)
			throw new ArgumentException( "Edge cannot connect a vertex to itself." );

		if (a < b) {
			this.A = a;
			this.B = b;
		} else {
			this.A = b;
			this.B = a;
		}
	}

	public override int GetHashCode() => HashCode.Combine( A, B );
	public override bool Equals( object? obj ) => obj is EdgeIndices edge && Equals( edge );
	public override string ToString() => $"({A}, {B})";
	public bool Equals( EdgeIndices other ) => A == other.A && B == other.B;
	public static bool operator ==( EdgeIndices left, EdgeIndices right ) => left.Equals( right );
	public static bool operator !=( EdgeIndices left, EdgeIndices right ) => !(left == right);
}
