namespace Engine.Data.Datatypes;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
public struct Vector3b {

	[System.Runtime.InteropServices.FieldOffset( 0 )]
	public byte X;
	[System.Runtime.InteropServices.FieldOffset( 1 )]
	public byte Y;
	[System.Runtime.InteropServices.FieldOffset( 2 )]
	public byte Z;

	public Vector3b( byte x, byte y, byte z ) {
		this.X = x;
		this.Y = y;
		this.Z = z;
	}

	public Vector3b( int x, int y, int z ) {
		this.X = (byte) x;
		this.Y = (byte) y;
		this.Z = (byte) z;
	}

	public Vector3b( byte v ) {
		this.X = this.Y = this.Z = v;
	}

	#region Instance Methods
	public override string ToString() => $"Vector3b[{this.X},{this.Y},{this.Z}]";
	public bool Equals( Vector3b other ) => this.X == other.X && this.Y == other.Y && this.Z == other.Z;
	public override bool Equals( object? obj ) => obj is Vector3b v && Equals( v );
	public override int GetHashCode() => (this.Z << 16) | (this.Y << 8) | this.X;
	#endregion

	#region Properties
	public Vector2b XY => new( this.X, this.Y );
	public Vector3i AsInt => new( this.X, this.Y, this.Z );
	#endregion

	#region Operations
	public static bool operator ==( Vector3b a, Vector3b b ) => a.Equals( b );
	public static bool operator !=( Vector3b a, Vector3b b ) => !a.Equals( b );
	public static implicit operator Vector3b( Vector3i a ) => new( a.X, a.Y, a.Z );
	#endregion

}
