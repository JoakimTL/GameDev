using ED;
using System;

namespace EM {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
	public struct Vector3i : System.IEquatable<Vector3i>, ILAMeasurable, ITransferable {

		public int X, Y, Z;

		public Vector3 AsFloat => new Vector3( X, Y, Z );
		public Vector3b AsByte => new Vector3b( (byte) X, (byte) Y, (byte) Z );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y + Z * Z );
		public float LengthSquared => X * X + Y * Y + Z * Z;

		public Vector3i( int x, int y, int z ) {
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3i( Vector2i a, int z ) : this( a.X, a.Y, z ) { }

		public Vector3i( Vector3i a ) : this( a.X, a.Y, a.Z ) { }

		public Vector3i( int s ) : this( s, s, s ) { }

		public Vector3i( byte[] data ) : this( DataTransform.ToInt32Array( data ) ) { }

		public static readonly Vector3i Zero = new Vector3i( 0 );
		public static readonly Vector3i One = new Vector3i( 1 );

		#region ITransferable
		public byte[] GetTransferData() {
			return DataTransform.GetBytes( X, Y, Z );
		}
		#endregion

		#region Override and Equals

		public override bool Equals( object other ) {
			if( other is Vector3i == false )
				return false;
			return Equals( (Vector3i) other );
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}

		public bool Equals( Vector3i other ) {
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public override string ToString() {
			return $"[{X},{Y},{Z}]";
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

		public Vector2i XZ {
			get => new Vector2i( X, Z ); set {
				X = value.X;
				Z = value.Y;
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

		public Vector2i YZ {
			get => new Vector2i( Y, Z ); set {
				Y = value.X;
				Z = value.Y;
			}
		}

		public Vector2i ZX {
			get => new Vector2i( Z, X ); set {
				Z = value.X;
				X = value.Y;
			}
		}

		public Vector2i ZY {
			get => new Vector2i( Z, Y ); set {
				Z = value.X;
				Y = value.Y;
			}
		}

		public Vector2i ZZ {
			get => new Vector2i( Z, Z );
		}

		public Vector3i XXX {
			get => new Vector3i( X, X, X );
		}

		public Vector3i XXY {
			get => new Vector3i( X, X, Y );
		}

		public Vector3i XXZ {
			get => new Vector3i( X, X, Z );
		}

		public Vector3i XYX {
			get => new Vector3i( X, Y, X );
		}

		public Vector3i XYY {
			get => new Vector3i( X, Y, Y );
		}

		public Vector3i XYZ {
			get => new Vector3i( X, Y, Z ); set {
				X = value.X;
				Y = value.Y;
				Z = value.Z;
			}
		}

		public Vector3i XZX {
			get => new Vector3i( X, Z, X );
		}

		public Vector3i XZY {
			get => new Vector3i( X, Z, Y ); set {
				X = value.X;
				Z = value.Y;
				Y = value.Z;
			}
		}

		public Vector3i XZZ {
			get => new Vector3i( X, Z, Z );
		}

		public Vector3i YXX {
			get => new Vector3i( Y, X, X );
		}

		public Vector3i YXY {
			get => new Vector3i( Y, X, Y );
		}

		public Vector3i YXZ {
			get => new Vector3i( Y, X, Z ); set {
				Y = value.X;
				X = value.Y;
				Z = value.Z;
			}
		}

		public Vector3i YYX {
			get => new Vector3i( Y, Y, X );
		}

		public Vector3i YYY {
			get => new Vector3i( Y, Y, Y );
		}

		public Vector3i YYZ {
			get => new Vector3i( Y, Y, Z );
		}

		public Vector3i YZX {
			get => new Vector3i( Y, Z, X ); set {
				Y = value.X;
				Z = value.Y;
				X = value.Z;
			}
		}

		public Vector3i YZY {
			get => new Vector3i( Y, Z, Y );
		}

		public Vector3i YZZ {
			get => new Vector3i( Y, Z, Z );
		}

		public Vector3i ZXX {
			get => new Vector3i( Z, X, X );
		}

		public Vector3i ZXY {
			get => new Vector3i( Z, X, Y ); set {
				Z = value.X;
				X = value.Y;
				Y = value.Z;
			}
		}

		public Vector3i ZXZ {
			get => new Vector3i( Z, X, Z );
		}

		public Vector3i ZYX {
			get => new Vector3i( Z, Y, X ); set {
				Z = value.X;
				Y = value.Y;
				X = value.Z;
			}
		}

		public Vector3i ZYY {
			get => new Vector3i( Z, Y, Y );
		}

		public Vector3i ZYZ {
			get => new Vector3i( Z, Y, Z );
		}

		public Vector3i ZZX {
			get => new Vector3i( Z, Z, X );
		}

		public Vector3i ZZY {
			get => new Vector3i( Z, Z, Y );
		}

		public Vector3i ZZZ {
			get => new Vector3i( Z, Z, Z );
		}

		public Vector4i XXXX {
			get => new Vector4i( X, X, X, X );
		}

		public Vector4i XXXY {
			get => new Vector4i( X, X, X, Y );
		}

		public Vector4i XXXZ {
			get => new Vector4i( X, X, X, Z );
		}

		public Vector4i XXYX {
			get => new Vector4i( X, X, Y, X );
		}

		public Vector4i XXYY {
			get => new Vector4i( X, X, Y, Y );
		}

		public Vector4i XXYZ {
			get => new Vector4i( X, X, Y, Z );
		}

		public Vector4i XXZX {
			get => new Vector4i( X, X, Z, X );
		}

		public Vector4i XXZY {
			get => new Vector4i( X, X, Z, Y );
		}

		public Vector4i XXZZ {
			get => new Vector4i( X, X, Z, Z );
		}

		public Vector4i XYXX {
			get => new Vector4i( X, Y, X, X );
		}

		public Vector4i XYXY {
			get => new Vector4i( X, Y, X, Y );
		}

		public Vector4i XYXZ {
			get => new Vector4i( X, Y, X, Z );
		}

		public Vector4i XYYX {
			get => new Vector4i( X, Y, Y, X );
		}

		public Vector4i XYYY {
			get => new Vector4i( X, Y, Y, Y );
		}

		public Vector4i XYYZ {
			get => new Vector4i( X, Y, Y, Z );
		}

		public Vector4i XYZX {
			get => new Vector4i( X, Y, Z, X );
		}

		public Vector4i XYZY {
			get => new Vector4i( X, Y, Z, Y );
		}

		public Vector4i XYZZ {
			get => new Vector4i( X, Y, Z, Z );
		}

		public Vector4i XZXX {
			get => new Vector4i( X, Z, X, X );
		}

		public Vector4i XZXY {
			get => new Vector4i( X, Z, X, Y );
		}

		public Vector4i XZXZ {
			get => new Vector4i( X, Z, X, Z );
		}

		public Vector4i XZYX {
			get => new Vector4i( X, Z, Y, X );
		}

		public Vector4i XZYY {
			get => new Vector4i( X, Z, Y, Y );
		}

		public Vector4i XZYZ {
			get => new Vector4i( X, Z, Y, Z );
		}

		public Vector4i XZZX {
			get => new Vector4i( X, Z, Z, X );
		}

		public Vector4i XZZY {
			get => new Vector4i( X, Z, Z, Y );
		}

		public Vector4i XZZZ {
			get => new Vector4i( X, Z, Z, Z );
		}

		public Vector4i YXXX {
			get => new Vector4i( Y, X, X, X );
		}

		public Vector4i YXXY {
			get => new Vector4i( Y, X, X, Y );
		}

		public Vector4i YXXZ {
			get => new Vector4i( Y, X, X, Z );
		}

		public Vector4i YXYX {
			get => new Vector4i( Y, X, Y, X );
		}

		public Vector4i YXYY {
			get => new Vector4i( Y, X, Y, Y );
		}

		public Vector4i YXYZ {
			get => new Vector4i( Y, X, Y, Z );
		}

		public Vector4i YXZX {
			get => new Vector4i( Y, X, Z, X );
		}

		public Vector4i YXZY {
			get => new Vector4i( Y, X, Z, Y );
		}

		public Vector4i YXZZ {
			get => new Vector4i( Y, X, Z, Z );
		}

		public Vector4i YYXX {
			get => new Vector4i( Y, Y, X, X );
		}

		public Vector4i YYXY {
			get => new Vector4i( Y, Y, X, Y );
		}

		public Vector4i YYXZ {
			get => new Vector4i( Y, Y, X, Z );
		}

		public Vector4i YYYX {
			get => new Vector4i( Y, Y, Y, X );
		}

		public Vector4i YYYY {
			get => new Vector4i( Y, Y, Y, Y );
		}

		public Vector4i YYYZ {
			get => new Vector4i( Y, Y, Y, Z );
		}

		public Vector4i YYZX {
			get => new Vector4i( Y, Y, Z, X );
		}

		public Vector4i YYZY {
			get => new Vector4i( Y, Y, Z, Y );
		}

		public Vector4i YYZZ {
			get => new Vector4i( Y, Y, Z, Z );
		}

		public Vector4i YZXX {
			get => new Vector4i( Y, Z, X, X );
		}

		public Vector4i YZXY {
			get => new Vector4i( Y, Z, X, Y );
		}

		public Vector4i YZXZ {
			get => new Vector4i( Y, Z, X, Z );
		}

		public Vector4i YZYX {
			get => new Vector4i( Y, Z, Y, X );
		}

		public Vector4i YZYY {
			get => new Vector4i( Y, Z, Y, Y );
		}

		public Vector4i YZYZ {
			get => new Vector4i( Y, Z, Y, Z );
		}

		public Vector4i YZZX {
			get => new Vector4i( Y, Z, Z, X );
		}

		public Vector4i YZZY {
			get => new Vector4i( Y, Z, Z, Y );
		}

		public Vector4i YZZZ {
			get => new Vector4i( Y, Z, Z, Z );
		}

		public Vector4i ZXXX {
			get => new Vector4i( Z, X, X, X );
		}

		public Vector4i ZXXY {
			get => new Vector4i( Z, X, X, Y );
		}

		public Vector4i ZXXZ {
			get => new Vector4i( Z, X, X, Z );
		}

		public Vector4i ZXYX {
			get => new Vector4i( Z, X, Y, X );
		}

		public Vector4i ZXYY {
			get => new Vector4i( Z, X, Y, Y );
		}

		public Vector4i ZXYZ {
			get => new Vector4i( Z, X, Y, Z );
		}

		public Vector4i ZXZX {
			get => new Vector4i( Z, X, Z, X );
		}

		public Vector4i ZXZY {
			get => new Vector4i( Z, X, Z, Y );
		}

		public Vector4i ZXZZ {
			get => new Vector4i( Z, X, Z, Z );
		}

		public Vector4i ZYXX {
			get => new Vector4i( Z, Y, X, X );
		}

		public Vector4i ZYXY {
			get => new Vector4i( Z, Y, X, Y );
		}

		public Vector4i ZYXZ {
			get => new Vector4i( Z, Y, X, Z );
		}

		public Vector4i ZYYX {
			get => new Vector4i( Z, Y, Y, X );
		}

		public Vector4i ZYYY {
			get => new Vector4i( Z, Y, Y, Y );
		}

		public Vector4i ZYYZ {
			get => new Vector4i( Z, Y, Y, Z );
		}

		public Vector4i ZYZX {
			get => new Vector4i( Z, Y, Z, X );
		}

		public Vector4i ZYZY {
			get => new Vector4i( Z, Y, Z, Y );
		}

		public Vector4i ZYZZ {
			get => new Vector4i( Z, Y, Z, Z );
		}

		public Vector4i ZZXX {
			get => new Vector4i( Z, Z, X, X );
		}

		public Vector4i ZZXY {
			get => new Vector4i( Z, Z, X, Y );
		}

		public Vector4i ZZXZ {
			get => new Vector4i( Z, Z, X, Z );
		}

		public Vector4i ZZYX {
			get => new Vector4i( Z, Z, Y, X );
		}

		public Vector4i ZZYY {
			get => new Vector4i( Z, Z, Y, Y );
		}

		public Vector4i ZZYZ {
			get => new Vector4i( Z, Z, Y, Z );
		}

		public Vector4i ZZZX {
			get => new Vector4i( Z, Z, Z, X );
		}

		public Vector4i ZZZY {
			get => new Vector4i( Z, Z, Z, Y );
		}

		public Vector4i ZZZZ {
			get => new Vector4i( Z, Z, Z, Z );
		}
		#endregion

		#region Operators
		public static Vector3i operator +( Vector3i a, Vector3i b ) {
			return new Vector3i( a.X + b.X, a.Y + b.Y, a.Z + b.Z );
		}

		public static Vector3i operator -( Vector3i a ) {
			return new Vector3i( -a.X, -a.Y, -a.Z );
		}

		public static Vector3i operator *( Vector3i a, int scalar ) {
			return new Vector3i( a.X * scalar, a.Y * scalar, a.Z * scalar );
		}

		public static Vector3i operator *( int scalar, Vector3i a ) {
			return new Vector3i( a.X * scalar, a.Y * scalar, a.Z * scalar );
		}

		public static Vector3 operator *( Vector3i a, float scalar ) {
			return new Vector3( a.X * scalar, a.Y * scalar, a.Z * scalar );
		}

		public static Vector3 operator *( float scalar, Vector3i a ) {
			return new Vector3( a.X * scalar, a.Y * scalar, a.Z * scalar );
		}

		public static Vector3i operator *( Vector3i a, Vector3i b ) {
			return new Vector3i( a.X * b.X, a.Y * b.Y, a.Z * b.Z );
		}

		public static Vector3 operator *( Vector3i a, Vector3 b ) {
			return new Vector3( a.X * b.X, a.Y * b.Y, a.Z * b.Z );
		}

		public static Vector3 operator *( Vector3 a, Vector3i b ) {
			return new Vector3( a.X * b.X, a.Y * b.Y, a.Z * b.Z );
		}

		public static Vector3i operator -( Vector3i a, Vector3i b ) {
			return a + -b;
		}

		public static Vector3i operator /( Vector3i a, int divident ) {
			return ( a * ( 1f / divident ) ).IntCasted;
		}

		public static Vector3 operator /( Vector3i a, float divident ) {
			return a * ( 1f / divident );
		}

		public static bool operator ==( Vector3i a, Vector3i b ) {
			return a.Equals( b );
		}

		public static bool operator !=( Vector3i a, Vector3i b ) {
			return !a.Equals( b );
		}

		#region Implicit
		public static implicit operator Vector3i( int[] a ) {
			if( a.Length != 3 )
				throw new System.ArgumentException( "Length of array is not compatible with Vector3i" );
			return new Vector3i( a[ 0 ], a[ 1 ], a[ 2 ] );
		}

		public static implicit operator Vector3i( int a ) {
			return new Vector3i( a );
		}

		public static implicit operator int[]( Vector3i a ) {
			return new int[] { a.X, a.Y, a.Z };
		}
		#endregion
		#endregion

	}
}
