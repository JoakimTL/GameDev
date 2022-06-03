using System.Numerics;

namespace Engine.Data.Datatypes;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
public struct Vector2i {

	[System.Runtime.InteropServices.FieldOffset( 0 )]
	public int X;
	[System.Runtime.InteropServices.FieldOffset( 4 )]
	public int Y;

	public Vector2i( int x, int y ) {
		this.X = x;
		this.Y = y;
	}

	public Vector2i( int v ) {
		this.X = this.Y = v;
	}

	#region Instance Methods
	public Vector2i Negate() => new( -this.X, -this.Y );
	public override string ToString() => $"Vector2i[{this.X},{this.Y}]";
	public bool Equals( Vector2i other ) => this.X == other.X && this.Y == other.Y;
	public override bool Equals( object? obj ) => obj is Vector2i v && Equals( v );
	public override int GetHashCode() => this.X ^ this.Y;
	#endregion

	#region Properties
	public Vector2 AsFloat => new( this.X, this.Y );
	#endregion

	#region Static Methods
	/// <returns>True if any axis of the vector is negative or zero.</returns>
	public static bool NegativeOrZero( Vector2i v ) => v.X <= 0 || v.Y <= 0;
	public static Vector2i Min( Vector2i a, Vector2i b ) => new( Math.Min( a.X, b.X ), Math.Min( a.Y, b.Y ) );
	public static Vector2i Max( Vector2i a, Vector2i b ) => new( Math.Max( a.X, b.X ), Math.Max( a.Y, b.Y ) );
	public static Vector2i Round( Vector2 v ) => new( (int) MathF.Round( v.X ), (int) MathF.Round( v.Y ) );
	public static Vector2i Floor( Vector2 v ) => new( (int) MathF.Floor( v.X ), (int) MathF.Floor( v.Y ) );
	public static Vector2i Ceiling( Vector2 v ) => new( (int) MathF.Ceiling( v.X ), (int) MathF.Ceiling( v.Y ) );
	public static Vector2i Cast( Vector2 v ) => new( (int) v.X, (int) v.Y );

	#region Static Operations
	public static Vector2i Add( Vector2i a, Vector2i b ) => new( a.X + b.X, a.Y + b.Y );
	public static Vector2 Add( Vector2i a, Vector2 b ) => a.AsFloat + b;
	public static Vector2 Add( Vector2 a, Vector2i b ) => a + b.AsFloat;
	public static Vector2i Subtract( Vector2i a, Vector2i b ) => Add( a, b.Negate() );
	public static Vector2i Multiply( Vector2i a, Vector2i b ) => new( a.X * b.X, a.Y * b.Y );
	public static Vector2 Multiply( Vector2i a, Vector2 b ) => a.AsFloat * b;
	public static Vector2 Multiply( Vector2 a, Vector2i b ) => a * b.AsFloat;
	public static Vector2i Multiply( Vector2i a, int s ) => new( a.X * s, a.Y * s );
	public static Vector2 Multiply( Vector2i a, float s ) => a.AsFloat * s;
	public static Vector2 Divide( Vector2i a, Vector2 b ) => a.AsFloat / b;
	public static Vector2 Divide( Vector2 a, Vector2i b ) => a / b.AsFloat;
	public static Vector2 Divide( Vector2i a, float s ) => a.AsFloat / s;
	#endregion

	#region Operations
	public static Vector2i operator +( Vector2i a, Vector2i b ) => Add( a, b );
	public static Vector2i operator -( Vector2i a ) => a.Negate();
	public static Vector2i operator -( Vector2i a, Vector2i b ) => Subtract( a, b );
	public static Vector2i operator *( Vector2i a, Vector2i b ) => Multiply( a, b );
	public static Vector2 operator *( Vector2 a, Vector2i b ) => Multiply( a, b );
	public static Vector2 operator *( Vector2i a, Vector2 b ) => Multiply( a, b );
	public static Vector2i operator *( Vector2i a, int s ) => Multiply( a, s );
	public static Vector2 operator *( Vector2i a, float s ) => Multiply( a, s );
	public static Vector2 operator /( Vector2 a, Vector2i b ) => Divide( a, b );
	public static Vector2 operator /( Vector2i a, Vector2 b ) => Divide( a, b );
	public static Vector2 operator /( Vector2i a, float s ) => Divide( a, s );
	public static bool operator ==( Vector2i a, Vector2i b ) => a.Equals( b );
	public static bool operator !=( Vector2i a, Vector2i b ) => !a.Equals( b );
	public static implicit operator Vector2i( (int, int) a ) => new( a.Item1, a.Item2 );
	public static implicit operator Vector2i( int a ) => new( a );
	#endregion
	#endregion

}
