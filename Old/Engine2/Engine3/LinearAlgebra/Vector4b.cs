using Engine.Utilities.Data;

namespace Engine.LinearAlgebra {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Explicit )]
	public struct Vector4b : System.IEquatable<Vector4b> {

		[System.Runtime.InteropServices.FieldOffset( 0 )]
		public byte X;
		[System.Runtime.InteropServices.FieldOffset( 1 )]
		public byte Y;
		[System.Runtime.InteropServices.FieldOffset( 2 )]
		public byte Z;
		[System.Runtime.InteropServices.FieldOffset( 3 )]
		public byte W;

		public Vector4 AsFloat => new Vector4( X, Y, Z, W );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y + Z * Z + W * W );
		public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

		public Vector4b( byte x, byte y, byte z, byte w ) {
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4b( Vector2b a, byte z, byte w ) : this( a.X, a.Y, z, w ) {
		}
		public Vector4b( Vector2b a, Vector2b b ) : this( a.X, a.Y, b.X, b.Y ) { }

		public Vector4b( Vector3b a, byte w ) : this( a.X, a.Y, a.Z, w ) { }

		public Vector4b( Vector4b a ) : this( a.X, a.Y, a.Z, a.W ) { }

		public Vector4b( byte s ) : this( s, s, s, s ) { }

		public static readonly Vector4b Zero = new Vector4b( 0 );
		public static readonly Vector4b R = new Vector4b( 255, 0, 0, 0 );
		public static readonly Vector4b G = new Vector4b( 0, 255, 0, 0 );
		public static readonly Vector4b B = new Vector4b( 0, 0, 255, 0 );
		public static readonly Vector4b A = new Vector4b( 0, 0, 0, 255 );
		/**<summary>Each component set to 255.</summary>*/
		public static readonly Vector4b Byte = new Vector4b( 255 );

		#region Override and Equals

		public override bool Equals( object other ) {
			if( other is Vector4b == false )
				return false;
			return Equals( (Vector4b) other );
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
		}

		public bool Equals( Vector4b other ) {
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

		public override string ToString() {
			return $"[{X},{Y},{Z},{W}]";
		}

		#endregion

		#region Swizzle
		public Vector2b XX {
			get => new Vector2b( X, X );
		}

		public Vector2b XY {
			get => new Vector2b( X, Y ); set {
				X = value.X;
				Y = value.Y;
			}
		}

		public Vector2b XZ {
			get => new Vector2b( X, Z ); set {
				X = value.X;
				Z = value.Y;
			}
		}

		public Vector2b XW {
			get => new Vector2b( X, W ); set {
				X = value.X;
				W = value.Y;
			}
		}

		public Vector2b YX {
			get => new Vector2b( Y, X ); set {
				Y = value.X;
				X = value.Y;
			}
		}

		public Vector2b YY {
			get => new Vector2b( Y, Y );
		}

		public Vector2b YZ {
			get => new Vector2b( Y, Z ); set {
				Y = value.X;
				Z = value.Y;
			}
		}

		public Vector2b YW {
			get => new Vector2b( Y, W ); set {
				Y = value.X;
				W = value.Y;
			}
		}

		public Vector2b ZX {
			get => new Vector2b( Z, X ); set {
				Z = value.X;
				X = value.Y;
			}
		}

		public Vector2b ZY {
			get => new Vector2b( Z, Y ); set {
				Z = value.X;
				Y = value.Y;
			}
		}

		public Vector2b ZZ {
			get => new Vector2b( Z, Z );
		}

		public Vector2b ZW {
			get => new Vector2b( Z, W ); set {
				Z = value.X;
				W = value.Y;
			}
		}

		public Vector2b WX {
			get => new Vector2b( W, X ); set {
				W = value.X;
				X = value.Y;
			}
		}

		public Vector2b WY {
			get => new Vector2b( W, Y ); set {
				W = value.X;
				Y = value.Y;
			}
		}

		public Vector2b WZ {
			get => new Vector2b( W, Z ); set {
				W = value.X;
				Z = value.Y;
			}
		}

		public Vector2b WW {
			get => new Vector2b( W, W );
		}

		public Vector3b XXX {
			get => new Vector3b( X, X, X );
		}

		public Vector3b XXY {
			get => new Vector3b( X, X, Y );
		}

		public Vector3b XXZ {
			get => new Vector3b( X, X, Z );
		}

		public Vector3b XXW {
			get => new Vector3b( X, X, W );
		}

		public Vector3b XYX {
			get => new Vector3b( X, Y, X );
		}

		public Vector3b XYY {
			get => new Vector3b( X, Y, Y );
		}

		public Vector3b XYZ {
			get => new Vector3b( X, Y, Z ); set {
				X = value.X;
				Y = value.Y;
				Z = value.Z;
			}
		}

		public Vector3b XYW {
			get => new Vector3b( X, Y, W ); set {
				X = value.X;
				Y = value.Y;
				W = value.Z;
			}
		}

		public Vector3b XZX {
			get => new Vector3b( X, Z, X );
		}

		public Vector3b XZY {
			get => new Vector3b( X, Z, Y ); set {
				X = value.X;
				Z = value.Y;
				Y = value.Z;
			}
		}

		public Vector3b XZZ {
			get => new Vector3b( X, Z, Z );
		}

		public Vector3b XZW {
			get => new Vector3b( X, Z, W ); set {
				X = value.X;
				Z = value.Y;
				W = value.Z;
			}
		}

		public Vector3b XWX {
			get => new Vector3b( X, W, X );
		}

		public Vector3b XWY {
			get => new Vector3b( X, W, Y ); set {
				X = value.X;
				W = value.Y;
				Y = value.Z;
			}
		}

		public Vector3b XWZ {
			get => new Vector3b( X, W, Z ); set {
				X = value.X;
				W = value.Y;
				Z = value.Z;
			}
		}

		public Vector3b XWW {
			get => new Vector3b( X, W, W );
		}

		public Vector3b YXX {
			get => new Vector3b( Y, X, X );
		}

		public Vector3b YXY {
			get => new Vector3b( Y, X, Y );
		}

		public Vector3b YXZ {
			get => new Vector3b( Y, X, Z ); set {
				Y = value.X;
				X = value.Y;
				Z = value.Z;
			}
		}

		public Vector3b YXW {
			get => new Vector3b( Y, X, W ); set {
				Y = value.X;
				X = value.Y;
				W = value.Z;
			}
		}

		public Vector3b YYX {
			get => new Vector3b( Y, Y, X );
		}

		public Vector3b YYY {
			get => new Vector3b( Y, Y, Y );
		}

		public Vector3b YYZ {
			get => new Vector3b( Y, Y, Z );
		}

		public Vector3b YYW {
			get => new Vector3b( Y, Y, W );
		}

		public Vector3b YZX {
			get => new Vector3b( Y, Z, X ); set {
				Y = value.X;
				Z = value.Y;
				X = value.Z;
			}
		}

		public Vector3b YZY {
			get => new Vector3b( Y, Z, Y );
		}

		public Vector3b YZZ {
			get => new Vector3b( Y, Z, Z );
		}

		public Vector3b YZW {
			get => new Vector3b( Y, Z, W ); set {
				Y = value.X;
				Z = value.Y;
				W = value.Z;
			}
		}

		public Vector3b YWX {
			get => new Vector3b( Y, W, X ); set {
				Y = value.X;
				W = value.Y;
				X = value.Z;
			}
		}

		public Vector3b YWY {
			get => new Vector3b( Y, W, Y );
		}

		public Vector3b YWZ {
			get => new Vector3b( Y, W, Z ); set {
				Y = value.X;
				W = value.Y;
				Z = value.Z;
			}
		}

		public Vector3b YWW {
			get => new Vector3b( Y, W, W );
		}

		public Vector3b ZXX {
			get => new Vector3b( Z, X, X );
		}

		public Vector3b ZXY {
			get => new Vector3b( Z, X, Y ); set {
				Z = value.X;
				X = value.Y;
				Y = value.Z;
			}
		}

		public Vector3b ZXZ {
			get => new Vector3b( Z, X, Z );
		}

		public Vector3b ZXW {
			get => new Vector3b( Z, X, W ); set {
				Z = value.X;
				X = value.Y;
				W = value.Z;
			}
		}

		public Vector3b ZYX {
			get => new Vector3b( Z, Y, X ); set {
				Z = value.X;
				Y = value.Y;
				X = value.Z;
			}
		}

		public Vector3b ZYY {
			get => new Vector3b( Z, Y, Y );
		}

		public Vector3b ZYZ {
			get => new Vector3b( Z, Y, Z );
		}

		public Vector3b ZYW {
			get => new Vector3b( Z, Y, W ); set {
				Z = value.X;
				Y = value.Y;
				W = value.Z;
			}
		}

		public Vector3b ZZX {
			get => new Vector3b( Z, Z, X );
		}

		public Vector3b ZZY {
			get => new Vector3b( Z, Z, Y );
		}

		public Vector3b ZZZ {
			get => new Vector3b( Z, Z, Z );
		}

		public Vector3b ZZW {
			get => new Vector3b( Z, Z, W );
		}

		public Vector3b ZWX {
			get => new Vector3b( Z, W, X ); set {
				Z = value.X;
				W = value.Y;
				X = value.Z;
			}
		}

		public Vector3b ZWY {
			get => new Vector3b( Z, W, Y ); set {
				Z = value.X;
				W = value.Y;
				Y = value.Z;
			}
		}

		public Vector3b ZWZ {
			get => new Vector3b( Z, W, Z );
		}

		public Vector3b ZWW {
			get => new Vector3b( Z, W, W );
		}

		public Vector3b WXX {
			get => new Vector3b( W, X, X );
		}

		public Vector3b WXY {
			get => new Vector3b( W, X, Y ); set {
				W = value.X;
				X = value.Y;
				Y = value.Z;
			}
		}

		public Vector3b WXZ {
			get => new Vector3b( W, X, Z ); set {
				W = value.X;
				X = value.Y;
				Z = value.Z;
			}
		}

		public Vector3b WXW {
			get => new Vector3b( W, X, W );
		}

		public Vector3b WYX {
			get => new Vector3b( W, Y, X ); set {
				W = value.X;
				Y = value.Y;
				X = value.Z;
			}
		}

		public Vector3b WYY {
			get => new Vector3b( W, Y, Y );
		}

		public Vector3b WYZ {
			get => new Vector3b( W, Y, Z ); set {
				W = value.X;
				Y = value.Y;
				Z = value.Z;
			}
		}

		public Vector3b WYW {
			get => new Vector3b( W, Y, W );
		}

		public Vector3b WZX {
			get => new Vector3b( W, Z, X ); set {
				W = value.X;
				Z = value.Y;
				X = value.Z;
			}
		}

		public Vector3b WZY {
			get => new Vector3b( W, Z, Y ); set {
				W = value.X;
				Z = value.Y;
				Y = value.Z;
			}
		}

		public Vector3b WZZ {
			get => new Vector3b( W, Z, Z );
		}

		public Vector3b WZW {
			get => new Vector3b( W, Z, W );
		}

		public Vector3b WWX {
			get => new Vector3b( W, W, X );
		}

		public Vector3b WWY {
			get => new Vector3b( W, W, Y );
		}

		public Vector3b WWZ {
			get => new Vector3b( W, W, Z );
		}

		public Vector3b WWW {
			get => new Vector3b( W, W, W );
		}

		public Vector4b XXXX {
			get => new Vector4b( X, X, X, X );
		}

		public Vector4b XXXY {
			get => new Vector4b( X, X, X, Y );
		}

		public Vector4b XXXZ {
			get => new Vector4b( X, X, X, Z );
		}

		public Vector4b XXXW {
			get => new Vector4b( X, X, X, W );
		}

		public Vector4b XXYX {
			get => new Vector4b( X, X, Y, X );
		}

		public Vector4b XXYY {
			get => new Vector4b( X, X, Y, Y );
		}

		public Vector4b XXYZ {
			get => new Vector4b( X, X, Y, Z );
		}

		public Vector4b XXYW {
			get => new Vector4b( X, X, Y, W );
		}

		public Vector4b XXZX {
			get => new Vector4b( X, X, Z, X );
		}

		public Vector4b XXZY {
			get => new Vector4b( X, X, Z, Y );
		}

		public Vector4b XXZZ {
			get => new Vector4b( X, X, Z, Z );
		}

		public Vector4b XXZW {
			get => new Vector4b( X, X, Z, W );
		}

		public Vector4b XXWX {
			get => new Vector4b( X, X, W, X );
		}

		public Vector4b XXWY {
			get => new Vector4b( X, X, W, Y );
		}

		public Vector4b XXWZ {
			get => new Vector4b( X, X, W, Z );
		}

		public Vector4b XXWW {
			get => new Vector4b( X, X, W, W );
		}

		public Vector4b XYXX {
			get => new Vector4b( X, Y, X, X );
		}

		public Vector4b XYXY {
			get => new Vector4b( X, Y, X, Y );
		}

		public Vector4b XYXZ {
			get => new Vector4b( X, Y, X, Z );
		}

		public Vector4b XYXW {
			get => new Vector4b( X, Y, X, W );
		}

		public Vector4b XYYX {
			get => new Vector4b( X, Y, Y, X );
		}

		public Vector4b XYYY {
			get => new Vector4b( X, Y, Y, Y );
		}

		public Vector4b XYYZ {
			get => new Vector4b( X, Y, Y, Z );
		}

		public Vector4b XYYW {
			get => new Vector4b( X, Y, Y, W );
		}

		public Vector4b XYZX {
			get => new Vector4b( X, Y, Z, X );
		}

		public Vector4b XYZY {
			get => new Vector4b( X, Y, Z, Y );
		}

		public Vector4b XYZZ {
			get => new Vector4b( X, Y, Z, Z );
		}

		public Vector4b XYZW {
			get => new Vector4b( X, Y, Z, W ); set {
				X = value.X;
				Y = value.Y;
				Z = value.Z;
				W = value.W;
			}
		}

		public Vector4b XYWX {
			get => new Vector4b( X, Y, W, X );
		}

		public Vector4b XYWY {
			get => new Vector4b( X, Y, W, Y );
		}

		public Vector4b XYWZ {
			get => new Vector4b( X, Y, W, Z ); set {
				X = value.X;
				Y = value.Y;
				W = value.Z;
				Z = value.W;
			}
		}

		public Vector4b XYWW {
			get => new Vector4b( X, Y, W, W );
		}

		public Vector4b XZXX {
			get => new Vector4b( X, Z, X, X );
		}

		public Vector4b XZXY {
			get => new Vector4b( X, Z, X, Y );
		}

		public Vector4b XZXZ {
			get => new Vector4b( X, Z, X, Z );
		}

		public Vector4b XZXW {
			get => new Vector4b( X, Z, X, W );
		}

		public Vector4b XZYX {
			get => new Vector4b( X, Z, Y, X );
		}

		public Vector4b XZYY {
			get => new Vector4b( X, Z, Y, Y );
		}

		public Vector4b XZYZ {
			get => new Vector4b( X, Z, Y, Z );
		}

		public Vector4b XZYW {
			get => new Vector4b( X, Z, Y, W ); set {
				X = value.X;
				Z = value.Y;
				Y = value.Z;
				W = value.W;
			}
		}

		public Vector4b XZZX {
			get => new Vector4b( X, Z, Z, X );
		}

		public Vector4b XZZY {
			get => new Vector4b( X, Z, Z, Y );
		}

		public Vector4b XZZZ {
			get => new Vector4b( X, Z, Z, Z );
		}

		public Vector4b XZZW {
			get => new Vector4b( X, Z, Z, W );
		}

		public Vector4b XZWX {
			get => new Vector4b( X, Z, W, X );
		}

		public Vector4b XZWY {
			get => new Vector4b( X, Z, W, Y ); set {
				X = value.X;
				Z = value.Y;
				W = value.Z;
				Y = value.W;
			}
		}

		public Vector4b XZWZ {
			get => new Vector4b( X, Z, W, Z );
		}

		public Vector4b XZWW {
			get => new Vector4b( X, Z, W, W );
		}

		public Vector4b XWXX {
			get => new Vector4b( X, W, X, X );
		}

		public Vector4b XWXY {
			get => new Vector4b( X, W, X, Y );
		}

		public Vector4b XWXZ {
			get => new Vector4b( X, W, X, Z );
		}

		public Vector4b XWXW {
			get => new Vector4b( X, W, X, W );
		}

		public Vector4b XWYX {
			get => new Vector4b( X, W, Y, X );
		}

		public Vector4b XWYY {
			get => new Vector4b( X, W, Y, Y );
		}

		public Vector4b XWYZ {
			get => new Vector4b( X, W, Y, Z ); set {
				X = value.X;
				W = value.Y;
				Y = value.Z;
				Z = value.W;
			}
		}

		public Vector4b XWYW {
			get => new Vector4b( X, W, Y, W );
		}

		public Vector4b XWZX {
			get => new Vector4b( X, W, Z, X );
		}

		public Vector4b XWZY {
			get => new Vector4b( X, W, Z, Y ); set {
				X = value.X;
				W = value.Y;
				Z = value.Z;
				Y = value.W;
			}
		}

		public Vector4b XWZZ {
			get => new Vector4b( X, W, Z, Z );
		}

		public Vector4b XWZW {
			get => new Vector4b( X, W, Z, W );
		}

		public Vector4b XWWX {
			get => new Vector4b( X, W, W, X );
		}

		public Vector4b XWWY {
			get => new Vector4b( X, W, W, Y );
		}

		public Vector4b XWWZ {
			get => new Vector4b( X, W, W, Z );
		}

		public Vector4b XWWW {
			get => new Vector4b( X, W, W, W );
		}

		public Vector4b YXXX {
			get => new Vector4b( Y, X, X, X );
		}

		public Vector4b YXXY {
			get => new Vector4b( Y, X, X, Y );
		}

		public Vector4b YXXZ {
			get => new Vector4b( Y, X, X, Z );
		}

		public Vector4b YXXW {
			get => new Vector4b( Y, X, X, W );
		}

		public Vector4b YXYX {
			get => new Vector4b( Y, X, Y, X );
		}

		public Vector4b YXYY {
			get => new Vector4b( Y, X, Y, Y );
		}

		public Vector4b YXYZ {
			get => new Vector4b( Y, X, Y, Z );
		}

		public Vector4b YXYW {
			get => new Vector4b( Y, X, Y, W );
		}

		public Vector4b YXZX {
			get => new Vector4b( Y, X, Z, X );
		}

		public Vector4b YXZY {
			get => new Vector4b( Y, X, Z, Y );
		}

		public Vector4b YXZZ {
			get => new Vector4b( Y, X, Z, Z );
		}

		public Vector4b YXZW {
			get => new Vector4b( Y, X, Z, W ); set {
				Y = value.X;
				X = value.Y;
				Z = value.Z;
				W = value.W;
			}
		}

		public Vector4b YXWX {
			get => new Vector4b( Y, X, W, X );
		}

		public Vector4b YXWY {
			get => new Vector4b( Y, X, W, Y );
		}

		public Vector4b YXWZ {
			get => new Vector4b( Y, X, W, Z ); set {
				Y = value.X;
				X = value.Y;
				W = value.Z;
				Z = value.W;
			}
		}

		public Vector4b YXWW {
			get => new Vector4b( Y, X, W, W );
		}

		public Vector4b YYXX {
			get => new Vector4b( Y, Y, X, X );
		}

		public Vector4b YYXY {
			get => new Vector4b( Y, Y, X, Y );
		}

		public Vector4b YYXZ {
			get => new Vector4b( Y, Y, X, Z );
		}

		public Vector4b YYXW {
			get => new Vector4b( Y, Y, X, W );
		}

		public Vector4b YYYX {
			get => new Vector4b( Y, Y, Y, X );
		}

		public Vector4b YYYY {
			get => new Vector4b( Y, Y, Y, Y );
		}

		public Vector4b YYYZ {
			get => new Vector4b( Y, Y, Y, Z );
		}

		public Vector4b YYYW {
			get => new Vector4b( Y, Y, Y, W );
		}

		public Vector4b YYZX {
			get => new Vector4b( Y, Y, Z, X );
		}

		public Vector4b YYZY {
			get => new Vector4b( Y, Y, Z, Y );
		}

		public Vector4b YYZZ {
			get => new Vector4b( Y, Y, Z, Z );
		}

		public Vector4b YYZW {
			get => new Vector4b( Y, Y, Z, W );
		}

		public Vector4b YYWX {
			get => new Vector4b( Y, Y, W, X );
		}

		public Vector4b YYWY {
			get => new Vector4b( Y, Y, W, Y );
		}

		public Vector4b YYWZ {
			get => new Vector4b( Y, Y, W, Z );
		}

		public Vector4b YYWW {
			get => new Vector4b( Y, Y, W, W );
		}

		public Vector4b YZXX {
			get => new Vector4b( Y, Z, X, X );
		}

		public Vector4b YZXY {
			get => new Vector4b( Y, Z, X, Y );
		}

		public Vector4b YZXZ {
			get => new Vector4b( Y, Z, X, Z );
		}

		public Vector4b YZXW {
			get => new Vector4b( Y, Z, X, W ); set {
				Y = value.X;
				Z = value.Y;
				X = value.Z;
				W = value.W;
			}
		}

		public Vector4b YZYX {
			get => new Vector4b( Y, Z, Y, X );
		}

		public Vector4b YZYY {
			get => new Vector4b( Y, Z, Y, Y );
		}

		public Vector4b YZYZ {
			get => new Vector4b( Y, Z, Y, Z );
		}

		public Vector4b YZYW {
			get => new Vector4b( Y, Z, Y, W );
		}

		public Vector4b YZZX {
			get => new Vector4b( Y, Z, Z, X );
		}

		public Vector4b YZZY {
			get => new Vector4b( Y, Z, Z, Y );
		}

		public Vector4b YZZZ {
			get => new Vector4b( Y, Z, Z, Z );
		}

		public Vector4b YZZW {
			get => new Vector4b( Y, Z, Z, W );
		}

		public Vector4b YZWX {
			get => new Vector4b( Y, Z, W, X ); set {
				Y = value.X;
				Z = value.Y;
				W = value.Z;
				X = value.W;
			}
		}

		public Vector4b YZWY {
			get => new Vector4b( Y, Z, W, Y );
		}

		public Vector4b YZWZ {
			get => new Vector4b( Y, Z, W, Z );
		}

		public Vector4b YZWW {
			get => new Vector4b( Y, Z, W, W );
		}

		public Vector4b YWXX {
			get => new Vector4b( Y, W, X, X );
		}

		public Vector4b YWXY {
			get => new Vector4b( Y, W, X, Y );
		}

		public Vector4b YWXZ {
			get => new Vector4b( Y, W, X, Z ); set {
				Y = value.X;
				W = value.Y;
				X = value.Z;
				Z = value.W;
			}
		}

		public Vector4b YWXW {
			get => new Vector4b( Y, W, X, W );
		}

		public Vector4b YWYX {
			get => new Vector4b( Y, W, Y, X );
		}

		public Vector4b YWYY {
			get => new Vector4b( Y, W, Y, Y );
		}

		public Vector4b YWYZ {
			get => new Vector4b( Y, W, Y, Z );
		}

		public Vector4b YWYW {
			get => new Vector4b( Y, W, Y, W );
		}

		public Vector4b YWZX {
			get => new Vector4b( Y, W, Z, X ); set {
				Y = value.X;
				W = value.Y;
				Z = value.Z;
				X = value.W;
			}
		}

		public Vector4b YWZY {
			get => new Vector4b( Y, W, Z, Y );
		}

		public Vector4b YWZZ {
			get => new Vector4b( Y, W, Z, Z );
		}

		public Vector4b YWZW {
			get => new Vector4b( Y, W, Z, W );
		}

		public Vector4b YWWX {
			get => new Vector4b( Y, W, W, X );
		}

		public Vector4b YWWY {
			get => new Vector4b( Y, W, W, Y );
		}

		public Vector4b YWWZ {
			get => new Vector4b( Y, W, W, Z );
		}

		public Vector4b YWWW {
			get => new Vector4b( Y, W, W, W );
		}

		public Vector4b ZXXX {
			get => new Vector4b( Z, X, X, X );
		}

		public Vector4b ZXXY {
			get => new Vector4b( Z, X, X, Y );
		}

		public Vector4b ZXXZ {
			get => new Vector4b( Z, X, X, Z );
		}

		public Vector4b ZXXW {
			get => new Vector4b( Z, X, X, W );
		}

		public Vector4b ZXYX {
			get => new Vector4b( Z, X, Y, X );
		}

		public Vector4b ZXYY {
			get => new Vector4b( Z, X, Y, Y );
		}

		public Vector4b ZXYZ {
			get => new Vector4b( Z, X, Y, Z );
		}

		public Vector4b ZXYW {
			get => new Vector4b( Z, X, Y, W ); set {
				Z = value.X;
				X = value.Y;
				Y = value.Z;
				W = value.W;
			}
		}

		public Vector4b ZXZX {
			get => new Vector4b( Z, X, Z, X );
		}

		public Vector4b ZXZY {
			get => new Vector4b( Z, X, Z, Y );
		}

		public Vector4b ZXZZ {
			get => new Vector4b( Z, X, Z, Z );
		}

		public Vector4b ZXZW {
			get => new Vector4b( Z, X, Z, W );
		}

		public Vector4b ZXWX {
			get => new Vector4b( Z, X, W, X );
		}

		public Vector4b ZXWY {
			get => new Vector4b( Z, X, W, Y ); set {
				Z = value.X;
				X = value.Y;
				W = value.Z;
				Y = value.W;
			}
		}

		public Vector4b ZXWZ {
			get => new Vector4b( Z, X, W, Z );
		}

		public Vector4b ZXWW {
			get => new Vector4b( Z, X, W, W );
		}

		public Vector4b ZYXX {
			get => new Vector4b( Z, Y, X, X );
		}

		public Vector4b ZYXY {
			get => new Vector4b( Z, Y, X, Y );
		}

		public Vector4b ZYXZ {
			get => new Vector4b( Z, Y, X, Z );
		}

		public Vector4b ZYXW {
			get => new Vector4b( Z, Y, X, W ); set {
				Z = value.X;
				Y = value.Y;
				X = value.Z;
				W = value.W;
			}
		}

		public Vector4b ZYYX {
			get => new Vector4b( Z, Y, Y, X );
		}

		public Vector4b ZYYY {
			get => new Vector4b( Z, Y, Y, Y );
		}

		public Vector4b ZYYZ {
			get => new Vector4b( Z, Y, Y, Z );
		}

		public Vector4b ZYYW {
			get => new Vector4b( Z, Y, Y, W );
		}

		public Vector4b ZYZX {
			get => new Vector4b( Z, Y, Z, X );
		}

		public Vector4b ZYZY {
			get => new Vector4b( Z, Y, Z, Y );
		}

		public Vector4b ZYZZ {
			get => new Vector4b( Z, Y, Z, Z );
		}

		public Vector4b ZYZW {
			get => new Vector4b( Z, Y, Z, W );
		}

		public Vector4b ZYWX {
			get => new Vector4b( Z, Y, W, X ); set {
				Z = value.X;
				Y = value.Y;
				W = value.Z;
				X = value.W;
			}
		}

		public Vector4b ZYWY {
			get => new Vector4b( Z, Y, W, Y );
		}

		public Vector4b ZYWZ {
			get => new Vector4b( Z, Y, W, Z );
		}

		public Vector4b ZYWW {
			get => new Vector4b( Z, Y, W, W );
		}

		public Vector4b ZZXX {
			get => new Vector4b( Z, Z, X, X );
		}

		public Vector4b ZZXY {
			get => new Vector4b( Z, Z, X, Y );
		}

		public Vector4b ZZXZ {
			get => new Vector4b( Z, Z, X, Z );
		}

		public Vector4b ZZXW {
			get => new Vector4b( Z, Z, X, W );
		}

		public Vector4b ZZYX {
			get => new Vector4b( Z, Z, Y, X );
		}

		public Vector4b ZZYY {
			get => new Vector4b( Z, Z, Y, Y );
		}

		public Vector4b ZZYZ {
			get => new Vector4b( Z, Z, Y, Z );
		}

		public Vector4b ZZYW {
			get => new Vector4b( Z, Z, Y, W );
		}

		public Vector4b ZZZX {
			get => new Vector4b( Z, Z, Z, X );
		}

		public Vector4b ZZZY {
			get => new Vector4b( Z, Z, Z, Y );
		}

		public Vector4b ZZZZ {
			get => new Vector4b( Z, Z, Z, Z );
		}

		public Vector4b ZZZW {
			get => new Vector4b( Z, Z, Z, W );
		}

		public Vector4b ZZWX {
			get => new Vector4b( Z, Z, W, X );
		}

		public Vector4b ZZWY {
			get => new Vector4b( Z, Z, W, Y );
		}

		public Vector4b ZZWZ {
			get => new Vector4b( Z, Z, W, Z );
		}

		public Vector4b ZZWW {
			get => new Vector4b( Z, Z, W, W );
		}

		public Vector4b ZWXX {
			get => new Vector4b( Z, W, X, X );
		}

		public Vector4b ZWXY {
			get => new Vector4b( Z, W, X, Y ); set {
				Z = value.X;
				W = value.Y;
				X = value.Z;
				Y = value.W;
			}
		}

		public Vector4b ZWXZ {
			get => new Vector4b( Z, W, X, Z );
		}

		public Vector4b ZWXW {
			get => new Vector4b( Z, W, X, W );
		}

		public Vector4b ZWYX {
			get => new Vector4b( Z, W, Y, X ); set {
				Z = value.X;
				W = value.Y;
				Y = value.Z;
				X = value.W;
			}
		}

		public Vector4b ZWYY {
			get => new Vector4b( Z, W, Y, Y );
		}

		public Vector4b ZWYZ {
			get => new Vector4b( Z, W, Y, Z );
		}

		public Vector4b ZWYW {
			get => new Vector4b( Z, W, Y, W );
		}

		public Vector4b ZWZX {
			get => new Vector4b( Z, W, Z, X );
		}

		public Vector4b ZWZY {
			get => new Vector4b( Z, W, Z, Y );
		}

		public Vector4b ZWZZ {
			get => new Vector4b( Z, W, Z, Z );
		}

		public Vector4b ZWZW {
			get => new Vector4b( Z, W, Z, W );
		}

		public Vector4b ZWWX {
			get => new Vector4b( Z, W, W, X );
		}

		public Vector4b ZWWY {
			get => new Vector4b( Z, W, W, Y );
		}

		public Vector4b ZWWZ {
			get => new Vector4b( Z, W, W, Z );
		}

		public Vector4b ZWWW {
			get => new Vector4b( Z, W, W, W );
		}

		public Vector4b WXXX {
			get => new Vector4b( W, X, X, X );
		}

		public Vector4b WXXY {
			get => new Vector4b( W, X, X, Y );
		}

		public Vector4b WXXZ {
			get => new Vector4b( W, X, X, Z );
		}

		public Vector4b WXXW {
			get => new Vector4b( W, X, X, W );
		}

		public Vector4b WXYX {
			get => new Vector4b( W, X, Y, X );
		}

		public Vector4b WXYY {
			get => new Vector4b( W, X, Y, Y );
		}

		public Vector4b WXYZ {
			get => new Vector4b( W, X, Y, Z ); set {
				W = value.X;
				X = value.Y;
				Y = value.Z;
				Z = value.W;
			}
		}

		public Vector4b WXYW {
			get => new Vector4b( W, X, Y, W );
		}

		public Vector4b WXZX {
			get => new Vector4b( W, X, Z, X );
		}

		public Vector4b WXZY {
			get => new Vector4b( W, X, Z, Y ); set {
				W = value.X;
				X = value.Y;
				Z = value.Z;
				Y = value.W;
			}
		}

		public Vector4b WXZZ {
			get => new Vector4b( W, X, Z, Z );
		}

		public Vector4b WXZW {
			get => new Vector4b( W, X, Z, W );
		}

		public Vector4b WXWX {
			get => new Vector4b( W, X, W, X );
		}

		public Vector4b WXWY {
			get => new Vector4b( W, X, W, Y );
		}

		public Vector4b WXWZ {
			get => new Vector4b( W, X, W, Z );
		}

		public Vector4b WXWW {
			get => new Vector4b( W, X, W, W );
		}

		public Vector4b WYXX {
			get => new Vector4b( W, Y, X, X );
		}

		public Vector4b WYXY {
			get => new Vector4b( W, Y, X, Y );
		}

		public Vector4b WYXZ {
			get => new Vector4b( W, Y, X, Z ); set {
				W = value.X;
				Y = value.Y;
				X = value.Z;
				Z = value.W;
			}
		}

		public Vector4b WYXW {
			get => new Vector4b( W, Y, X, W );
		}

		public Vector4b WYYX {
			get => new Vector4b( W, Y, Y, X );
		}

		public Vector4b WYYY {
			get => new Vector4b( W, Y, Y, Y );
		}

		public Vector4b WYYZ {
			get => new Vector4b( W, Y, Y, Z );
		}

		public Vector4b WYYW {
			get => new Vector4b( W, Y, Y, W );
		}

		public Vector4b WYZX {
			get => new Vector4b( W, Y, Z, X ); set {
				W = value.X;
				Y = value.Y;
				Z = value.Z;
				X = value.W;
			}
		}

		public Vector4b WYZY {
			get => new Vector4b( W, Y, Z, Y );
		}

		public Vector4b WYZZ {
			get => new Vector4b( W, Y, Z, Z );
		}

		public Vector4b WYZW {
			get => new Vector4b( W, Y, Z, W );
		}

		public Vector4b WYWX {
			get => new Vector4b( W, Y, W, X );
		}

		public Vector4b WYWY {
			get => new Vector4b( W, Y, W, Y );
		}

		public Vector4b WYWZ {
			get => new Vector4b( W, Y, W, Z );
		}

		public Vector4b WYWW {
			get => new Vector4b( W, Y, W, W );
		}

		public Vector4b WZXX {
			get => new Vector4b( W, Z, X, X );
		}

		public Vector4b WZXY {
			get => new Vector4b( W, Z, X, Y ); set {
				W = value.X;
				Z = value.Y;
				X = value.Z;
				Y = value.W;
			}
		}

		public Vector4b WZXZ {
			get => new Vector4b( W, Z, X, Z );
		}

		public Vector4b WZXW {
			get => new Vector4b( W, Z, X, W );
		}

		public Vector4b WZYX {
			get => new Vector4b( W, Z, Y, X ); set {
				W = value.X;
				Z = value.Y;
				Y = value.Z;
				X = value.W;
			}
		}

		public Vector4b WZYY {
			get => new Vector4b( W, Z, Y, Y );
		}

		public Vector4b WZYZ {
			get => new Vector4b( W, Z, Y, Z );
		}

		public Vector4b WZYW {
			get => new Vector4b( W, Z, Y, W );
		}

		public Vector4b WZZX {
			get => new Vector4b( W, Z, Z, X );
		}

		public Vector4b WZZY {
			get => new Vector4b( W, Z, Z, Y );
		}

		public Vector4b WZZZ {
			get => new Vector4b( W, Z, Z, Z );
		}

		public Vector4b WZZW {
			get => new Vector4b( W, Z, Z, W );
		}

		public Vector4b WZWX {
			get => new Vector4b( W, Z, W, X );
		}

		public Vector4b WZWY {
			get => new Vector4b( W, Z, W, Y );
		}

		public Vector4b WZWZ {
			get => new Vector4b( W, Z, W, Z );
		}

		public Vector4b WZWW {
			get => new Vector4b( W, Z, W, W );
		}

		public Vector4b WWXX {
			get => new Vector4b( W, W, X, X );
		}

		public Vector4b WWXY {
			get => new Vector4b( W, W, X, Y );
		}

		public Vector4b WWXZ {
			get => new Vector4b( W, W, X, Z );
		}

		public Vector4b WWXW {
			get => new Vector4b( W, W, X, W );
		}

		public Vector4b WWYX {
			get => new Vector4b( W, W, Y, X );
		}

		public Vector4b WWYY {
			get => new Vector4b( W, W, Y, Y );
		}

		public Vector4b WWYZ {
			get => new Vector4b( W, W, Y, Z );
		}

		public Vector4b WWYW {
			get => new Vector4b( W, W, Y, W );
		}

		public Vector4b WWZX {
			get => new Vector4b( W, W, Z, X );
		}

		public Vector4b WWZY {
			get => new Vector4b( W, W, Z, Y );
		}

		public Vector4b WWZZ {
			get => new Vector4b( W, W, Z, Z );
		}

		public Vector4b WWZW {
			get => new Vector4b( W, W, Z, W );
		}

		public Vector4b WWWX {
			get => new Vector4b( W, W, W, X );
		}

		public Vector4b WWWY {
			get => new Vector4b( W, W, W, Y );
		}

		public Vector4b WWWZ {
			get => new Vector4b( W, W, W, Z );
		}

		public Vector4b WWWW {
			get => new Vector4b( W, W, W, W );
		}
		#endregion

		#region Operators
		public static Vector4b operator +( Vector4b a, Vector4b b ) {
			return new Vector4b( (byte) ( a.X + b.X ), (byte) ( a.Y + b.Y ), (byte) ( a.Z + b.Z ), (byte) ( a.W + b.W ) );
		}

		public static Vector4b operator *( Vector4b a, byte scalar ) {
			return new Vector4b( (byte) ( a.X * scalar ), (byte) ( a.Y * scalar ), (byte) ( a.Z * scalar ), (byte) ( a.W * scalar ) );
		}

		public static Vector4b operator *( byte scalar, Vector4b a ) {
			return new Vector4b( (byte) ( a.X * scalar ), (byte) ( a.Y * scalar ), (byte) ( a.Z * scalar ), (byte) ( a.W * scalar ) );
		}

		public static Vector4b operator *( Vector4b a, float scalar ) {
			return new Vector4( a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar ).IntRounded.AsByte;
		}

		public static Vector4b operator *( float scalar, Vector4b a ) {
			return new Vector4( a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar ).IntRounded.AsByte;
		}

		public static Vector4b operator *( Vector4b a, Vector4b b ) {
			return new Vector4b( (byte) ( a.X * b.X ), (byte) ( a.Y * b.Y ), (byte) ( a.Z * b.Z ), (byte) ( a.W * b.W ) );
		}

		public static Vector4b operator *( Vector4b a, Vector4 b ) {
			return new Vector4( a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W ).IntFloored.AsByte;
		}

		public static Vector4b operator *( Vector4 a, Vector4b b ) {
			return new Vector4( a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W ).IntFloored.AsByte;
		}

		public static Vector4b operator -( Vector4b a, Vector4b b ) {
			return new Vector4b( (byte) ( a.X - b.X ), (byte) ( a.Y - b.Y ), (byte) ( a.Z - b.Z ), (byte) ( a.W - b.W ) );
		}

		public static Vector4b operator /( Vector4b a, int divident ) {
			return ( a * ( 1f / divident ) );
		}

		public static Vector4b operator /( Vector4b a, float divident ) {
			return a * ( 1f / divident );
		}

		public static bool operator ==( Vector4b a, Vector4b b ) {
			return a.Equals( b );
		}

		public static bool operator !=( Vector4b a, Vector4b b ) {
			return !a.Equals( b );
		}

		#region Implicit
		public static implicit operator Vector4b( byte[] a ) {
			if( a.Length != 4 )
				throw new System.ArgumentException( "Length of array is not compatible with Vector4b" );
			return new Vector4b( a[ 0 ], a[ 1 ], a[ 2 ], a[ 3 ] );
		}

		public static implicit operator Vector4b( byte a ) {
			return new Vector4b( a );
		}

		public static implicit operator byte[]( Vector4b a ) {
			return new byte[] { a.X, a.Y, a.Z, a.W };
		}
		#endregion
		#endregion

	}
}
