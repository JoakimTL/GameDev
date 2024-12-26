using Engine.Utilities.Data;
using System;

namespace Engine.LinearAlgebra {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
	public struct Vector2ui : System.IEquatable<Vector2ui> {

		[System.Runtime.InteropServices.FieldOffset( 0 )]
		public uint X;
		[System.Runtime.InteropServices.FieldOffset( 4 )]
		public uint Y;

		public Vector2 AsFloat => new Vector2( X, Y );
		public Vector2b AsByte => new Vector2b( (byte) X, (byte) Y );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y );
		public float LengthSquared => X * X + Y * Y;

		public Vector2ui( uint x, uint y ) {
			X = x;
			Y = y;
		}

		public Vector2ui( Vector2ui a ) : this( a.X, a.Y ) { }

		public Vector2ui( uint s ) : this( s, s ) { }

		public Vector2ui( byte[] data ) : this( DataTransform.ToUInt32Array( data ) ) { }

		public static readonly Vector2i Zero = new Vector2i( 0 );

		#region Override and Equals

		public override bool Equals( object other ) {
			if( !( other is Vector2ui ) )
				return false;
			return Equals( (Vector2ui) other );
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public bool Equals( Vector2ui other ) {
			return X == other.X && Y == other.Y;
		}

		public override string ToString() {
			return $"[{X},{Y}]";
		}

		#endregion

		#region Swizzle
		public Vector2ui XX {
			get => new Vector2ui( X, X );
		}

		public Vector2ui XY {
			get => new Vector2ui( X, Y ); set {
				X = value.X;
				Y = value.Y;
			}
		}

		public Vector2ui YX {
			get => new Vector2ui( Y, X ); set {
				Y = value.X;
				X = value.Y;
			}
		}

		public Vector2ui YY {
			get => new Vector2ui( Y, Y );
		}
		#endregion

		#region Operators
		public static Vector2ui operator +( Vector2ui a, Vector2ui b ) {
			return new Vector2ui( a.X + b.X, a.Y + b.Y );
		}

		public static Vector2i operator -( Vector2ui a ) {
			return new Vector2i( (int) -(long) a.X, (int) -(long) a.Y );
		}

		public static Vector2ui operator *( Vector2ui a, uint scalar ) {
			return new Vector2ui( a.X * scalar, a.Y * scalar );
		}

		public static Vector2ui operator *( uint scalar, Vector2ui a ) {
			return new Vector2ui( a.X * scalar, a.Y * scalar );
		}

		public static Vector2 operator *( Vector2ui a, float scalar ) {
			return new Vector2( a.X * scalar, a.Y * scalar );
		}

		public static Vector2 operator *( float scalar, Vector2ui a ) {
			return new Vector2( a.X * scalar, a.Y * scalar );
		}

		public static Vector2ui operator *( Vector2ui a, Vector2ui b ) {
			return new Vector2ui( a.X * b.X, a.Y * b.Y );
		}

		public static Vector2 operator *( Vector2ui a, Vector2 b ) {
			return new Vector2( a.X * b.X, a.Y * b.Y );
		}

		public static Vector2 operator *( Vector2 a, Vector2ui b ) {
			return new Vector2( a.X * b.X, a.Y * b.Y );
		}

		public static Vector2ui operator -( Vector2ui a, Vector2ui b ) {
			return new Vector2ui( a.X - b.X, a.Y - b.Y );
		}

		public static Vector2ui operator /( Vector2ui a, uint divident ) {
			return new Vector2ui( a.X / divident, a.Y / divident );
		}

		public static Vector2 operator /( Vector2ui a, float divident ) {
			return a * ( 1f / divident );
		}

		public static bool operator ==( Vector2ui a, Vector2ui b ) {
			return a.Equals( b );
		}

		public static bool operator !=( Vector2ui a, Vector2ui b ) {
			return !a.Equals( b );
		}

		#region Implicit
		public static implicit operator Vector2ui( uint[] a ) {
			if( a.Length != 2 )
				throw new System.ArgumentException( "Length of array is not compatible with Vector2i" );
			return new Vector2ui( a[ 0 ], a[ 1 ] );
		}

		public static implicit operator Vector2ui( (uint, uint) a ) {
			return new Vector2ui( a.Item1, a.Item2 );
		}

		public static implicit operator Vector2ui( uint a ) {
			return new Vector2ui( a );
		}

		public static implicit operator uint[]( Vector2ui a ) {
			return new uint[] { a.X, a.Y };
		}
		#endregion
		#endregion

	}
}
