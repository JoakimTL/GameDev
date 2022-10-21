namespace Engine.Datatypes;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
public struct Vector2b {

	[System.Runtime.InteropServices.FieldOffset( 0 )]
	public byte X;
	[System.Runtime.InteropServices.FieldOffset( 1 )]
	public byte Y;

	public Vector2b( byte x, byte y ) {
		X = x;
		Y = y;
	}

	public Vector2b( int x, int y ) {
		X = (byte) x;
		Y = (byte) y;
	}

	public Vector2b( byte v ) {
		X = Y = v;
	}

	#region Instance Methods
	public override string ToString() => $"Vector2b[{X},{Y}]";
	public bool Equals( Vector2b other ) => X == other.X && Y == other.Y;
	public override bool Equals( object? obj ) => obj is Vector2b v && Equals( v );
	public override int GetHashCode() => Y << 8 | X;
	#endregion

	#region Properties
	public Vector2i AsInt => new( X, Y );
	#endregion

	#region Operations
	public static bool operator ==( Vector2b a, Vector2b b ) => a.Equals( b );
	public static bool operator !=( Vector2b a, Vector2b b ) => !a.Equals( b );
	public static implicit operator Vector2b( Vector2i a ) => new( a.X, a.Y );
	#endregion

}
