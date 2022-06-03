using System.Numerics;

namespace Engine.Data.Datatypes;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
public struct Vector3i {

	[System.Runtime.InteropServices.FieldOffset( 0 )]
	public int X;
	[System.Runtime.InteropServices.FieldOffset( 4 )]
	public int Y;
	[System.Runtime.InteropServices.FieldOffset( 8 )]
	public int Z;

	public Vector3i( int x, int y, int z ) {
		this.X = x;
		this.Y = y;
		this.Z = z;
	}

	public Vector3i( int v ) {
		this.X = this.Y = this.Z = v;
	}

	#region Instance Methods
	public Vector3i Negate() => new( -this.X, -this.Y, -this.Z );
	public bool Inside( AABB3i b ) => AABB3i.Inside( ref b, ref this );
	public override string ToString() => $"Vector3i[{this.X},{this.Y},{this.Z}]";
	public bool Equals( Vector3i other ) => this.X == other.X && this.Y == other.Y && this.Z == other.Z;
	public override bool Equals( object? obj ) => obj is Vector3i v && Equals( v );
	public override int GetHashCode() => this.X ^ this.Y ^ this.Z;
	#endregion

	#region Properties
	public Vector2i XY => new( this.X, this.Y );
	public Vector2i XZ => new( this.X, this.Z );
	public Vector2i YZ => new( this.Y, this.Z );
	public Vector3 AsFloat => new( this.X, this.Y, this.Z );
	#endregion

	#region Static Methods
	public static Vector3i Min( Vector3i a, Vector3i b ) => new( Math.Min( a.X, b.X ), Math.Min( a.Y, b.Y ), Math.Min( a.Z, b.Z ) );
	public static Vector3i Max( Vector3i a, Vector3i b ) => new( Math.Max( a.X, b.X ), Math.Max( a.Y, b.Y ), Math.Max( a.Z, b.Z ) );
	public static Vector3i Round( Vector3 v ) => new( (int) MathF.Round( v.X ), (int) MathF.Round( v.Y ), (int) MathF.Round( v.Z ) );
	public static Vector3i Floor( Vector3 v ) => new( (int) MathF.Floor( v.X ), (int) MathF.Floor( v.Y ), (int) MathF.Floor( v.Z ) );
	public static Vector3i Ceiling( Vector3 v ) => new( (int) MathF.Ceiling( v.X ), (int) MathF.Ceiling( v.Y ), (int) MathF.Ceiling( v.Z ) );
	public static Vector3i Cast( Vector3 v ) => new( (int) v.X, (int) v.Y, (int) v.Z );
	public static Vector3i Abs( Vector3i v ) => new( Math.Abs( v.X ), Math.Abs( v.Y ), Math.Abs( v.Z ) );

	#region Static Operations
	public static Vector3i Add( Vector3i a, Vector3i b ) => new( a.X + b.X, a.Y + b.Y, a.Z + b.Z );
	public static Vector3 Add( Vector3i a, Vector3 b ) => a.AsFloat + b;
	public static Vector3 Add( Vector3 a, Vector3i b ) => a + b.AsFloat;
	public static Vector3i Subtract( Vector3i a, Vector3i b ) => Add( a, b.Negate() );
	public static Vector3i Multiply( Vector3i a, Vector3i b ) => new( a.X * b.X, a.Y * b.Y, a.Z * b.Z );
	public static Vector3 Multiply( Vector3i a, Vector3 b ) => a.AsFloat * b;
	public static Vector3 Multiply( Vector3 a, Vector3i b ) => a * b.AsFloat;
	public static Vector3i Multiply( Vector3i a, int s ) => new( a.X * s, a.Y * s, a.Z * s );
	public static Vector3 Multiply( Vector3i a, float s ) => a.AsFloat * s;
	public static Vector3 Divide( Vector3i a, Vector3 b ) => a.AsFloat / b;
	public static Vector3 Divide( Vector3 a, Vector3i b ) => a / b.AsFloat;
	public static Vector3 Divide( Vector3i a, float s ) => a.AsFloat / s;
	#endregion

	#region Operations
	public static Vector3i operator +( Vector3i a, Vector3i b ) => Add( a, b );
	public static Vector3i operator -( Vector3i a ) => a.Negate();
	public static Vector3i operator -( Vector3i a, Vector3i b ) => Subtract( a, b );
	public static Vector3i operator *( Vector3i a, Vector3i b ) => Multiply( a, b );
	public static Vector3 operator *( Vector3 a, Vector3i b ) => Multiply( a, b );
	public static Vector3 operator *( Vector3i a, Vector3 b ) => Multiply( a, b );
	public static Vector3i operator *( Vector3i a, int s ) => Multiply( a, s );
	public static Vector3 operator *( Vector3i a, float s ) => Multiply( a, s );
	public static Vector3 operator /( Vector3 a, Vector3i b ) => Divide( a, b );
	public static Vector3 operator /( Vector3i a, Vector3 b ) => Divide( a, b );
	public static Vector3 operator /( Vector3i a, float s ) => Divide( a, s );
	public static bool operator ==( Vector3i a, Vector3i b ) => a.Equals( b );
	public static bool operator !=( Vector3i a, Vector3i b ) => !a.Equals( b );
	public static implicit operator Vector3i( (int, int, int) a ) => new( a.Item1, a.Item2, a.Item3 );
	public static implicit operator Vector3i( int a ) => new( a );
	#endregion
	#endregion

}
