﻿using Engine.Utilities.Data;

namespace Engine.LinearAlgebra {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
	public struct Vector4 : System.IEquatable<Vector4> {

		[System.Runtime.InteropServices.FieldOffset( 0 )]
		public float X;
		[System.Runtime.InteropServices.FieldOffset( 4 )]
		public float Y;
		[System.Runtime.InteropServices.FieldOffset( 8 )]
		public float Z;
		[System.Runtime.InteropServices.FieldOffset( 12 )]
		public float W;

		public Quaternion AsQuaternion => new Quaternion( X, Y, Z, W );

		public Vector4i IntCasted => new Vector4i( (int) X, (int) Y, (int) Z, (int) W );
		public Vector4i IntRounded => new Vector4i( (int) System.Math.Round( X ), (int) System.Math.Round( Y ), (int) System.Math.Round( Z ), (int) System.Math.Round( W ) );
		public Vector4i IntFloored => new Vector4i( (int) System.Math.Floor( X ), (int) System.Math.Floor( Y ), (int) System.Math.Floor( Z ), (int) System.Math.Floor( W ) );
		public Vector4i IntCeiled => new Vector4i( (int) System.Math.Ceiling( X ), (int) System.Math.Ceiling( Y ), (int) System.Math.Ceiling( Z ), (int) System.Math.Ceiling( W ) );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y + Z * Z + W * W );
		public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

		public Vector4 Normalized => Normalize( ref this );

		public Vector4( float x, float y, float z, float w ) {
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4( Vector2 a, float z, float w ) : this( a.X, a.Y, z, w ) {
		}
		public Vector4( Vector2 a, Vector2 b ) : this( a.X, a.Y, b.X, b.Y ) { }

		public Vector4( Vector3 a, float w ) : this( a.X, a.Y, a.Z, w ) { }

		public Vector4( Vector4 a ) : this( a.X, a.Y, a.Z, a.W ) { }

		public Vector4( float s ) : this( s, s, s, s ) { }

		public Vector4( byte[] data ) : this( DataTransform.ToFloat32Array( data ) ) { }

		public static readonly Vector4 Zero = new Vector4( 0 );
		public static readonly Vector4 One = new Vector4( 1 );
		public static readonly Vector4 UnitX = new Vector4( 1, 0, 0, 0 );
		public static readonly Vector4 UnitY = new Vector4( 0, 1, 0, 0 );
		public static readonly Vector4 UnitZ = new Vector4( 0, 0, 1, 0 );
		public static readonly Vector4 UnitW = new Vector4( 0, 0, 0, 1 );

		public void Normalize() {
			Normalize( ref this );
		}

		public static float Distance( Vector4 a, Vector4 b ) {
			return ( a - b ).Length;
		}

		#region Transforms
		#region Matrix4
		public static void Transform( ref Vector4 vec, ref Matrix4 mat, out Vector4 result ) {
			result = new Vector4(
				vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X + vec.W * mat.Row3.X,
				vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y + vec.W * mat.Row3.Y,
				vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z + vec.W * mat.Row3.Z,
				vec.X * mat.Row0.W + vec.Y * mat.Row1.W + vec.Z * mat.Row2.W + vec.W * mat.Row3.W );
		}

		public static Vector4 Transform( ref Vector4 vec, ref Matrix4 mat ) {
			Transform( ref vec, ref mat, out Vector4 result );
			return result;
		}

		public static Vector4 Transform( Vector4 vec, Matrix4 mat ) {
			Transform( ref vec, ref mat, out Vector4 result );
			return result;
		}
		#endregion
		#region Quaternion
		public static void Transform( ref Vector4 vec, ref Quaternion quat, out Vector4 result ) {
			Quaternion v = new Quaternion( vec.X, vec.Y, vec.Z, vec.W );
			Quaternion.Invert( ref quat, out Quaternion i );
			Quaternion.Multiply( ref quat, ref v, out Quaternion t );
			Quaternion.Multiply( ref t, ref i, out v );

			result = new Vector4( v.X, v.Y, v.Z, v.W );
		}

		public static Vector4 Transform( ref Vector4 vec, ref Quaternion quat ) {
			Transform( ref vec, ref quat, out Vector4 r );
			return r;
		}

		public static Vector4 Transform( Vector4 vec, Quaternion quat ) {
			Transform( ref vec, ref quat, out Vector4 r );
			return r;
		}

		#endregion
		#endregion

		#region Normalize

		public static void Normalize( ref Vector4 a, out Vector4 result ) {
			result = a / a.Length;
		}

		public static Vector4 Normalize( ref Vector4 a ) {
			Normalize( ref a, out Vector4 r );
			return r;
		}

		public static Vector4 Normalize( Vector4 a ) {
			Normalize( ref a, out Vector4 r );
			return r;
		}

		#endregion

		#region Dot Product

		public static float Dot( ref Vector4 a, ref Vector4 b ) {
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
		}

		public static float Dot( Vector4 a, Vector4 b ) {
			return Dot( ref a, ref b );
		}

		#endregion

		#region Override and Equals

		public override bool Equals( object other ) {
			if( other is Vector4 == false )
				return false;
			return Equals( (Vector4) other );
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
		}

		public bool Equals( Vector4 other ) {
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

		public override string ToString() {
			return $"[{string.Format( "{0,6:F2}", X )},{string.Format( "{0,6:F2}", Y )},{string.Format( "{0,6:F2}", Z )},{string.Format( "{0,6:F2}", W )}]";
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

		public Vector2 XW {
			get => new Vector2( X, W ); set {
				X = value.X;
				W = value.Y;
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

		public Vector2 YW {
			get => new Vector2( Y, W ); set {
				Y = value.X;
				W = value.Y;
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

		public Vector2 ZW {
			get => new Vector2( Z, W ); set {
				Z = value.X;
				W = value.Y;
			}
		}

		public Vector2 WX {
			get => new Vector2( W, X ); set {
				W = value.X;
				X = value.Y;
			}
		}

		public Vector2 WY {
			get => new Vector2( W, Y ); set {
				W = value.X;
				Y = value.Y;
			}
		}

		public Vector2 WZ {
			get => new Vector2( W, Z ); set {
				W = value.X;
				Z = value.Y;
			}
		}

		public Vector2 WW {
			get => new Vector2( W, W );
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

		public Vector3 XXW {
			get => new Vector3( X, X, W );
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

		public Vector3 XYW {
			get => new Vector3( X, Y, W ); set {
				X = value.X;
				Y = value.Y;
				W = value.Z;
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

		public Vector3 XZW {
			get => new Vector3( X, Z, W ); set {
				X = value.X;
				Z = value.Y;
				W = value.Z;
			}
		}

		public Vector3 XWX {
			get => new Vector3( X, W, X );
		}

		public Vector3 XWY {
			get => new Vector3( X, W, Y ); set {
				X = value.X;
				W = value.Y;
				Y = value.Z;
			}
		}

		public Vector3 XWZ {
			get => new Vector3( X, W, Z ); set {
				X = value.X;
				W = value.Y;
				Z = value.Z;
			}
		}

		public Vector3 XWW {
			get => new Vector3( X, W, W );
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

		public Vector3 YXW {
			get => new Vector3( Y, X, W ); set {
				Y = value.X;
				X = value.Y;
				W = value.Z;
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

		public Vector3 YYW {
			get => new Vector3( Y, Y, W );
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

		public Vector3 YZW {
			get => new Vector3( Y, Z, W ); set {
				Y = value.X;
				Z = value.Y;
				W = value.Z;
			}
		}

		public Vector3 YWX {
			get => new Vector3( Y, W, X ); set {
				Y = value.X;
				W = value.Y;
				X = value.Z;
			}
		}

		public Vector3 YWY {
			get => new Vector3( Y, W, Y );
		}

		public Vector3 YWZ {
			get => new Vector3( Y, W, Z ); set {
				Y = value.X;
				W = value.Y;
				Z = value.Z;
			}
		}

		public Vector3 YWW {
			get => new Vector3( Y, W, W );
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

		public Vector3 ZXW {
			get => new Vector3( Z, X, W ); set {
				Z = value.X;
				X = value.Y;
				W = value.Z;
			}
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

		public Vector3 ZYW {
			get => new Vector3( Z, Y, W ); set {
				Z = value.X;
				Y = value.Y;
				W = value.Z;
			}
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

		public Vector3 ZZW {
			get => new Vector3( Z, Z, W );
		}

		public Vector3 ZWX {
			get => new Vector3( Z, W, X ); set {
				Z = value.X;
				W = value.Y;
				X = value.Z;
			}
		}

		public Vector3 ZWY {
			get => new Vector3( Z, W, Y ); set {
				Z = value.X;
				W = value.Y;
				Y = value.Z;
			}
		}

		public Vector3 ZWZ {
			get => new Vector3( Z, W, Z );
		}

		public Vector3 ZWW {
			get => new Vector3( Z, W, W );
		}

		public Vector3 WXX {
			get => new Vector3( W, X, X );
		}

		public Vector3 WXY {
			get => new Vector3( W, X, Y ); set {
				W = value.X;
				X = value.Y;
				Y = value.Z;
			}
		}

		public Vector3 WXZ {
			get => new Vector3( W, X, Z ); set {
				W = value.X;
				X = value.Y;
				Z = value.Z;
			}
		}

		public Vector3 WXW {
			get => new Vector3( W, X, W );
		}

		public Vector3 WYX {
			get => new Vector3( W, Y, X ); set {
				W = value.X;
				Y = value.Y;
				X = value.Z;
			}
		}

		public Vector3 WYY {
			get => new Vector3( W, Y, Y );
		}

		public Vector3 WYZ {
			get => new Vector3( W, Y, Z ); set {
				W = value.X;
				Y = value.Y;
				Z = value.Z;
			}
		}

		public Vector3 WYW {
			get => new Vector3( W, Y, W );
		}

		public Vector3 WZX {
			get => new Vector3( W, Z, X ); set {
				W = value.X;
				Z = value.Y;
				X = value.Z;
			}
		}

		public Vector3 WZY {
			get => new Vector3( W, Z, Y ); set {
				W = value.X;
				Z = value.Y;
				Y = value.Z;
			}
		}

		public Vector3 WZZ {
			get => new Vector3( W, Z, Z );
		}

		public Vector3 WZW {
			get => new Vector3( W, Z, W );
		}

		public Vector3 WWX {
			get => new Vector3( W, W, X );
		}

		public Vector3 WWY {
			get => new Vector3( W, W, Y );
		}

		public Vector3 WWZ {
			get => new Vector3( W, W, Z );
		}

		public Vector3 WWW {
			get => new Vector3( W, W, W );
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

		public Vector4 XXXW {
			get => new Vector4( X, X, X, W );
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

		public Vector4 XXYW {
			get => new Vector4( X, X, Y, W );
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

		public Vector4 XXZW {
			get => new Vector4( X, X, Z, W );
		}

		public Vector4 XXWX {
			get => new Vector4( X, X, W, X );
		}

		public Vector4 XXWY {
			get => new Vector4( X, X, W, Y );
		}

		public Vector4 XXWZ {
			get => new Vector4( X, X, W, Z );
		}

		public Vector4 XXWW {
			get => new Vector4( X, X, W, W );
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

		public Vector4 XYXW {
			get => new Vector4( X, Y, X, W );
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

		public Vector4 XYYW {
			get => new Vector4( X, Y, Y, W );
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

		public Vector4 XYZW {
			get => new Vector4( X, Y, Z, W ); set {
				X = value.X;
				Y = value.Y;
				Z = value.Z;
				W = value.W;
			}
		}

		public Vector4 XYWX {
			get => new Vector4( X, Y, W, X );
		}

		public Vector4 XYWY {
			get => new Vector4( X, Y, W, Y );
		}

		public Vector4 XYWZ {
			get => new Vector4( X, Y, W, Z ); set {
				X = value.X;
				Y = value.Y;
				W = value.Z;
				Z = value.W;
			}
		}

		public Vector4 XYWW {
			get => new Vector4( X, Y, W, W );
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

		public Vector4 XZXW {
			get => new Vector4( X, Z, X, W );
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

		public Vector4 XZYW {
			get => new Vector4( X, Z, Y, W ); set {
				X = value.X;
				Z = value.Y;
				Y = value.Z;
				W = value.W;
			}
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

		public Vector4 XZZW {
			get => new Vector4( X, Z, Z, W );
		}

		public Vector4 XZWX {
			get => new Vector4( X, Z, W, X );
		}

		public Vector4 XZWY {
			get => new Vector4( X, Z, W, Y ); set {
				X = value.X;
				Z = value.Y;
				W = value.Z;
				Y = value.W;
			}
		}

		public Vector4 XZWZ {
			get => new Vector4( X, Z, W, Z );
		}

		public Vector4 XZWW {
			get => new Vector4( X, Z, W, W );
		}

		public Vector4 XWXX {
			get => new Vector4( X, W, X, X );
		}

		public Vector4 XWXY {
			get => new Vector4( X, W, X, Y );
		}

		public Vector4 XWXZ {
			get => new Vector4( X, W, X, Z );
		}

		public Vector4 XWXW {
			get => new Vector4( X, W, X, W );
		}

		public Vector4 XWYX {
			get => new Vector4( X, W, Y, X );
		}

		public Vector4 XWYY {
			get => new Vector4( X, W, Y, Y );
		}

		public Vector4 XWYZ {
			get => new Vector4( X, W, Y, Z ); set {
				X = value.X;
				W = value.Y;
				Y = value.Z;
				Z = value.W;
			}
		}

		public Vector4 XWYW {
			get => new Vector4( X, W, Y, W );
		}

		public Vector4 XWZX {
			get => new Vector4( X, W, Z, X );
		}

		public Vector4 XWZY {
			get => new Vector4( X, W, Z, Y ); set {
				X = value.X;
				W = value.Y;
				Z = value.Z;
				Y = value.W;
			}
		}

		public Vector4 XWZZ {
			get => new Vector4( X, W, Z, Z );
		}

		public Vector4 XWZW {
			get => new Vector4( X, W, Z, W );
		}

		public Vector4 XWWX {
			get => new Vector4( X, W, W, X );
		}

		public Vector4 XWWY {
			get => new Vector4( X, W, W, Y );
		}

		public Vector4 XWWZ {
			get => new Vector4( X, W, W, Z );
		}

		public Vector4 XWWW {
			get => new Vector4( X, W, W, W );
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

		public Vector4 YXXW {
			get => new Vector4( Y, X, X, W );
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

		public Vector4 YXYW {
			get => new Vector4( Y, X, Y, W );
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

		public Vector4 YXZW {
			get => new Vector4( Y, X, Z, W ); set {
				Y = value.X;
				X = value.Y;
				Z = value.Z;
				W = value.W;
			}
		}

		public Vector4 YXWX {
			get => new Vector4( Y, X, W, X );
		}

		public Vector4 YXWY {
			get => new Vector4( Y, X, W, Y );
		}

		public Vector4 YXWZ {
			get => new Vector4( Y, X, W, Z ); set {
				Y = value.X;
				X = value.Y;
				W = value.Z;
				Z = value.W;
			}
		}

		public Vector4 YXWW {
			get => new Vector4( Y, X, W, W );
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

		public Vector4 YYXW {
			get => new Vector4( Y, Y, X, W );
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

		public Vector4 YYYW {
			get => new Vector4( Y, Y, Y, W );
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

		public Vector4 YYZW {
			get => new Vector4( Y, Y, Z, W );
		}

		public Vector4 YYWX {
			get => new Vector4( Y, Y, W, X );
		}

		public Vector4 YYWY {
			get => new Vector4( Y, Y, W, Y );
		}

		public Vector4 YYWZ {
			get => new Vector4( Y, Y, W, Z );
		}

		public Vector4 YYWW {
			get => new Vector4( Y, Y, W, W );
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

		public Vector4 YZXW {
			get => new Vector4( Y, Z, X, W ); set {
				Y = value.X;
				Z = value.Y;
				X = value.Z;
				W = value.W;
			}
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

		public Vector4 YZYW {
			get => new Vector4( Y, Z, Y, W );
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

		public Vector4 YZZW {
			get => new Vector4( Y, Z, Z, W );
		}

		public Vector4 YZWX {
			get => new Vector4( Y, Z, W, X ); set {
				Y = value.X;
				Z = value.Y;
				W = value.Z;
				X = value.W;
			}
		}

		public Vector4 YZWY {
			get => new Vector4( Y, Z, W, Y );
		}

		public Vector4 YZWZ {
			get => new Vector4( Y, Z, W, Z );
		}

		public Vector4 YZWW {
			get => new Vector4( Y, Z, W, W );
		}

		public Vector4 YWXX {
			get => new Vector4( Y, W, X, X );
		}

		public Vector4 YWXY {
			get => new Vector4( Y, W, X, Y );
		}

		public Vector4 YWXZ {
			get => new Vector4( Y, W, X, Z ); set {
				Y = value.X;
				W = value.Y;
				X = value.Z;
				Z = value.W;
			}
		}

		public Vector4 YWXW {
			get => new Vector4( Y, W, X, W );
		}

		public Vector4 YWYX {
			get => new Vector4( Y, W, Y, X );
		}

		public Vector4 YWYY {
			get => new Vector4( Y, W, Y, Y );
		}

		public Vector4 YWYZ {
			get => new Vector4( Y, W, Y, Z );
		}

		public Vector4 YWYW {
			get => new Vector4( Y, W, Y, W );
		}

		public Vector4 YWZX {
			get => new Vector4( Y, W, Z, X ); set {
				Y = value.X;
				W = value.Y;
				Z = value.Z;
				X = value.W;
			}
		}

		public Vector4 YWZY {
			get => new Vector4( Y, W, Z, Y );
		}

		public Vector4 YWZZ {
			get => new Vector4( Y, W, Z, Z );
		}

		public Vector4 YWZW {
			get => new Vector4( Y, W, Z, W );
		}

		public Vector4 YWWX {
			get => new Vector4( Y, W, W, X );
		}

		public Vector4 YWWY {
			get => new Vector4( Y, W, W, Y );
		}

		public Vector4 YWWZ {
			get => new Vector4( Y, W, W, Z );
		}

		public Vector4 YWWW {
			get => new Vector4( Y, W, W, W );
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

		public Vector4 ZXXW {
			get => new Vector4( Z, X, X, W );
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

		public Vector4 ZXYW {
			get => new Vector4( Z, X, Y, W ); set {
				Z = value.X;
				X = value.Y;
				Y = value.Z;
				W = value.W;
			}
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

		public Vector4 ZXZW {
			get => new Vector4( Z, X, Z, W );
		}

		public Vector4 ZXWX {
			get => new Vector4( Z, X, W, X );
		}

		public Vector4 ZXWY {
			get => new Vector4( Z, X, W, Y ); set {
				Z = value.X;
				X = value.Y;
				W = value.Z;
				Y = value.W;
			}
		}

		public Vector4 ZXWZ {
			get => new Vector4( Z, X, W, Z );
		}

		public Vector4 ZXWW {
			get => new Vector4( Z, X, W, W );
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

		public Vector4 ZYXW {
			get => new Vector4( Z, Y, X, W ); set {
				Z = value.X;
				Y = value.Y;
				X = value.Z;
				W = value.W;
			}
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

		public Vector4 ZYYW {
			get => new Vector4( Z, Y, Y, W );
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

		public Vector4 ZYZW {
			get => new Vector4( Z, Y, Z, W );
		}

		public Vector4 ZYWX {
			get => new Vector4( Z, Y, W, X ); set {
				Z = value.X;
				Y = value.Y;
				W = value.Z;
				X = value.W;
			}
		}

		public Vector4 ZYWY {
			get => new Vector4( Z, Y, W, Y );
		}

		public Vector4 ZYWZ {
			get => new Vector4( Z, Y, W, Z );
		}

		public Vector4 ZYWW {
			get => new Vector4( Z, Y, W, W );
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

		public Vector4 ZZXW {
			get => new Vector4( Z, Z, X, W );
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

		public Vector4 ZZYW {
			get => new Vector4( Z, Z, Y, W );
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

		public Vector4 ZZZW {
			get => new Vector4( Z, Z, Z, W );
		}

		public Vector4 ZZWX {
			get => new Vector4( Z, Z, W, X );
		}

		public Vector4 ZZWY {
			get => new Vector4( Z, Z, W, Y );
		}

		public Vector4 ZZWZ {
			get => new Vector4( Z, Z, W, Z );
		}

		public Vector4 ZZWW {
			get => new Vector4( Z, Z, W, W );
		}

		public Vector4 ZWXX {
			get => new Vector4( Z, W, X, X );
		}

		public Vector4 ZWXY {
			get => new Vector4( Z, W, X, Y ); set {
				Z = value.X;
				W = value.Y;
				X = value.Z;
				Y = value.W;
			}
		}

		public Vector4 ZWXZ {
			get => new Vector4( Z, W, X, Z );
		}

		public Vector4 ZWXW {
			get => new Vector4( Z, W, X, W );
		}

		public Vector4 ZWYX {
			get => new Vector4( Z, W, Y, X ); set {
				Z = value.X;
				W = value.Y;
				Y = value.Z;
				X = value.W;
			}
		}

		public Vector4 ZWYY {
			get => new Vector4( Z, W, Y, Y );
		}

		public Vector4 ZWYZ {
			get => new Vector4( Z, W, Y, Z );
		}

		public Vector4 ZWYW {
			get => new Vector4( Z, W, Y, W );
		}

		public Vector4 ZWZX {
			get => new Vector4( Z, W, Z, X );
		}

		public Vector4 ZWZY {
			get => new Vector4( Z, W, Z, Y );
		}

		public Vector4 ZWZZ {
			get => new Vector4( Z, W, Z, Z );
		}

		public Vector4 ZWZW {
			get => new Vector4( Z, W, Z, W );
		}

		public Vector4 ZWWX {
			get => new Vector4( Z, W, W, X );
		}

		public Vector4 ZWWY {
			get => new Vector4( Z, W, W, Y );
		}

		public Vector4 ZWWZ {
			get => new Vector4( Z, W, W, Z );
		}

		public Vector4 ZWWW {
			get => new Vector4( Z, W, W, W );
		}

		public Vector4 WXXX {
			get => new Vector4( W, X, X, X );
		}

		public Vector4 WXXY {
			get => new Vector4( W, X, X, Y );
		}

		public Vector4 WXXZ {
			get => new Vector4( W, X, X, Z );
		}

		public Vector4 WXXW {
			get => new Vector4( W, X, X, W );
		}

		public Vector4 WXYX {
			get => new Vector4( W, X, Y, X );
		}

		public Vector4 WXYY {
			get => new Vector4( W, X, Y, Y );
		}

		public Vector4 WXYZ {
			get => new Vector4( W, X, Y, Z ); set {
				W = value.X;
				X = value.Y;
				Y = value.Z;
				Z = value.W;
			}
		}

		public Vector4 WXYW {
			get => new Vector4( W, X, Y, W );
		}

		public Vector4 WXZX {
			get => new Vector4( W, X, Z, X );
		}

		public Vector4 WXZY {
			get => new Vector4( W, X, Z, Y ); set {
				W = value.X;
				X = value.Y;
				Z = value.Z;
				Y = value.W;
			}
		}

		public Vector4 WXZZ {
			get => new Vector4( W, X, Z, Z );
		}

		public Vector4 WXZW {
			get => new Vector4( W, X, Z, W );
		}

		public Vector4 WXWX {
			get => new Vector4( W, X, W, X );
		}

		public Vector4 WXWY {
			get => new Vector4( W, X, W, Y );
		}

		public Vector4 WXWZ {
			get => new Vector4( W, X, W, Z );
		}

		public Vector4 WXWW {
			get => new Vector4( W, X, W, W );
		}

		public Vector4 WYXX {
			get => new Vector4( W, Y, X, X );
		}

		public Vector4 WYXY {
			get => new Vector4( W, Y, X, Y );
		}

		public Vector4 WYXZ {
			get => new Vector4( W, Y, X, Z ); set {
				W = value.X;
				Y = value.Y;
				X = value.Z;
				Z = value.W;
			}
		}

		public Vector4 WYXW {
			get => new Vector4( W, Y, X, W );
		}

		public Vector4 WYYX {
			get => new Vector4( W, Y, Y, X );
		}

		public Vector4 WYYY {
			get => new Vector4( W, Y, Y, Y );
		}

		public Vector4 WYYZ {
			get => new Vector4( W, Y, Y, Z );
		}

		public Vector4 WYYW {
			get => new Vector4( W, Y, Y, W );
		}

		public Vector4 WYZX {
			get => new Vector4( W, Y, Z, X ); set {
				W = value.X;
				Y = value.Y;
				Z = value.Z;
				X = value.W;
			}
		}

		public Vector4 WYZY {
			get => new Vector4( W, Y, Z, Y );
		}

		public Vector4 WYZZ {
			get => new Vector4( W, Y, Z, Z );
		}

		public Vector4 WYZW {
			get => new Vector4( W, Y, Z, W );
		}

		public Vector4 WYWX {
			get => new Vector4( W, Y, W, X );
		}

		public Vector4 WYWY {
			get => new Vector4( W, Y, W, Y );
		}

		public Vector4 WYWZ {
			get => new Vector4( W, Y, W, Z );
		}

		public Vector4 WYWW {
			get => new Vector4( W, Y, W, W );
		}

		public Vector4 WZXX {
			get => new Vector4( W, Z, X, X );
		}

		public Vector4 WZXY {
			get => new Vector4( W, Z, X, Y ); set {
				W = value.X;
				Z = value.Y;
				X = value.Z;
				Y = value.W;
			}
		}

		public Vector4 WZXZ {
			get => new Vector4( W, Z, X, Z );
		}

		public Vector4 WZXW {
			get => new Vector4( W, Z, X, W );
		}

		public Vector4 WZYX {
			get => new Vector4( W, Z, Y, X ); set {
				W = value.X;
				Z = value.Y;
				Y = value.Z;
				X = value.W;
			}
		}

		public Vector4 WZYY {
			get => new Vector4( W, Z, Y, Y );
		}

		public Vector4 WZYZ {
			get => new Vector4( W, Z, Y, Z );
		}

		public Vector4 WZYW {
			get => new Vector4( W, Z, Y, W );
		}

		public Vector4 WZZX {
			get => new Vector4( W, Z, Z, X );
		}

		public Vector4 WZZY {
			get => new Vector4( W, Z, Z, Y );
		}

		public Vector4 WZZZ {
			get => new Vector4( W, Z, Z, Z );
		}

		public Vector4 WZZW {
			get => new Vector4( W, Z, Z, W );
		}

		public Vector4 WZWX {
			get => new Vector4( W, Z, W, X );
		}

		public Vector4 WZWY {
			get => new Vector4( W, Z, W, Y );
		}

		public Vector4 WZWZ {
			get => new Vector4( W, Z, W, Z );
		}

		public Vector4 WZWW {
			get => new Vector4( W, Z, W, W );
		}

		public Vector4 WWXX {
			get => new Vector4( W, W, X, X );
		}

		public Vector4 WWXY {
			get => new Vector4( W, W, X, Y );
		}

		public Vector4 WWXZ {
			get => new Vector4( W, W, X, Z );
		}

		public Vector4 WWXW {
			get => new Vector4( W, W, X, W );
		}

		public Vector4 WWYX {
			get => new Vector4( W, W, Y, X );
		}

		public Vector4 WWYY {
			get => new Vector4( W, W, Y, Y );
		}

		public Vector4 WWYZ {
			get => new Vector4( W, W, Y, Z );
		}

		public Vector4 WWYW {
			get => new Vector4( W, W, Y, W );
		}

		public Vector4 WWZX {
			get => new Vector4( W, W, Z, X );
		}

		public Vector4 WWZY {
			get => new Vector4( W, W, Z, Y );
		}

		public Vector4 WWZZ {
			get => new Vector4( W, W, Z, Z );
		}

		public Vector4 WWZW {
			get => new Vector4( W, W, Z, W );
		}

		public Vector4 WWWX {
			get => new Vector4( W, W, W, X );
		}

		public Vector4 WWWY {
			get => new Vector4( W, W, W, Y );
		}

		public Vector4 WWWZ {
			get => new Vector4( W, W, W, Z );
		}

		public Vector4 WWWW {
			get => new Vector4( W, W, W, W );
		}
		#endregion

		#region Operators
		public static Vector4 operator +( Vector4 a, Vector4 b ) {
			return new Vector4( a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W );
		}

		public static Vector4 operator -( Vector4 a ) {
			return new Vector4( -a.X, -a.Y, -a.Z, -a.W );
		}

		public static Vector4 operator *( Vector4 a, float scalar ) {
			return new Vector4( a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar );
		}

		public static Vector4 operator *( float scalar, Vector4 a ) {
			return new Vector4( a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar );
		}

		public static Vector4 operator *( Vector4 a, Vector4 b ) {
			return new Vector4( a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W );
		}

		public static Vector4 operator -( Vector4 a, Vector4 b ) {
			return a + -b;
		}

		public static Vector4 operator /( Vector4 a, float divident ) {
			return a * ( 1f / divident );
		}

		public static Vector4 operator /( Vector4 a, Vector4 b ) {
			return new Vector4( a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W );
		}

		public static bool operator ==( Vector4 a, Vector4 b ) {
			return a.Equals( b );
		}

		public static bool operator !=( Vector4 a, Vector4 b ) {
			return !a.Equals( b );
		}

		#region Implicit
		public static implicit operator Vector4( float[] a ) {
			if( a.Length != 4 )
				throw new System.ArgumentException( "Length of array is not compatible with Vector4" );
			return new Vector4( a[ 0 ], a[ 1 ], a[ 2 ], a[ 3 ] );
		}

		public static implicit operator Vector4( (float, float, float, float) a ) {
			return new Vector4( a.Item1, a.Item2, a.Item3, a.Item4 );
		}

		public static implicit operator Vector4( float a ) {
			return new Vector4( a );
		}

		public static implicit operator float[] ( Vector4 a ) {
			return new float[] { a.X, a.Y, a.Z, a.W };
		}
		#endregion
		#endregion
	}
}
