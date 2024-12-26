using System;

namespace Engine.LMath {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
	public struct Vector4i : System.IEquatable<Vector4i>, ILAMeasurable {

		public int X, Y, Z, W;

		public Vector4 AsFloat => new Vector4( X, Y, Z, W );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y + Z * Z + W * W );
		public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

		public Vector4i( int x, int y, int z, int w ) {
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4i( Vector2i a, int z, int w ) : this( a.X, a.Y, z, w ) {
		}
		public Vector4i( Vector2i a, Vector2i b ) : this( a.X, a.Y, b.X, b.Y ) { }

		public Vector4i( Vector3i a, int w ) : this( a.X, a.Y, a.Z, w ) { }

		public Vector4i( Vector4i a ) : this( a.X, a.Y, a.Z, a.W ) { }

		public Vector4i( int s ) : this( s, s, s, s ) { }

		public static readonly Vector4i Zero = new Vector4i( 0 );

		#region Override and Equals

		public override bool Equals( object other ) {
			if( other is Vector4i == false )
				return false;
			return Equals( (Vector4i) other );
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
		}

		public bool Equals( Vector4i other ) {
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

		public override string ToString() {
			return $"[{X},{Y},{Z},{W}]";
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

		public Vector2i XW {
			get => new Vector2i( X, W ); set {
				X = value.X;
				W = value.Y;
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

		public Vector2i YW {
			get => new Vector2i( Y, W ); set {
				Y = value.X;
				W = value.Y;
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

		public Vector2i ZW {
			get => new Vector2i( Z, W ); set {
				Z = value.X;
				W = value.Y;
			}
		}

		public Vector2i WX {
			get => new Vector2i( W, X ); set {
				W = value.X;
				X = value.Y;
			}
		}

		public Vector2i WY {
			get => new Vector2i( W, Y ); set {
				W = value.X;
				Y = value.Y;
			}
		}

		public Vector2i WZ {
			get => new Vector2i( W, Z ); set {
				W = value.X;
				Z = value.Y;
			}
		}

		public Vector2i WW {
			get => new Vector2i( W, W );
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

		public Vector3i XXW {
			get => new Vector3i( X, X, W );
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

		public Vector3i XYW {
			get => new Vector3i( X, Y, W ); set {
				X = value.X;
				Y = value.Y;
				W = value.Z;
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

		public Vector3i XZW {
			get => new Vector3i( X, Z, W ); set {
				X = value.X;
				Z = value.Y;
				W = value.Z;
			}
		}

		public Vector3i XWX {
			get => new Vector3i( X, W, X );
		}

		public Vector3i XWY {
			get => new Vector3i( X, W, Y ); set {
				X = value.X;
				W = value.Y;
				Y = value.Z;
			}
		}

		public Vector3i XWZ {
			get => new Vector3i( X, W, Z ); set {
				X = value.X;
				W = value.Y;
				Z = value.Z;
			}
		}

		public Vector3i XWW {
			get => new Vector3i( X, W, W );
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

		public Vector3i YXW {
			get => new Vector3i( Y, X, W ); set {
				Y = value.X;
				X = value.Y;
				W = value.Z;
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

		public Vector3i YYW {
			get => new Vector3i( Y, Y, W );
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

		public Vector3i YZW {
			get => new Vector3i( Y, Z, W ); set {
				Y = value.X;
				Z = value.Y;
				W = value.Z;
			}
		}

		public Vector3i YWX {
			get => new Vector3i( Y, W, X ); set {
				Y = value.X;
				W = value.Y;
				X = value.Z;
			}
		}

		public Vector3i YWY {
			get => new Vector3i( Y, W, Y );
		}

		public Vector3i YWZ {
			get => new Vector3i( Y, W, Z ); set {
				Y = value.X;
				W = value.Y;
				Z = value.Z;
			}
		}

		public Vector3i YWW {
			get => new Vector3i( Y, W, W );
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

		public Vector3i ZXW {
			get => new Vector3i( Z, X, W ); set {
				Z = value.X;
				X = value.Y;
				W = value.Z;
			}
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

		public Vector3i ZYW {
			get => new Vector3i( Z, Y, W ); set {
				Z = value.X;
				Y = value.Y;
				W = value.Z;
			}
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

		public Vector3i ZZW {
			get => new Vector3i( Z, Z, W );
		}

		public Vector3i ZWX {
			get => new Vector3i( Z, W, X ); set {
				Z = value.X;
				W = value.Y;
				X = value.Z;
			}
		}

		public Vector3i ZWY {
			get => new Vector3i( Z, W, Y ); set {
				Z = value.X;
				W = value.Y;
				Y = value.Z;
			}
		}

		public Vector3i ZWZ {
			get => new Vector3i( Z, W, Z );
		}

		public Vector3i ZWW {
			get => new Vector3i( Z, W, W );
		}

		public Vector3i WXX {
			get => new Vector3i( W, X, X );
		}

		public Vector3i WXY {
			get => new Vector3i( W, X, Y ); set {
				W = value.X;
				X = value.Y;
				Y = value.Z;
			}
		}

		public Vector3i WXZ {
			get => new Vector3i( W, X, Z ); set {
				W = value.X;
				X = value.Y;
				Z = value.Z;
			}
		}

		public Vector3i WXW {
			get => new Vector3i( W, X, W );
		}

		public Vector3i WYX {
			get => new Vector3i( W, Y, X ); set {
				W = value.X;
				Y = value.Y;
				X = value.Z;
			}
		}

		public Vector3i WYY {
			get => new Vector3i( W, Y, Y );
		}

		public Vector3i WYZ {
			get => new Vector3i( W, Y, Z ); set {
				W = value.X;
				Y = value.Y;
				Z = value.Z;
			}
		}

		public Vector3i WYW {
			get => new Vector3i( W, Y, W );
		}

		public Vector3i WZX {
			get => new Vector3i( W, Z, X ); set {
				W = value.X;
				Z = value.Y;
				X = value.Z;
			}
		}

		public Vector3i WZY {
			get => new Vector3i( W, Z, Y ); set {
				W = value.X;
				Z = value.Y;
				Y = value.Z;
			}
		}

		public Vector3i WZZ {
			get => new Vector3i( W, Z, Z );
		}

		public Vector3i WZW {
			get => new Vector3i( W, Z, W );
		}

		public Vector3i WWX {
			get => new Vector3i( W, W, X );
		}

		public Vector3i WWY {
			get => new Vector3i( W, W, Y );
		}

		public Vector3i WWZ {
			get => new Vector3i( W, W, Z );
		}

		public Vector3i WWW {
			get => new Vector3i( W, W, W );
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

		public Vector4i XXXW {
			get => new Vector4i( X, X, X, W );
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

		public Vector4i XXYW {
			get => new Vector4i( X, X, Y, W );
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

		public Vector4i XXZW {
			get => new Vector4i( X, X, Z, W );
		}

		public Vector4i XXWX {
			get => new Vector4i( X, X, W, X );
		}

		public Vector4i XXWY {
			get => new Vector4i( X, X, W, Y );
		}

		public Vector4i XXWZ {
			get => new Vector4i( X, X, W, Z );
		}

		public Vector4i XXWW {
			get => new Vector4i( X, X, W, W );
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

		public Vector4i XYXW {
			get => new Vector4i( X, Y, X, W );
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

		public Vector4i XYYW {
			get => new Vector4i( X, Y, Y, W );
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

		public Vector4i XYZW {
			get => new Vector4i( X, Y, Z, W ); set {
				X = value.X;
				Y = value.Y;
				Z = value.Z;
				W = value.W;
			}
		}

		public Vector4i XYWX {
			get => new Vector4i( X, Y, W, X );
		}

		public Vector4i XYWY {
			get => new Vector4i( X, Y, W, Y );
		}

		public Vector4i XYWZ {
			get => new Vector4i( X, Y, W, Z ); set {
				X = value.X;
				Y = value.Y;
				W = value.Z;
				Z = value.W;
			}
		}

		public Vector4i XYWW {
			get => new Vector4i( X, Y, W, W );
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

		public Vector4i XZXW {
			get => new Vector4i( X, Z, X, W );
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

		public Vector4i XZYW {
			get => new Vector4i( X, Z, Y, W ); set {
				X = value.X;
				Z = value.Y;
				Y = value.Z;
				W = value.W;
			}
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

		public Vector4i XZZW {
			get => new Vector4i( X, Z, Z, W );
		}

		public Vector4i XZWX {
			get => new Vector4i( X, Z, W, X );
		}

		public Vector4i XZWY {
			get => new Vector4i( X, Z, W, Y ); set {
				X = value.X;
				Z = value.Y;
				W = value.Z;
				Y = value.W;
			}
		}

		public Vector4i XZWZ {
			get => new Vector4i( X, Z, W, Z );
		}

		public Vector4i XZWW {
			get => new Vector4i( X, Z, W, W );
		}

		public Vector4i XWXX {
			get => new Vector4i( X, W, X, X );
		}

		public Vector4i XWXY {
			get => new Vector4i( X, W, X, Y );
		}

		public Vector4i XWXZ {
			get => new Vector4i( X, W, X, Z );
		}

		public Vector4i XWXW {
			get => new Vector4i( X, W, X, W );
		}

		public Vector4i XWYX {
			get => new Vector4i( X, W, Y, X );
		}

		public Vector4i XWYY {
			get => new Vector4i( X, W, Y, Y );
		}

		public Vector4i XWYZ {
			get => new Vector4i( X, W, Y, Z ); set {
				X = value.X;
				W = value.Y;
				Y = value.Z;
				Z = value.W;
			}
		}

		public Vector4i XWYW {
			get => new Vector4i( X, W, Y, W );
		}

		public Vector4i XWZX {
			get => new Vector4i( X, W, Z, X );
		}

		public Vector4i XWZY {
			get => new Vector4i( X, W, Z, Y ); set {
				X = value.X;
				W = value.Y;
				Z = value.Z;
				Y = value.W;
			}
		}

		public Vector4i XWZZ {
			get => new Vector4i( X, W, Z, Z );
		}

		public Vector4i XWZW {
			get => new Vector4i( X, W, Z, W );
		}

		public Vector4i XWWX {
			get => new Vector4i( X, W, W, X );
		}

		public Vector4i XWWY {
			get => new Vector4i( X, W, W, Y );
		}

		public Vector4i XWWZ {
			get => new Vector4i( X, W, W, Z );
		}

		public Vector4i XWWW {
			get => new Vector4i( X, W, W, W );
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

		public Vector4i YXXW {
			get => new Vector4i( Y, X, X, W );
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

		public Vector4i YXYW {
			get => new Vector4i( Y, X, Y, W );
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

		public Vector4i YXZW {
			get => new Vector4i( Y, X, Z, W ); set {
				Y = value.X;
				X = value.Y;
				Z = value.Z;
				W = value.W;
			}
		}

		public Vector4i YXWX {
			get => new Vector4i( Y, X, W, X );
		}

		public Vector4i YXWY {
			get => new Vector4i( Y, X, W, Y );
		}

		public Vector4i YXWZ {
			get => new Vector4i( Y, X, W, Z ); set {
				Y = value.X;
				X = value.Y;
				W = value.Z;
				Z = value.W;
			}
		}

		public Vector4i YXWW {
			get => new Vector4i( Y, X, W, W );
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

		public Vector4i YYXW {
			get => new Vector4i( Y, Y, X, W );
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

		public Vector4i YYYW {
			get => new Vector4i( Y, Y, Y, W );
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

		public Vector4i YYZW {
			get => new Vector4i( Y, Y, Z, W );
		}

		public Vector4i YYWX {
			get => new Vector4i( Y, Y, W, X );
		}

		public Vector4i YYWY {
			get => new Vector4i( Y, Y, W, Y );
		}

		public Vector4i YYWZ {
			get => new Vector4i( Y, Y, W, Z );
		}

		public Vector4i YYWW {
			get => new Vector4i( Y, Y, W, W );
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

		public Vector4i YZXW {
			get => new Vector4i( Y, Z, X, W ); set {
				Y = value.X;
				Z = value.Y;
				X = value.Z;
				W = value.W;
			}
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

		public Vector4i YZYW {
			get => new Vector4i( Y, Z, Y, W );
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

		public Vector4i YZZW {
			get => new Vector4i( Y, Z, Z, W );
		}

		public Vector4i YZWX {
			get => new Vector4i( Y, Z, W, X ); set {
				Y = value.X;
				Z = value.Y;
				W = value.Z;
				X = value.W;
			}
		}

		public Vector4i YZWY {
			get => new Vector4i( Y, Z, W, Y );
		}

		public Vector4i YZWZ {
			get => new Vector4i( Y, Z, W, Z );
		}

		public Vector4i YZWW {
			get => new Vector4i( Y, Z, W, W );
		}

		public Vector4i YWXX {
			get => new Vector4i( Y, W, X, X );
		}

		public Vector4i YWXY {
			get => new Vector4i( Y, W, X, Y );
		}

		public Vector4i YWXZ {
			get => new Vector4i( Y, W, X, Z ); set {
				Y = value.X;
				W = value.Y;
				X = value.Z;
				Z = value.W;
			}
		}

		public Vector4i YWXW {
			get => new Vector4i( Y, W, X, W );
		}

		public Vector4i YWYX {
			get => new Vector4i( Y, W, Y, X );
		}

		public Vector4i YWYY {
			get => new Vector4i( Y, W, Y, Y );
		}

		public Vector4i YWYZ {
			get => new Vector4i( Y, W, Y, Z );
		}

		public Vector4i YWYW {
			get => new Vector4i( Y, W, Y, W );
		}

		public Vector4i YWZX {
			get => new Vector4i( Y, W, Z, X ); set {
				Y = value.X;
				W = value.Y;
				Z = value.Z;
				X = value.W;
			}
		}

		public Vector4i YWZY {
			get => new Vector4i( Y, W, Z, Y );
		}

		public Vector4i YWZZ {
			get => new Vector4i( Y, W, Z, Z );
		}

		public Vector4i YWZW {
			get => new Vector4i( Y, W, Z, W );
		}

		public Vector4i YWWX {
			get => new Vector4i( Y, W, W, X );
		}

		public Vector4i YWWY {
			get => new Vector4i( Y, W, W, Y );
		}

		public Vector4i YWWZ {
			get => new Vector4i( Y, W, W, Z );
		}

		public Vector4i YWWW {
			get => new Vector4i( Y, W, W, W );
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

		public Vector4i ZXXW {
			get => new Vector4i( Z, X, X, W );
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

		public Vector4i ZXYW {
			get => new Vector4i( Z, X, Y, W ); set {
				Z = value.X;
				X = value.Y;
				Y = value.Z;
				W = value.W;
			}
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

		public Vector4i ZXZW {
			get => new Vector4i( Z, X, Z, W );
		}

		public Vector4i ZXWX {
			get => new Vector4i( Z, X, W, X );
		}

		public Vector4i ZXWY {
			get => new Vector4i( Z, X, W, Y ); set {
				Z = value.X;
				X = value.Y;
				W = value.Z;
				Y = value.W;
			}
		}

		public Vector4i ZXWZ {
			get => new Vector4i( Z, X, W, Z );
		}

		public Vector4i ZXWW {
			get => new Vector4i( Z, X, W, W );
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

		public Vector4i ZYXW {
			get => new Vector4i( Z, Y, X, W ); set {
				Z = value.X;
				Y = value.Y;
				X = value.Z;
				W = value.W;
			}
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

		public Vector4i ZYYW {
			get => new Vector4i( Z, Y, Y, W );
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

		public Vector4i ZYZW {
			get => new Vector4i( Z, Y, Z, W );
		}

		public Vector4i ZYWX {
			get => new Vector4i( Z, Y, W, X ); set {
				Z = value.X;
				Y = value.Y;
				W = value.Z;
				X = value.W;
			}
		}

		public Vector4i ZYWY {
			get => new Vector4i( Z, Y, W, Y );
		}

		public Vector4i ZYWZ {
			get => new Vector4i( Z, Y, W, Z );
		}

		public Vector4i ZYWW {
			get => new Vector4i( Z, Y, W, W );
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

		public Vector4i ZZXW {
			get => new Vector4i( Z, Z, X, W );
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

		public Vector4i ZZYW {
			get => new Vector4i( Z, Z, Y, W );
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

		public Vector4i ZZZW {
			get => new Vector4i( Z, Z, Z, W );
		}

		public Vector4i ZZWX {
			get => new Vector4i( Z, Z, W, X );
		}

		public Vector4i ZZWY {
			get => new Vector4i( Z, Z, W, Y );
		}

		public Vector4i ZZWZ {
			get => new Vector4i( Z, Z, W, Z );
		}

		public Vector4i ZZWW {
			get => new Vector4i( Z, Z, W, W );
		}

		public Vector4i ZWXX {
			get => new Vector4i( Z, W, X, X );
		}

		public Vector4i ZWXY {
			get => new Vector4i( Z, W, X, Y ); set {
				Z = value.X;
				W = value.Y;
				X = value.Z;
				Y = value.W;
			}
		}

		public Vector4i ZWXZ {
			get => new Vector4i( Z, W, X, Z );
		}

		public Vector4i ZWXW {
			get => new Vector4i( Z, W, X, W );
		}

		public Vector4i ZWYX {
			get => new Vector4i( Z, W, Y, X ); set {
				Z = value.X;
				W = value.Y;
				Y = value.Z;
				X = value.W;
			}
		}

		public Vector4i ZWYY {
			get => new Vector4i( Z, W, Y, Y );
		}

		public Vector4i ZWYZ {
			get => new Vector4i( Z, W, Y, Z );
		}

		public Vector4i ZWYW {
			get => new Vector4i( Z, W, Y, W );
		}

		public Vector4i ZWZX {
			get => new Vector4i( Z, W, Z, X );
		}

		public Vector4i ZWZY {
			get => new Vector4i( Z, W, Z, Y );
		}

		public Vector4i ZWZZ {
			get => new Vector4i( Z, W, Z, Z );
		}

		public Vector4i ZWZW {
			get => new Vector4i( Z, W, Z, W );
		}

		public Vector4i ZWWX {
			get => new Vector4i( Z, W, W, X );
		}

		public Vector4i ZWWY {
			get => new Vector4i( Z, W, W, Y );
		}

		public Vector4i ZWWZ {
			get => new Vector4i( Z, W, W, Z );
		}

		public Vector4i ZWWW {
			get => new Vector4i( Z, W, W, W );
		}

		public Vector4i WXXX {
			get => new Vector4i( W, X, X, X );
		}

		public Vector4i WXXY {
			get => new Vector4i( W, X, X, Y );
		}

		public Vector4i WXXZ {
			get => new Vector4i( W, X, X, Z );
		}

		public Vector4i WXXW {
			get => new Vector4i( W, X, X, W );
		}

		public Vector4i WXYX {
			get => new Vector4i( W, X, Y, X );
		}

		public Vector4i WXYY {
			get => new Vector4i( W, X, Y, Y );
		}

		public Vector4i WXYZ {
			get => new Vector4i( W, X, Y, Z ); set {
				W = value.X;
				X = value.Y;
				Y = value.Z;
				Z = value.W;
			}
		}

		public Vector4i WXYW {
			get => new Vector4i( W, X, Y, W );
		}

		public Vector4i WXZX {
			get => new Vector4i( W, X, Z, X );
		}

		public Vector4i WXZY {
			get => new Vector4i( W, X, Z, Y ); set {
				W = value.X;
				X = value.Y;
				Z = value.Z;
				Y = value.W;
			}
		}

		public Vector4i WXZZ {
			get => new Vector4i( W, X, Z, Z );
		}

		public Vector4i WXZW {
			get => new Vector4i( W, X, Z, W );
		}

		public Vector4i WXWX {
			get => new Vector4i( W, X, W, X );
		}

		public Vector4i WXWY {
			get => new Vector4i( W, X, W, Y );
		}

		public Vector4i WXWZ {
			get => new Vector4i( W, X, W, Z );
		}

		public Vector4i WXWW {
			get => new Vector4i( W, X, W, W );
		}

		public Vector4i WYXX {
			get => new Vector4i( W, Y, X, X );
		}

		public Vector4i WYXY {
			get => new Vector4i( W, Y, X, Y );
		}

		public Vector4i WYXZ {
			get => new Vector4i( W, Y, X, Z ); set {
				W = value.X;
				Y = value.Y;
				X = value.Z;
				Z = value.W;
			}
		}

		public Vector4i WYXW {
			get => new Vector4i( W, Y, X, W );
		}

		public Vector4i WYYX {
			get => new Vector4i( W, Y, Y, X );
		}

		public Vector4i WYYY {
			get => new Vector4i( W, Y, Y, Y );
		}

		public Vector4i WYYZ {
			get => new Vector4i( W, Y, Y, Z );
		}

		public Vector4i WYYW {
			get => new Vector4i( W, Y, Y, W );
		}

		public Vector4i WYZX {
			get => new Vector4i( W, Y, Z, X ); set {
				W = value.X;
				Y = value.Y;
				Z = value.Z;
				X = value.W;
			}
		}

		public Vector4i WYZY {
			get => new Vector4i( W, Y, Z, Y );
		}

		public Vector4i WYZZ {
			get => new Vector4i( W, Y, Z, Z );
		}

		public Vector4i WYZW {
			get => new Vector4i( W, Y, Z, W );
		}

		public Vector4i WYWX {
			get => new Vector4i( W, Y, W, X );
		}

		public Vector4i WYWY {
			get => new Vector4i( W, Y, W, Y );
		}

		public Vector4i WYWZ {
			get => new Vector4i( W, Y, W, Z );
		}

		public Vector4i WYWW {
			get => new Vector4i( W, Y, W, W );
		}

		public Vector4i WZXX {
			get => new Vector4i( W, Z, X, X );
		}

		public Vector4i WZXY {
			get => new Vector4i( W, Z, X, Y ); set {
				W = value.X;
				Z = value.Y;
				X = value.Z;
				Y = value.W;
			}
		}

		public Vector4i WZXZ {
			get => new Vector4i( W, Z, X, Z );
		}

		public Vector4i WZXW {
			get => new Vector4i( W, Z, X, W );
		}

		public Vector4i WZYX {
			get => new Vector4i( W, Z, Y, X ); set {
				W = value.X;
				Z = value.Y;
				Y = value.Z;
				X = value.W;
			}
		}

		public Vector4i WZYY {
			get => new Vector4i( W, Z, Y, Y );
		}

		public Vector4i WZYZ {
			get => new Vector4i( W, Z, Y, Z );
		}

		public Vector4i WZYW {
			get => new Vector4i( W, Z, Y, W );
		}

		public Vector4i WZZX {
			get => new Vector4i( W, Z, Z, X );
		}

		public Vector4i WZZY {
			get => new Vector4i( W, Z, Z, Y );
		}

		public Vector4i WZZZ {
			get => new Vector4i( W, Z, Z, Z );
		}

		public Vector4i WZZW {
			get => new Vector4i( W, Z, Z, W );
		}

		public Vector4i WZWX {
			get => new Vector4i( W, Z, W, X );
		}

		public Vector4i WZWY {
			get => new Vector4i( W, Z, W, Y );
		}

		public Vector4i WZWZ {
			get => new Vector4i( W, Z, W, Z );
		}

		public Vector4i WZWW {
			get => new Vector4i( W, Z, W, W );
		}

		public Vector4i WWXX {
			get => new Vector4i( W, W, X, X );
		}

		public Vector4i WWXY {
			get => new Vector4i( W, W, X, Y );
		}

		public Vector4i WWXZ {
			get => new Vector4i( W, W, X, Z );
		}

		public Vector4i WWXW {
			get => new Vector4i( W, W, X, W );
		}

		public Vector4i WWYX {
			get => new Vector4i( W, W, Y, X );
		}

		public Vector4i WWYY {
			get => new Vector4i( W, W, Y, Y );
		}

		public Vector4i WWYZ {
			get => new Vector4i( W, W, Y, Z );
		}

		public Vector4i WWYW {
			get => new Vector4i( W, W, Y, W );
		}

		public Vector4i WWZX {
			get => new Vector4i( W, W, Z, X );
		}

		public Vector4i WWZY {
			get => new Vector4i( W, W, Z, Y );
		}

		public Vector4i WWZZ {
			get => new Vector4i( W, W, Z, Z );
		}

		public Vector4i WWZW {
			get => new Vector4i( W, W, Z, W );
		}

		public Vector4i WWWX {
			get => new Vector4i( W, W, W, X );
		}

		public Vector4i WWWY {
			get => new Vector4i( W, W, W, Y );
		}

		public Vector4i WWWZ {
			get => new Vector4i( W, W, W, Z );
		}

		public Vector4i WWWW {
			get => new Vector4i( W, W, W, W );
		}
		#endregion

		#region Operators
		public static Vector4i operator +( Vector4i a, Vector4i b ) {
			return new Vector4i( a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W );
		}

		public static Vector4i operator -( Vector4i a ) {
			return new Vector4i( -a.X, -a.Y, -a.Z, -a.W );
		}

		public static Vector4i operator *( Vector4i a, int scalar ) {
			return new Vector4i( a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar );
		}

		public static Vector4i operator *( int scalar, Vector4i a ) {
			return new Vector4i( a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar );
		}

		public static Vector4 operator *( Vector4i a, float scalar ) {
			return new Vector4( a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar );
		}

		public static Vector4 operator *( float scalar, Vector4i a ) {
			return new Vector4( a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar );
		}

		public static Vector4i operator *( Vector4i a, Vector4i b ) {
			return new Vector4i( a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W );
		}

		public static Vector4 operator *( Vector4i a, Vector4 b ) {
			return new Vector4( a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W );
		}

		public static Vector4 operator *( Vector4 a, Vector4i b ) {
			return new Vector4( a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W );
		}

		public static Vector4i operator -( Vector4i a, Vector4i b ) {
			return a + -b;
		}

		public static Vector4i operator /( Vector4i a, int divident ) {
			return ( a * ( 1f / divident ) ).IntCasted;
		}

		public static Vector4 operator /( Vector4i a, float divident ) {
			return a * ( 1f / divident );
		}

		public static bool operator ==( Vector4i a, Vector4i b ) {
			return a.Equals( b );
		}

		public static bool operator !=( Vector4i a, Vector4i b ) {
			return !a.Equals( b );
		}

		#region Implicit
		public static implicit operator Vector4i( int[] a ) {
			if( a.Length != 4 )
				throw new System.ArgumentException( "Length of array is not compatible with Vector4i" );
			return new Vector4i( a[ 0 ], a[ 1 ], a[ 2 ], a[ 3 ] );
		}

		public static implicit operator Vector4i( int a ) {
			return new Vector4i( a );
		}

		public static implicit operator int[]( Vector4i a ) {
			return new int[] { a.X, a.Y, a.Z, a.W };
		}
		#endregion
		#endregion

	}
}
