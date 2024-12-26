using System;

namespace Engine.LMath {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
	public struct Vector2 : System.IEquatable<Vector2>, ILANormalizable<Vector2>, ILAMeasurable {

		public float X, Y;

		public Vector2i IntCasted => new Vector2i( (int) X, (int) Y );
		public Vector2i IntRounded => new Vector2i( (int) System.Math.Round( X ), (int) System.Math.Round( Y ) );
		public Vector2i IntFloored => new Vector2i( (int) System.Math.Floor( X ), (int) System.Math.Floor( Y ) );
		public Vector2i IntCeiled => new Vector2i( (int) System.Math.Ceiling( X ), (int) System.Math.Ceiling( Y ) );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y );
		public float LengthSquared => X * X + Y * Y;

		public Vector2 Normalized => Normalize( ref this );

		public Vector2 Perpendicular => new Vector2( -Y, X );

		public Vector2( float x, float y ) {
			X = x;
			Y = y;
		}

		public Vector2( Vector2 a ) : this( a.X, a.Y ) { }

		public Vector2( float s ) : this( s, s ) { }

		public static readonly Vector2 Zero = new Vector2( 0 );
		public static readonly Vector2 One = new Vector2( 1 );
		public static readonly Vector2 UnitX = new Vector2( 1, 0 );
		public static readonly Vector2 UnitY = new Vector2( 0, 1 );

		public void Normalize() {
			Normalize( ref this );
		}

		public static float Distance( Vector2 a, Vector2 b ) {
			return ( a - b ).Length;
		}

		#region Transforms
		#region Matrix4
		public static void Transform( ref Vector2 vec, ref Matrix4 mat, out Vector2 result ) {
			Vector4 v4 = new Vector4( vec.X, vec.Y, 0.0f, 1.0f );
			Vector4.Transform( ref v4, ref mat, out v4 );
			result = v4.XY;
		}

		public static Vector2 Transform( ref Vector2 vec, ref Matrix4 mat ) {
			Transform( ref vec, ref mat, out Vector2 result );
			return result;
		}

		public static Vector2 Transform( Vector2 vec, Matrix4 mat ) {
			Transform( ref vec, ref mat, out Vector2 result );
			return result;
		}
		#endregion
		#endregion

		#region Normalize

		public static void Normalize( ref Vector2 a, out Vector2 result ) {
			result = a / a.Length;
		}

		public static Vector2 Normalize( ref Vector2 a ) {
			Normalize( ref a, out Vector2 r );
			return r;
		}

		public static Vector2 Normalize( Vector2 a ) {
			Normalize( ref a, out Vector2 r );
			return r;
		}

		#endregion

		#region Dot Product

		public static float Dot( ref Vector2 a, ref Vector2 b ) {
			return a.X * b.X + a.Y * b.Y;
		}

		public static float Dot( Vector2 a, Vector2 b ) {
			return Dot( ref a, ref b );
		}

		#endregion

		#region Override and Equals

		public override bool Equals( object other ) {
			if( other is Vector2 == false )
				return false;
			return Equals( (Vector2) other );
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public bool Equals( Vector2 other ) {
			return X == other.X && Y == other.Y;
		}

		public override string ToString() {
			return $"[{string.Format( "{0,6:F2}", X )},{string.Format( "{0,6:F2}", Y )}]";
		}

		#endregion

		#region Swizzle
		public Vector2 XX {
			get => new Vector2( X, X );
		}

		public Vector2 XY {
			get => new Vector2( X, Y ); set {
				X = value.X;
				Y = value.Y;
			}
		}

		public Vector2 YX {
			get => new Vector2( Y, X ); set {
				Y = value.X;
				X = value.Y;
			}
		}

		public Vector2 YY {
			get => new Vector2( Y, Y );
		}

		public Vector3 XXX {
			get => new Vector3( X, X, X );
		}

		public Vector3 XXY {
			get => new Vector3( X, X, Y );
		}

		public Vector3 XYX {
			get => new Vector3( X, Y, X );
		}

		public Vector3 XYY {
			get => new Vector3( X, Y, Y );
		}

		public Vector3 YXX {
			get => new Vector3( Y, X, X );
		}

		public Vector3 YXY {
			get => new Vector3( Y, X, Y );
		}

		public Vector3 YYX {
			get => new Vector3( Y, Y, X );
		}

		public Vector3 YYY {
			get => new Vector3( Y, Y, Y );
		}

		public Vector4 XXXX {
			get => new Vector4( X, X, X, X );
		}

		public Vector4 XXXY {
			get => new Vector4( X, X, X, Y );
		}

		public Vector4 XXYX {
			get => new Vector4( X, X, Y, X );
		}

		public Vector4 XXYY {
			get => new Vector4( X, X, Y, Y );
		}

		public Vector4 XYXX {
			get => new Vector4( X, Y, X, X );
		}

		public Vector4 XYXY {
			get => new Vector4( X, Y, X, Y );
		}

		public Vector4 XYYX {
			get => new Vector4( X, Y, Y, X );
		}

		public Vector4 XYYY {
			get => new Vector4( X, Y, Y, Y );
		}

		public Vector4 YXXX {
			get => new Vector4( Y, X, X, X );
		}

		public Vector4 YXXY {
			get => new Vector4( Y, X, X, Y );
		}

		public Vector4 YXYX {
			get => new Vector4( Y, X, Y, X );
		}

		public Vector4 YXYY {
			get => new Vector4( Y, X, Y, Y );
		}

		public Vector4 YYXX {
			get => new Vector4( Y, Y, X, X );
		}

		public Vector4 YYXY {
			get => new Vector4( Y, Y, X, Y );
		}

		public Vector4 YYYX {
			get => new Vector4( Y, Y, Y, X );
		}

		public Vector4 YYYY {
			get => new Vector4( Y, Y, Y, Y );
		}
		#endregion

		#region Operators
		public static Vector2 operator +( Vector2 a, Vector2 b ) {
			return new Vector2( a.X + b.X, a.Y + b.Y );
		}

		public static Vector2 operator -( Vector2 a ) {
			return new Vector2( -a.X, -a.Y );
		}

		public static Vector2 operator *( Vector2 a, float scalar ) {
			return new Vector2( a.X * scalar, a.Y * scalar );
		}

		public static Vector2 operator *( float scalar, Vector2 a ) {
			return new Vector2( a.X * scalar, a.Y * scalar );
		}

		public static Vector2 operator *( Vector2 a, Vector2 b ) {
			return new Vector2( a.X * b.X, a.Y * b.Y );
		}

		public static Vector2 operator -( Vector2 a, Vector2 b ) {
			return a + -b;
		}

		public static Vector2 operator /( Vector2 a, float divident ) {
			return a * ( 1f / divident );
		}

		public static Vector2 operator /( Vector2 a, Vector2 b ) {
			return new Vector2( a.X / b.X, a.Y / b.Y );
		}

		public static bool operator ==( Vector2 a, Vector2 b ) {
			return a.Equals( b );
		}

		public static bool operator !=( Vector2 a, Vector2 b ) {
			return !a.Equals( b );
		}

		#region Implicit
		public static implicit operator Vector2( float[] a ) {
			if( a.Length != 2 )
				throw new System.ArgumentException( "Length of array is not compatible with Vector2" );
			return new Vector2( a[ 0 ], a[ 1 ] );
		}

		public static implicit operator Vector2( float a ) {
			return new Vector2( a );
		}

		public static implicit operator float[]( Vector2 a ) {
			return new float[] { a.X, a.Y };
		}
		#endregion
		#endregion
	}
}
