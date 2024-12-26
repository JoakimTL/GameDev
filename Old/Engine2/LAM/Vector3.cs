using ED;

namespace EM {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
	public struct Vector3 : System.IEquatable<Vector3>, ILANormalizable<Vector3>, ILAMeasurable, ITransferable {

		public float X, Y, Z;

		public Vector3i IntCasted => new Vector3i( (int) X, (int) Y, (int) Z );
		public Vector3i IntRounded => new Vector3i( (int) System.Math.Round( X ), (int) System.Math.Round( Y ), (int) System.Math.Round( Z ) );
		public Vector3i IntFloored => new Vector3i( (int) System.Math.Floor( X ), (int) System.Math.Floor( Y ), (int) System.Math.Floor( Z ) );
		public Vector3i IntCeiled => new Vector3i( (int) System.Math.Ceiling( X ), (int) System.Math.Ceiling( Y ), (int) System.Math.Ceiling( Z ) );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y + Z * Z );
		public float LengthSquared => X * X + Y * Y + Z * Z;

		public Vector3 Normalized => Normalize( ref this );

		public Vector3( float x, float y, float z ) {
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3( Vector2 a, float z ) : this( a.X, a.Y, z ) { }

		public Vector3( Vector3 a ) : this( a.X, a.Y, a.Z ) { }

		public Vector3( float s ) : this( s, s, s ) { }

		public Vector3( byte[] data ) : this( DataTransform.ToFloat32Array( data ) ) { }

		public static readonly Vector3 Zero = new Vector3( 0 );
		public static readonly Vector3 One = new Vector3( 1 );
		public static readonly Vector3 UnitX = new Vector3( 1, 0, 0 );
		public static readonly Vector3 UnitY = new Vector3( 0, 1, 0 );
		public static readonly Vector3 UnitZ = new Vector3( 0, 0, 1 );

		public void Normalize() {
			Normalize( ref this );
		}

		public static float Distance( Vector3 a, Vector3 b ) {
			return ( a - b ).Length;
		}

		#region ITransferable
		public byte[] GetTransferData() {
			return DataTransform.GetBytes( X, Y, Z );
		}
		#endregion

		#region Transforms
		#region Matrix4
		public static void Transform( ref Vector3 vec, ref Matrix4 mat, out Vector3 result ) {
			Vector4 v4 = new Vector4( vec.X, vec.Y, vec.Z, 1.0f );
			Vector4.Transform( ref v4, ref mat, out v4 );
			result = v4.XYZ;
		}

		public static Vector3 Transform( ref Vector3 vec, ref Matrix4 mat ) {
			Transform( ref vec, ref mat, out Vector3 result );
			return result;
		}

		public static Vector3 Transform( Vector3 vec, Matrix4 mat ) {
			Transform( ref vec, ref mat, out Vector3 result );
			return result;
		}
		#endregion
		#region Quaternion
		public static void Transform( ref Vector3 vec, ref Quaternion quat, out Vector3 result ) {
			// Since vec.W == 0, we can optimize quat * vec * quat^-1 as follows:
			// vec + 2.0 * cross(quat.xyz, cross(quat.xyz, vec) + quat.w * vec)
			Vector3 xyz = quat.XYZ, temp2;
			Cross( ref xyz, ref vec, out Vector3 temp );
			temp2 = vec * quat.W;//Vector3.Multiply( ref vec, quat.W, out temp2 );
			temp += temp2;//Vector3.Add( ref temp, ref temp2, out temp );
			Cross( ref xyz, ref temp, out temp );
			temp *= 2;//Vector3.Multiply( ref temp, 2, out temp );
			result = vec + temp; //Vector3.Add( ref vec, ref temp, out result );
		}

		public static Vector3 Transform( ref Vector3 vec, ref Quaternion quat ) {
			Transform( ref vec, ref quat, out Vector3 r );
			return r;
		}

		public static Vector3 Transform( Vector3 vec, Quaternion quat ) {
			Transform( ref vec, ref quat, out Vector3 r );
			return r;
		}
		#endregion
		#endregion

		#region Normalize

		public static void Normalize( ref Vector3 a, out Vector3 result ) {
			float l = a.Length;
			if( l == 0 )
				result = 0;
			else
				result = a / a.Length;
		}

		public static Vector3 Normalize( ref Vector3 a ) {
			Normalize( ref a, out Vector3 r );
			return r;
		}

		public static Vector3 Normalize( Vector3 a ) {
			Normalize( ref a, out Vector3 r );
			return r;
		}

		#endregion

		#region Dot Product

		public static float Dot( ref Vector3 a, ref Vector3 b ) {
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		public static float Dot( Vector3 a, Vector3 b ) {
			return Dot( ref a, ref b );
		}

		#endregion

		#region Cross Product

		public static void Cross( ref Vector3 a, ref Vector3 b, out Vector3 result ) {
			result = new Vector3(
				a.Y * b.Z - a.Z * b.Y,
				a.Z * b.X - a.X * b.Z,
				a.X * b.Y - a.Y * b.X
			);
		}

		public static Vector3 Cross( ref Vector3 a, ref Vector3 b ) {
			Cross( ref a, ref b, out Vector3 r );
			return r;
		}

		public static Vector3 Cross( Vector3 a, Vector3 b ) {
			Cross( ref a, ref b, out Vector3 r );
			return r;
		}

		#endregion

		#region Override and Equals

		public override bool Equals( object other ) {
			if( other is Vector3 == false )
				return false;
			return Equals( (Vector3) other );
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}

		public bool Equals( Vector3 other ) {
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public override string ToString() {
			return $"[{string.Format( "{0,6:F2}", X )},{string.Format( "{0,6:F2}", Y )},{string.Format( "{0,6:F2}", Z )}]";
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

		public Vector2 XZ {
			get => new Vector2( X, Z ); set {
				X = value.X;
				Z = value.Y;
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

		public Vector2 YZ {
			get => new Vector2( Y, Z ); set {
				Y = value.X;
				Z = value.Y;
			}
		}

		public Vector2 ZX {
			get => new Vector2( Z, X ); set {
				Z = value.X;
				X = value.Y;
			}
		}

		public Vector2 ZY {
			get => new Vector2( Z, Y ); set {
				Z = value.X;
				Y = value.Y;
			}
		}

		public Vector2 ZZ {
			get => new Vector2( Z, Z );
		}

		public Vector3 XXX {
			get => new Vector3( X, X, X );
		}

		public Vector3 XXY {
			get => new Vector3( X, X, Y );
		}

		public Vector3 XXZ {
			get => new Vector3( X, X, Z );
		}

		public Vector3 XYX {
			get => new Vector3( X, Y, X );
		}

		public Vector3 XYY {
			get => new Vector3( X, Y, Y );
		}

		public Vector3 XYZ {
			get => new Vector3( X, Y, Z ); set {
				X = value.X;
				Y = value.Y;
				Z = value.Z;
			}
		}

		public Vector3 XZX {
			get => new Vector3( X, Z, X );
		}

		public Vector3 XZY {
			get => new Vector3( X, Z, Y ); set {
				X = value.X;
				Z = value.Y;
				Y = value.Z;
			}
		}

		public Vector3 XZZ {
			get => new Vector3( X, Z, Z );
		}

		public Vector3 YXX {
			get => new Vector3( Y, X, X );
		}

		public Vector3 YXY {
			get => new Vector3( Y, X, Y );
		}

		public Vector3 YXZ {
			get => new Vector3( Y, X, Z ); set {
				Y = value.X;
				X = value.Y;
				Z = value.Z;
			}
		}

		public Vector3 YYX {
			get => new Vector3( Y, Y, X );
		}

		public Vector3 YYY {
			get => new Vector3( Y, Y, Y );
		}

		public Vector3 YYZ {
			get => new Vector3( Y, Y, Z );
		}

		public Vector3 YZX {
			get => new Vector3( Y, Z, X ); set {
				Y = value.X;
				Z = value.Y;
				X = value.Z;
			}
		}

		public Vector3 YZY {
			get => new Vector3( Y, Z, Y );
		}

		public Vector3 YZZ {
			get => new Vector3( Y, Z, Z );
		}

		public Vector3 ZXX {
			get => new Vector3( Z, X, X );
		}

		public Vector3 ZXY {
			get => new Vector3( Z, X, Y ); set {
				Z = value.X;
				X = value.Y;
				Y = value.Z;
			}
		}

		public Vector3 ZXZ {
			get => new Vector3( Z, X, Z );
		}

		public Vector3 ZYX {
			get => new Vector3( Z, Y, X ); set {
				Z = value.X;
				Y = value.Y;
				X = value.Z;
			}
		}

		public Vector3 ZYY {
			get => new Vector3( Z, Y, Y );
		}

		public Vector3 ZYZ {
			get => new Vector3( Z, Y, Z );
		}

		public Vector3 ZZX {
			get => new Vector3( Z, Z, X );
		}

		public Vector3 ZZY {
			get => new Vector3( Z, Z, Y );
		}

		public Vector3 ZZZ {
			get => new Vector3( Z, Z, Z );
		}

		public Vector4 XXXX {
			get => new Vector4( X, X, X, X );
		}

		public Vector4 XXXY {
			get => new Vector4( X, X, X, Y );
		}

		public Vector4 XXXZ {
			get => new Vector4( X, X, X, Z );
		}

		public Vector4 XXYX {
			get => new Vector4( X, X, Y, X );
		}

		public Vector4 XXYY {
			get => new Vector4( X, X, Y, Y );
		}

		public Vector4 XXYZ {
			get => new Vector4( X, X, Y, Z );
		}

		public Vector4 XXZX {
			get => new Vector4( X, X, Z, X );
		}

		public Vector4 XXZY {
			get => new Vector4( X, X, Z, Y );
		}

		public Vector4 XXZZ {
			get => new Vector4( X, X, Z, Z );
		}

		public Vector4 XYXX {
			get => new Vector4( X, Y, X, X );
		}

		public Vector4 XYXY {
			get => new Vector4( X, Y, X, Y );
		}

		public Vector4 XYXZ {
			get => new Vector4( X, Y, X, Z );
		}

		public Vector4 XYYX {
			get => new Vector4( X, Y, Y, X );
		}

		public Vector4 XYYY {
			get => new Vector4( X, Y, Y, Y );
		}

		public Vector4 XYYZ {
			get => new Vector4( X, Y, Y, Z );
		}

		public Vector4 XYZX {
			get => new Vector4( X, Y, Z, X );
		}

		public Vector4 XYZY {
			get => new Vector4( X, Y, Z, Y );
		}

		public Vector4 XYZZ {
			get => new Vector4( X, Y, Z, Z );
		}

		public Vector4 XZXX {
			get => new Vector4( X, Z, X, X );
		}

		public Vector4 XZXY {
			get => new Vector4( X, Z, X, Y );
		}

		public Vector4 XZXZ {
			get => new Vector4( X, Z, X, Z );
		}

		public Vector4 XZYX {
			get => new Vector4( X, Z, Y, X );
		}

		public Vector4 XZYY {
			get => new Vector4( X, Z, Y, Y );
		}

		public Vector4 XZYZ {
			get => new Vector4( X, Z, Y, Z );
		}

		public Vector4 XZZX {
			get => new Vector4( X, Z, Z, X );
		}

		public Vector4 XZZY {
			get => new Vector4( X, Z, Z, Y );
		}

		public Vector4 XZZZ {
			get => new Vector4( X, Z, Z, Z );
		}

		public Vector4 YXXX {
			get => new Vector4( Y, X, X, X );
		}

		public Vector4 YXXY {
			get => new Vector4( Y, X, X, Y );
		}

		public Vector4 YXXZ {
			get => new Vector4( Y, X, X, Z );
		}

		public Vector4 YXYX {
			get => new Vector4( Y, X, Y, X );
		}

		public Vector4 YXYY {
			get => new Vector4( Y, X, Y, Y );
		}

		public Vector4 YXYZ {
			get => new Vector4( Y, X, Y, Z );
		}

		public Vector4 YXZX {
			get => new Vector4( Y, X, Z, X );
		}

		public Vector4 YXZY {
			get => new Vector4( Y, X, Z, Y );
		}

		public Vector4 YXZZ {
			get => new Vector4( Y, X, Z, Z );
		}

		public Vector4 YYXX {
			get => new Vector4( Y, Y, X, X );
		}

		public Vector4 YYXY {
			get => new Vector4( Y, Y, X, Y );
		}

		public Vector4 YYXZ {
			get => new Vector4( Y, Y, X, Z );
		}

		public Vector4 YYYX {
			get => new Vector4( Y, Y, Y, X );
		}

		public Vector4 YYYY {
			get => new Vector4( Y, Y, Y, Y );
		}

		public Vector4 YYYZ {
			get => new Vector4( Y, Y, Y, Z );
		}

		public Vector4 YYZX {
			get => new Vector4( Y, Y, Z, X );
		}

		public Vector4 YYZY {
			get => new Vector4( Y, Y, Z, Y );
		}

		public Vector4 YYZZ {
			get => new Vector4( Y, Y, Z, Z );
		}

		public Vector4 YZXX {
			get => new Vector4( Y, Z, X, X );
		}

		public Vector4 YZXY {
			get => new Vector4( Y, Z, X, Y );
		}

		public Vector4 YZXZ {
			get => new Vector4( Y, Z, X, Z );
		}

		public Vector4 YZYX {
			get => new Vector4( Y, Z, Y, X );
		}

		public Vector4 YZYY {
			get => new Vector4( Y, Z, Y, Y );
		}

		public Vector4 YZYZ {
			get => new Vector4( Y, Z, Y, Z );
		}

		public Vector4 YZZX {
			get => new Vector4( Y, Z, Z, X );
		}

		public Vector4 YZZY {
			get => new Vector4( Y, Z, Z, Y );
		}

		public Vector4 YZZZ {
			get => new Vector4( Y, Z, Z, Z );
		}

		public Vector4 ZXXX {
			get => new Vector4( Z, X, X, X );
		}

		public Vector4 ZXXY {
			get => new Vector4( Z, X, X, Y );
		}

		public Vector4 ZXXZ {
			get => new Vector4( Z, X, X, Z );
		}

		public Vector4 ZXYX {
			get => new Vector4( Z, X, Y, X );
		}

		public Vector4 ZXYY {
			get => new Vector4( Z, X, Y, Y );
		}

		public Vector4 ZXYZ {
			get => new Vector4( Z, X, Y, Z );
		}

		public Vector4 ZXZX {
			get => new Vector4( Z, X, Z, X );
		}

		public Vector4 ZXZY {
			get => new Vector4( Z, X, Z, Y );
		}

		public Vector4 ZXZZ {
			get => new Vector4( Z, X, Z, Z );
		}

		public Vector4 ZYXX {
			get => new Vector4( Z, Y, X, X );
		}

		public Vector4 ZYXY {
			get => new Vector4( Z, Y, X, Y );
		}

		public Vector4 ZYXZ {
			get => new Vector4( Z, Y, X, Z );
		}

		public Vector4 ZYYX {
			get => new Vector4( Z, Y, Y, X );
		}

		public Vector4 ZYYY {
			get => new Vector4( Z, Y, Y, Y );
		}

		public Vector4 ZYYZ {
			get => new Vector4( Z, Y, Y, Z );
		}

		public Vector4 ZYZX {
			get => new Vector4( Z, Y, Z, X );
		}

		public Vector4 ZYZY {
			get => new Vector4( Z, Y, Z, Y );
		}

		public Vector4 ZYZZ {
			get => new Vector4( Z, Y, Z, Z );
		}

		public Vector4 ZZXX {
			get => new Vector4( Z, Z, X, X );
		}

		public Vector4 ZZXY {
			get => new Vector4( Z, Z, X, Y );
		}

		public Vector4 ZZXZ {
			get => new Vector4( Z, Z, X, Z );
		}

		public Vector4 ZZYX {
			get => new Vector4( Z, Z, Y, X );
		}

		public Vector4 ZZYY {
			get => new Vector4( Z, Z, Y, Y );
		}

		public Vector4 ZZYZ {
			get => new Vector4( Z, Z, Y, Z );
		}

		public Vector4 ZZZX {
			get => new Vector4( Z, Z, Z, X );
		}

		public Vector4 ZZZY {
			get => new Vector4( Z, Z, Z, Y );
		}

		public Vector4 ZZZZ {
			get => new Vector4( Z, Z, Z, Z );
		}
		#endregion

		#region Operators
		public static Vector3 operator +( Vector3 a, Vector3 b ) {
			return new Vector3( a.X + b.X, a.Y + b.Y, a.Z + b.Z );
		}

		public static Vector3 operator -( Vector3 a ) {
			return new Vector3( -a.X, -a.Y, -a.Z );
		}

		public static Vector3 operator *( Vector3 a, float scalar ) {
			return new Vector3( a.X * scalar, a.Y * scalar, a.Z * scalar );
		}

		public static Vector3 operator *( float scalar, Vector3 a ) {
			return new Vector3( a.X * scalar, a.Y * scalar, a.Z * scalar );
		}

		public static Vector3 operator *( Vector3 a, Vector3 b ) {
			return new Vector3( a.X * b.X, a.Y * b.Y, a.Z * b.Z );
		}

		public static Vector3 operator -( Vector3 a, Vector3 b ) {
			return a + -b;
		}

		public static Vector3 operator /( Vector3 a, float divident ) {
			return a * ( 1f / divident );
		}

		public static Vector3 operator /( Vector3 a, Vector3 b ) {
			return new Vector3( a.X / b.X, a.Y / b.Y, a.Z / b.Z );
		}

		public static bool operator ==( Vector3 a, Vector3 b ) {
			return a.Equals( b );
		}

		public static bool operator !=( Vector3 a, Vector3 b ) {
			return !a.Equals( b );
		}

		#region Implicit
		public static implicit operator Vector3( float[] a ) {
			if( a.Length != 3 )
				throw new System.ArgumentException( "Length of array is not compatible with Vector3" );
			return new Vector3( a[ 0 ], a[ 1 ], a[ 2 ] );
		}

		public static implicit operator Vector3( float a ) {
			return new Vector3( a );
		}

		public static implicit operator float[]( Vector3 a ) {
			return new float[] { a.X, a.Y, a.Z };
		}
		#endregion
		#endregion
	}
}
