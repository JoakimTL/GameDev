namespace Civs.World;

public readonly struct TileIndices : IEquatable<TileIndices> {
	public uint A { get; }
	public uint B { get; }
	public uint C { get; }

	public TileIndices( uint a, uint b, uint c ) {
		if (a == b || b == c || c == a)
			throw new ArgumentException( "Tile cannot connect a vertex to itself." );

		A = a;
		B = b;
		C = c;
	}

	public override int GetHashCode() => HashCode.Combine( A, B, C );
	public override bool Equals( object? obj ) => obj is TileIndices tile && Equals( tile );
	public override string ToString() => $"({A}, {B}, {C})";
	public bool Equals( TileIndices other ) => A == other.A && B == other.B && C == other.C;
	public static bool operator ==( TileIndices left, TileIndices right ) => left.Equals( right );
	public static bool operator !=( TileIndices left, TileIndices right ) => !(left == right);
}