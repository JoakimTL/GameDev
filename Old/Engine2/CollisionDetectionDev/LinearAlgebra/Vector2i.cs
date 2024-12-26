using System;

namespace Engine.LMath {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
	public struct Vector2i : System.IEquatable<Vector2i>, ILAMeasurable {

		public int X, Y;

		public Vector2 AsFloat => new Vector2( X, Y );
		public Vector2b AsByte => new Vector2b( (byte) X, (byte) Y );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y );
		public float LengthSquared => X * X + Y * Y;

		public Vector2i( int x, int y ) {
			X = x;
			Y = y;
		}

		public Vector2i( Vector2i a ) : this( a.X, a.Y ) { }

		public Vector2i( int s ) : this( s, s ) { }

		public static readonly Vector2i Zero = new Vector2i( 0 );

		#region Override and Equals

		public override bool Equals( object other ) {
			if( !( other is Vector2i ) )
				return false;
			return Equals( (Vector2i) other );
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public bool Equals( Vector2i other ) {
			return X == other.X && Y == other.Y;
		}

		public override string ToString() {
			return $"[{X},{Y}]";
		}

		#endregion

		#region Swizzle
		public Vector2i XX {
			get => new Vector2i( X, X );
		}

		public Vector2i XY {
			get => new Vector2i( X, Y ); set {
				X = value.X;
				Y = value.Y;
			}
		}

		public Vector2i YX {
			get => new Vector2i( Y, X ); set {
				Y = value.X;
				X = value.Y;
			}
		}

		public Vector2i YY {
			get => new Vector2i( Y, Y );
		}

		public Vector3i XXX {
			get => new Vector3i( X, X, X );
		}

		public Vector3i XXY {
			get => new Vector3i( X, X, Y );
		}

		public Vector3i XYX {
			get => new Vector3i( X, Y, X );
		}

		public Vector3i XYY {
			get => new Vector3i( X, Y, Y );
		}

		public Vector3i YXX {
			get => new Vector3i( Y, X, X );
		}

		public Vector3i YXY {
			get => new Vector3i( Y, X, Y );
		}

		public Vector3i YYX {
			get => new Vector3i( Y, Y, X );
		}

		public Vector3i YYY {
			get => new Vector3i( Y, Y, Y );
		}

		public Vector4i XXXX {
			get => new Vector4i( X, X, X, X );
		}

		public Vector4i XXXY {
			get => new Vector4i( X, X, X, Y );
		}

		public Vector4i XXYX {
			get => new Vector4i( X, X, Y, X );
		}

		public Vector4i XXYY {
			get => new Vector4i( X, X, Y, Y );
		}

		public Vector4i XYXX {
			get => new Vector4i( X, Y, X, X );
		}

		public Vector4i XYXY {
			get => new Vector4i( X, Y, X, Y );
		}

		public Vector4i XYYX {
			get => new Vector4i( X, Y, Y, X );
		}

		public Vector4i XYYY {
			get => new Vector4i( X, Y, Y, Y );
		}

		public Vector4i YXXX {
			get => new Vector4i( Y, X, X, X );
		}

		public Vector4i YXXY {
			get => new Vector4i( Y, X, X, Y );
		}

		public Vector4i YXYX {
			get => new Vector4i( Y, X, Y, X );
		}

		public Vector4i YXYY {
			get => new Vector4i( Y, X, Y, Y );
		}

		public Vector4i YYXX {
			get => new Vector4i( Y, Y, X, X );
		}

		public Vector4i YYXY {
			get => new Vector4i( Y, Y, X, Y );
		}

		public Vector4i YYYX {
			get => new Vector4i( Y, Y, Y, X );
		}

		public Vector4i YYYY {
			get => new Vector4i( Y, Y, Y, Y );
		}
		#endregion

		#region Operators
		public static Vector2i operator +( Vector2i a, Vector2i b ) {
			return new Vector2i( a.X + b.X, a.Y + b.Y );
		}

		public static Vector2i operator -( Vector2i a ) {
			return new Vector2i( -a.X, -a.Y );
		}

		public static Vector2i operator *( Vector2i a, int scalar ) {
			return new Vector2i( a.X * scalar, a.Y * scalar );
		}

		public static Vector2i operator *( int scalar, Vector2i a ) {
			return new Vector2i( a.X * scalar, a.Y * scalar );
		}

		public static Vector2 operator *( Vector2i a, float scalar ) {
			return new Vector2( a.X * scalar, a.Y * scalar );
		}

		public static Vector2 operator *( float scalar, Vector2i a ) {
			return new Vector2( a.X * scalar, a.Y * scalar );
		}

		public static Vector2i operator *( Vector2i a, Vector2i b ) {
			return new Vector2i( a.X * b.X, a.Y * b.Y );
		}

		public static Vector2 operator *( Vector2i a, Vector2 b ) {
			return new Vector2( a.X * b.X, a.Y * b.Y );
		}

		public static Vector2 operator *( Vector2 a, Vector2i b ) {
			return new Vector2( a.X * b.X, a.Y * b.Y );
		}

		public static Vector2i operator -( Vector2i a, Vector2i b ) {
			return a + -b;
		}

		public static Vector2i operator /( Vector2i a, int divident ) {
			return ( a * ( 1f / divident ) ).IntCasted;
		}

		public static Vector2 operator /( Vector2i a, float divident ) {
			return a * ( 1f / divident );
		}

		public static bool operator ==( Vector2i a, Vector2i b ) {
			return a.Equals( b );
		}

		public static bool operator !=( Vector2i a, Vector2i b ) {
			return !a.Equals( b );
		}

		#region Implicit
		public static implicit operator Vector2i( int[] a ) {
			if( a.Length != 2 )
				throw new System.ArgumentException( "Length of array is not compatible with Vector2i" );
			return new Vector2i( a[ 0 ], a[ 1 ] );
		}

		public static implicit operator Vector2i( int a ) {
			return new Vector2i( a );
		}

		public static implicit operator int[]( Vector2i a ) {
			return new int[] { a.X, a.Y };
		}
		#endregion
		#endregion

	}
}
