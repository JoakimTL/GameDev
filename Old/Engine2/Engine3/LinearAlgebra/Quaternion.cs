
using System.Data;

namespace Engine.LinearAlgebra {
	[System.Serializable]
	[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
	public struct Quaternion : System.IEquatable<Quaternion> {

		public float X, Y, Z, W;

		public Vector4 AsVector => new Vector4( X, Y, Z, W );

		public float Length => (float) System.Math.Sqrt( X * X + Y * Y + Z * Z + W * W );
		public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

		public Quaternion Conjugated => Conjugate( ref this );
		public Quaternion Normalized => Normalize( ref this );

		public Vector3 Forward => Vector3.Transform( -Vector3.UnitZ, this );
		public Vector3 Backward => Vector3.Transform( Vector3.UnitZ, this );
		public Vector3 Down => Vector3.Transform( -Vector3.UnitY, this );
		public Vector3 Up => Vector3.Transform( Vector3.UnitY, this );
		public Vector3 Left => Vector3.Transform( -Vector3.UnitX, this );
		public Vector3 Right => Vector3.Transform( Vector3.UnitX, this );

		public Quaternion( float x, float y, float z, float w ) {
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Quaternion( Vector3 XYZ, float W ) : this( XYZ.X, XYZ.Y, XYZ.Z, W ) { }

		public static readonly Quaternion Identity = new Quaternion( 0, 0, 0, 1 );

		public void Normalize() {
			Normalize( ref this );
		}

		#region ToAxisAngle

		public void ToAxisAngle( out Vector3 axis, out float angle ) {
			Vector4 result = ToAxisAngle();
			axis = result.XYZ;
			angle = result.W;
		}

		public Vector4 ToAxisAngle() {
			Quaternion q = this;
			if( System.Math.Abs( q.W ) > 1.0f )
				q.Normalize();

			Vector4 result = new Vector4();

			result.W = 2.0f * (float) System.Math.Acos( q.W ); // angle
			float den = (float) System.Math.Sqrt( 1.0 - q.W * q.W );
			if( den > 0.0001f ) {
				result.XYZ = q.XYZ / den;
			} else {
				result.XYZ = Vector3.UnitX;
			}

			return result;
		}

		#endregion

		#region FromAxisAngle
		/// <summary>
		/// Creates a quaternion rotated around the input axis, using the length of the axis to determine the amount of rotation
		/// </summary>
		/// <param name="vector">Axis of rotation</param>
		/// <param name="result">Resulting quaterion</param>
		public static void FromAxisVector( ref Vector3 vector, out Quaternion result ) {
			result = Identity;
			if( vector == 0 )
				return;
			float length = vector.Length;
			float angle = length * 0.5f;
			result.XYZ = vector / length * (float) System.Math.Sin( angle );
			result.W = (float) System.Math.Cos( angle );
			result.Normalize();
		}

		/// <summary>
		/// Creates a quaternion rotated around the input axis, using the length of the axis to determine the amount of rotation
		/// </summary>
		/// <param name="vector">Axis of rotation</param>
		/// <returns>Resulting quaterion</returns>
		public static Quaternion FromAxisVector( ref Vector3 vector ) {
			FromAxisVector( ref vector, out Quaternion result );
			return result;
		}

		/// <summary>
		/// Creates a quaternion rotated around the input axis, using the length of the axis to determine the amount of rotation
		/// </summary>
		/// <param name="vector">Axis of rotation</param>
		/// <returns>Resulting quaterion</returns>
		public static Quaternion FromAxisVector( Vector3 vector ) {
			return FromAxisVector( ref vector );
		}

		public static void FromAxisAngle( ref Vector3 axis, float angle, out Quaternion result ) {
			result = Identity;
			if( axis.LengthSquared == 0.0f )
				return;
			angle *= 0.5f;
			axis.Normalize();
			result.XYZ = axis * (float) System.Math.Sin( angle );
			result.W = (float) System.Math.Cos( angle );
			result.Normalize();
		}

		public static Quaternion FromAxisAngle( ref Vector3 axis, float angle ) {
			FromAxisAngle( ref axis, angle, out Quaternion r );
			return r;
		}

		public static Quaternion FromAxisAngle( Vector3 axis, float angle ) {
			FromAxisAngle( ref axis, angle, out Quaternion r );
			return r;
		}

		#endregion

		#region FromEuler
		public static Quaternion FromEulerRads( Vector3 angles ) {
			//https://stackoverflow.com/questions/11492299/quaternion-to-euler-angles-algorithm-how-to-convert-to-y-up-and-between-ha
			angles *= 0.5f;
			float cx = (float) System.Math.Cos( angles.X ); //cos(yaw)
			float sx = (float) System.Math.Sin( angles.X ); //sin(yaw)
			float cy = (float) System.Math.Cos( angles.Y ); //cos(pitch)
			float sy = (float) System.Math.Sin( angles.Y ); //sin(pitch)
			float cz = (float) System.Math.Cos( angles.Z ); //cos(roll)
			float sz = (float) System.Math.Sin( angles.Z ); //sin(roll)

			Quaternion q = new Quaternion(
				sz * cy * cx - cz * sy * sx,
				cz * sy * cx + sz * cy * sx,
				cz * cy * sx - sz * sy * cx,
				cz * cy * cx + sz * sy * sx
			);

			return q;
		}

		const float DegToRad = 3.14159f / 180f;

		public static Quaternion FromEulerRads( float yaw, float pitch, float roll ) => FromEulerRads( (yaw, pitch, roll) );

		public static Quaternion FromEulerDeg( Vector3 angles ) => FromEulerRads( angles * DegToRad );

		public static Quaternion FromEulerDeg( float yaw, float pitch, float roll ) => FromEulerDeg( (yaw, pitch, roll) );
		#endregion

		#region FromVectors
		public static Quaternion LookTowards( Vector3 forward, Vector3 up ) {
			Vector3 F = Vector3.Normalize( forward.ZXY );   // lookAt
			Vector3 R = Vector3.Normalize( Vector3.Cross( up.ZXY, F ) ); // sideaxis
			Vector3 U = Vector3.Cross( F, R );                  // rotatedup

			// note that R needed to be re-normalized
			// since F and worldUp are not necessary perpendicular
			// so must remove the sin(angle) factor of the cross-product
			// same not true for U because dot(R, F) = 0

			// adapted source
			Quaternion q;
			double trace = R.X + U.Y + F.Z;
			if( trace > 0.0 ) {
				float s = 0.5f / (float) System.Math.Sqrt( trace + 1.0 );
				q.W = 0.25f / s;
				q.X = ( U.Z - F.Y ) * s;
				q.Y = ( F.X - R.Z ) * s;
				q.Z = ( R.Y - U.X ) * s;
			} else {
				if( R.X > U.Y && R.X > F.Z ) {
					float s = 2.0f * (float) System.Math.Sqrt( 1.0 + R.X - U.Y - F.Z );
					q.W = ( U.Z - F.Y ) / s;
					q.X = 0.25f * s;
					q.Y = ( U.X + R.Y ) / s;
					q.Z = ( F.X + R.Z ) / s;
				} else if( U.Y > F.Z ) {
					float s = 2.0f * (float) System.Math.Sqrt( 1.0 + U.Y - R.X - F.Z );
					q.W = ( F.X - R.Z ) / s;
					q.X = ( U.X + R.Y ) / s;
					q.Y = 0.25f * s;
					q.Z = ( F.Y + U.Z ) / s;
				} else {
					float s = 2.0f * (float) System.Math.Sqrt( 1.0 + F.Z - R.X - U.Y );
					q.W = ( R.Y - U.X ) / s;
					q.X = ( F.X + R.Z ) / s;
					q.Y = ( F.Y + U.Z ) / s;
					q.Z = 0.25f * s;
				}
			}

			return q.Normalized.Conjugated;
			/*forward = forward.Normalized;
			Vector3 right = Vector3.Cross( forward, up ).Normalized;
			up = Vector3.Cross( forward, right );

			/*float M00 = right.X;
			float M01 = up.X;
			float M02 = forward.X;
			float M10 = right.Y;
			float M11 = up.Y;
			float M12 = forward.Y;
			float M20 = right.Z;
			float M21 = up.Z;
			float M22 = forward.Z;
			
				M00 = x.X,
				M01 = y.X,
				M02 = z.X,
				M10 = x.Y,
				M11 = y.Y,
				M12 = z.Y,
				M20 = x.Z,
				M21 = y.Z,
				M22 = z.Z,*/
			/*float M00 = right.X;
			float M01 = up.X;
			float M02 = forward.X;
			float M10 = right.Y;
			float M11 = up.Y;
			float M12 = forward.Y;
			float M20 = right.Z;
			float M21 = up.Z;
			float M22 = forward.Z;

			float tr = M00 + M11 + M22;
			float qx, qy, qz, qw;
			if( tr > 0 ) {
				float S = (float) Math.Sqrt( tr + 1.0 ) * 2; // S=4*qw 
				qw = 0.25f * S;
				qx = ( M21 - M12 ) / S;
				qy = ( M02 - M20 ) / S;
				qz = ( M10 - M01 ) / S;
			} else if( ( M00 > M11 ) && ( M00 > M22 ) ) {
				float S = (float) Math.Sqrt( 1.0 + M00 - M11 - M22 ) * 2; // S=4*qx 
				qw = ( M21 - M12 ) / S;
				qx = 0.25f * S;
				qy = ( M01 + M10 ) / S;
				qz = ( M02 + M20 ) / S;
			} else if( M11 > M22 ) {
				float S = (float) Math.Sqrt( 1.0 + M11 - M00 - M22 ) * 2; // S=4*qy
				qw = ( M02 - M20 ) / S;
				qx = ( M01 + M10 ) / S;
				qy = 0.25f * S;
				qz = ( M12 + M21 ) / S;
			} else {
				float S = (float) Math.Sqrt( 1.0 + M22 - M00 - M11 ) * 2; // S=4*qz
				qw = ( M10 - M01 ) / S;
				qx = ( M02 + M20 ) / S;
				qy = ( M12 + M21 ) / S;
				qz = 0.25f * S;
			}
			return new Quaternion( qx, qy, qz, qw ).Conjugated.Normalized;*/
			/*
			 Vector forward = lookAt; Vector up = upDirection;
			Vector::OrthoNormalize(&forward, &up);
			Vector right = Vector::Cross(up, forward);

			#define m 00 right.x
			#define m 01 up.x
			#define m 02 forward.x
			#define m 10 right.y
			#define m 11 up.y
			#define m 12 forward.y
			#define m 20 right.z
			#define m 21 up.z
			#define m 22 forward.z

			Quaternion ret;
			ret.w = sqrtf(1.0f + m 00 + m 11 + m 22) * 0.5f;
			float w4_recip = 1.0f / (4.0f * ret.w);
			ret.x = (m 21 - m 12) * w4_recip;
			ret.y = (m 02 - m 20) * w4_recip;
			ret.z = (m 10 - m 01) * w4_recip;

			#undef m 00
			#undef m 01
			#undef m 02
			#undef m 10
			#undef m 11
			#undef m 12
			#undef m 20
			#undef m 21
			#undef m 22

			return ret;
			 */
		}
		#endregion

		#region FromMatrix
		public static Quaternion FromMatrix( Matrix4 mat ) {
			float tr = mat.M00 + mat.M11 + mat.M22;
			float qx, qy, qz, qw;
			if( tr > 0 ) {
				float S = (float) System.Math.Sqrt( tr + 1.0 ) * 2; // S=4*qw 
				qw = 0.25f * S;
				qx = ( mat.M21 - mat.M12 ) / S;
				qy = ( mat.M02 - mat.M20 ) / S;
				qz = ( mat.M10 - mat.M01 ) / S;
			} else if( ( mat.M00 > mat.M11 ) & ( mat.M00 > mat.M22 ) ) {
				float S = (float) System.Math.Sqrt( 1.0 + mat.M00 - mat.M11 - mat.M22 ) * 2; // S=4*qx 
				qw = ( mat.M21 - mat.M12 ) / S;
				qx = 0.25f * S;
				qy = ( mat.M01 + mat.M10 ) / S;
				qz = ( mat.M02 + mat.M20 ) / S;
			} else if( mat.M11 > mat.M22 ) {
				float S = (float) System.Math.Sqrt( 1.0 + mat.M11 - mat.M00 - mat.M22 ) * 2; // S=4*qy
				qw = ( mat.M02 - mat.M20 ) / S;
				qx = ( mat.M01 + mat.M10 ) / S;
				qy = 0.25f * S;
				qz = ( mat.M12 + mat.M21 ) / S;
			} else {
				float S = (float) System.Math.Sqrt( 1.0 + mat.M22 - mat.M00 - mat.M11 ) * 2; // S=4*qz
				qw = ( mat.M10 - mat.M01 ) / S;
				qx = ( mat.M02 + mat.M20 ) / S;
				qy = ( mat.M12 + mat.M21 ) / S;
				qz = 0.25f * S;
			}
			return new Quaternion( qx, qy, qz, qw );
		}
		#endregion

		#region Conjugate

		public static void Conjugate( ref Quaternion q, out Quaternion result ) {
			result = new Quaternion( -q.XYZ, q.W );
		}

		public static Quaternion Conjugate( ref Quaternion q ) {
			Conjugate( ref q, out Quaternion r );
			return r;
		}

		public static Quaternion Conjugate( Quaternion q ) {
			Conjugate( ref q, out Quaternion r );
			return r;
		}

		#endregion

		#region Normalize

		public static void Normalize( ref Quaternion a, out Quaternion result ) {
			result = a / a.Length;
		}

		public static Quaternion Normalize( ref Quaternion a ) {
			Normalize( ref a, out Quaternion r );
			return r;
		}

		public static Quaternion Normalize( Quaternion a ) {
			Normalize( ref a, out Quaternion r );
			return r;
		}

		#endregion

		#region Multiply

		public static void Multiply( ref Quaternion left, ref Quaternion right, out Quaternion result ) {
			result = new Quaternion(
				right.W * left.XYZ + left.W * right.XYZ + Vector3.Cross( left.XYZ, right.XYZ ),
				left.W * right.W - Vector3.Dot( left.XYZ, right.XYZ ) );
		}

		public static Quaternion Multiply( ref Quaternion left, ref Quaternion right ) {
			Multiply( ref left, ref right, out Quaternion r );
			return r;
		}

		public static Quaternion Multiply( Quaternion left, Quaternion right ) {
			Multiply( ref left, ref right, out Quaternion r );
			return r;
		}

		#endregion

		#region Invert

		public static void Invert( ref Quaternion q, out Quaternion result ) {
			float lengthSq = q.LengthSquared;
			if( lengthSq != 0.0 ) {
				float i = 1.0f / lengthSq;
				result = new Quaternion( q.XYZ * -i, q.W * i );
			} else {
				result = q;
			}
		}

		public static Quaternion Invert( ref Quaternion q ) {
			Invert( ref q, out Quaternion r );
			return r;
		}

		public static Quaternion Invert( Quaternion q ) {
			Invert( ref q, out Quaternion r );
			return r;
		}

		#endregion

		#region Override and Equals

		public override bool Equals( object other ) {
			if( other is Quaternion == false )
				return false;
			return Equals( (Quaternion) other );
		}

		public override int GetHashCode() {
			return AsVector.GetHashCode();
		}

		public bool Equals( Quaternion other ) {
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

		public override string ToString() {
			return $"[({string.Format( "{0,6:F2}", X )},{string.Format( "{0,6:F2}", Y )},{string.Format( "{0,6:F2}", Z )}) : {string.Format( "{0,6:F2}", W )}]";
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

		public Quaternion XXXX {
			get => new Quaternion( X, X, X, X );
		}

		public Quaternion XXXY {
			get => new Quaternion( X, X, X, Y );
		}

		public Quaternion XXXZ {
			get => new Quaternion( X, X, X, Z );
		}

		public Quaternion XXXW {
			get => new Quaternion( X, X, X, W );
		}

		public Quaternion XXYX {
			get => new Quaternion( X, X, Y, X );
		}

		public Quaternion XXYY {
			get => new Quaternion( X, X, Y, Y );
		}

		public Quaternion XXYZ {
			get => new Quaternion( X, X, Y, Z );
		}

		public Quaternion XXYW {
			get => new Quaternion( X, X, Y, W );
		}

		public Quaternion XXZX {
			get => new Quaternion( X, X, Z, X );
		}

		public Quaternion XXZY {
			get => new Quaternion( X, X, Z, Y );
		}

		public Quaternion XXZZ {
			get => new Quaternion( X, X, Z, Z );
		}

		public Quaternion XXZW {
			get => new Quaternion( X, X, Z, W );
		}

		public Quaternion XXWX {
			get => new Quaternion( X, X, W, X );
		}

		public Quaternion XXWY {
			get => new Quaternion( X, X, W, Y );
		}

		public Quaternion XXWZ {
			get => new Quaternion( X, X, W, Z );
		}

		public Quaternion XXWW {
			get => new Quaternion( X, X, W, W );
		}

		public Quaternion XYXX {
			get => new Quaternion( X, Y, X, X );
		}

		public Quaternion XYXY {
			get => new Quaternion( X, Y, X, Y );
		}

		public Quaternion XYXZ {
			get => new Quaternion( X, Y, X, Z );
		}

		public Quaternion XYXW {
			get => new Quaternion( X, Y, X, W );
		}

		public Quaternion XYYX {
			get => new Quaternion( X, Y, Y, X );
		}

		public Quaternion XYYY {
			get => new Quaternion( X, Y, Y, Y );
		}

		public Quaternion XYYZ {
			get => new Quaternion( X, Y, Y, Z );
		}

		public Quaternion XYYW {
			get => new Quaternion( X, Y, Y, W );
		}

		public Quaternion XYZX {
			get => new Quaternion( X, Y, Z, X );
		}

		public Quaternion XYZY {
			get => new Quaternion( X, Y, Z, Y );
		}

		public Quaternion XYZZ {
			get => new Quaternion( X, Y, Z, Z );
		}

		public Quaternion XYZW {
			get => new Quaternion( X, Y, Z, W ); set {
				X = value.X;
				Y = value.Y;
				Z = value.Z;
				W = value.W;
			}
		}

		public Quaternion XYWX {
			get => new Quaternion( X, Y, W, X );
		}

		public Quaternion XYWY {
			get => new Quaternion( X, Y, W, Y );
		}

		public Quaternion XYWZ {
			get => new Quaternion( X, Y, W, Z ); set {
				X = value.X;
				Y = value.Y;
				W = value.Z;
				Z = value.W;
			}
		}

		public Quaternion XYWW {
			get => new Quaternion( X, Y, W, W );
		}

		public Quaternion XZXX {
			get => new Quaternion( X, Z, X, X );
		}

		public Quaternion XZXY {
			get => new Quaternion( X, Z, X, Y );
		}

		public Quaternion XZXZ {
			get => new Quaternion( X, Z, X, Z );
		}

		public Quaternion XZXW {
			get => new Quaternion( X, Z, X, W );
		}

		public Quaternion XZYX {
			get => new Quaternion( X, Z, Y, X );
		}

		public Quaternion XZYY {
			get => new Quaternion( X, Z, Y, Y );
		}

		public Quaternion XZYZ {
			get => new Quaternion( X, Z, Y, Z );
		}

		public Quaternion XZYW {
			get => new Quaternion( X, Z, Y, W ); set {
				X = value.X;
				Z = value.Y;
				Y = value.Z;
				W = value.W;
			}
		}

		public Quaternion XZZX {
			get => new Quaternion( X, Z, Z, X );
		}

		public Quaternion XZZY {
			get => new Quaternion( X, Z, Z, Y );
		}

		public Quaternion XZZZ {
			get => new Quaternion( X, Z, Z, Z );
		}

		public Quaternion XZZW {
			get => new Quaternion( X, Z, Z, W );
		}

		public Quaternion XZWX {
			get => new Quaternion( X, Z, W, X );
		}

		public Quaternion XZWY {
			get => new Quaternion( X, Z, W, Y ); set {
				X = value.X;
				Z = value.Y;
				W = value.Z;
				Y = value.W;
			}
		}

		public Quaternion XZWZ {
			get => new Quaternion( X, Z, W, Z );
		}

		public Quaternion XZWW {
			get => new Quaternion( X, Z, W, W );
		}

		public Quaternion XWXX {
			get => new Quaternion( X, W, X, X );
		}

		public Quaternion XWXY {
			get => new Quaternion( X, W, X, Y );
		}

		public Quaternion XWXZ {
			get => new Quaternion( X, W, X, Z );
		}

		public Quaternion XWXW {
			get => new Quaternion( X, W, X, W );
		}

		public Quaternion XWYX {
			get => new Quaternion( X, W, Y, X );
		}

		public Quaternion XWYY {
			get => new Quaternion( X, W, Y, Y );
		}

		public Quaternion XWYZ {
			get => new Quaternion( X, W, Y, Z ); set {
				X = value.X;
				W = value.Y;
				Y = value.Z;
				Z = value.W;
			}
		}

		public Quaternion XWYW {
			get => new Quaternion( X, W, Y, W );
		}

		public Quaternion XWZX {
			get => new Quaternion( X, W, Z, X );
		}

		public Quaternion XWZY {
			get => new Quaternion( X, W, Z, Y ); set {
				X = value.X;
				W = value.Y;
				Z = value.Z;
				Y = value.W;
			}
		}

		public Quaternion XWZZ {
			get => new Quaternion( X, W, Z, Z );
		}

		public Quaternion XWZW {
			get => new Quaternion( X, W, Z, W );
		}

		public Quaternion XWWX {
			get => new Quaternion( X, W, W, X );
		}

		public Quaternion XWWY {
			get => new Quaternion( X, W, W, Y );
		}

		public Quaternion XWWZ {
			get => new Quaternion( X, W, W, Z );
		}

		public Quaternion XWWW {
			get => new Quaternion( X, W, W, W );
		}

		public Quaternion YXXX {
			get => new Quaternion( Y, X, X, X );
		}

		public Quaternion YXXY {
			get => new Quaternion( Y, X, X, Y );
		}

		public Quaternion YXXZ {
			get => new Quaternion( Y, X, X, Z );
		}

		public Quaternion YXXW {
			get => new Quaternion( Y, X, X, W );
		}

		public Quaternion YXYX {
			get => new Quaternion( Y, X, Y, X );
		}

		public Quaternion YXYY {
			get => new Quaternion( Y, X, Y, Y );
		}

		public Quaternion YXYZ {
			get => new Quaternion( Y, X, Y, Z );
		}

		public Quaternion YXYW {
			get => new Quaternion( Y, X, Y, W );
		}

		public Quaternion YXZX {
			get => new Quaternion( Y, X, Z, X );
		}

		public Quaternion YXZY {
			get => new Quaternion( Y, X, Z, Y );
		}

		public Quaternion YXZZ {
			get => new Quaternion( Y, X, Z, Z );
		}

		public Quaternion YXZW {
			get => new Quaternion( Y, X, Z, W ); set {
				Y = value.X;
				X = value.Y;
				Z = value.Z;
				W = value.W;
			}
		}

		public Quaternion YXWX {
			get => new Quaternion( Y, X, W, X );
		}

		public Quaternion YXWY {
			get => new Quaternion( Y, X, W, Y );
		}

		public Quaternion YXWZ {
			get => new Quaternion( Y, X, W, Z ); set {
				Y = value.X;
				X = value.Y;
				W = value.Z;
				Z = value.W;
			}
		}

		public Quaternion YXWW {
			get => new Quaternion( Y, X, W, W );
		}

		public Quaternion YYXX {
			get => new Quaternion( Y, Y, X, X );
		}

		public Quaternion YYXY {
			get => new Quaternion( Y, Y, X, Y );
		}

		public Quaternion YYXZ {
			get => new Quaternion( Y, Y, X, Z );
		}

		public Quaternion YYXW {
			get => new Quaternion( Y, Y, X, W );
		}

		public Quaternion YYYX {
			get => new Quaternion( Y, Y, Y, X );
		}

		public Quaternion YYYY {
			get => new Quaternion( Y, Y, Y, Y );
		}

		public Quaternion YYYZ {
			get => new Quaternion( Y, Y, Y, Z );
		}

		public Quaternion YYYW {
			get => new Quaternion( Y, Y, Y, W );
		}

		public Quaternion YYZX {
			get => new Quaternion( Y, Y, Z, X );
		}

		public Quaternion YYZY {
			get => new Quaternion( Y, Y, Z, Y );
		}

		public Quaternion YYZZ {
			get => new Quaternion( Y, Y, Z, Z );
		}

		public Quaternion YYZW {
			get => new Quaternion( Y, Y, Z, W );
		}

		public Quaternion YYWX {
			get => new Quaternion( Y, Y, W, X );
		}

		public Quaternion YYWY {
			get => new Quaternion( Y, Y, W, Y );
		}

		public Quaternion YYWZ {
			get => new Quaternion( Y, Y, W, Z );
		}

		public Quaternion YYWW {
			get => new Quaternion( Y, Y, W, W );
		}

		public Quaternion YZXX {
			get => new Quaternion( Y, Z, X, X );
		}

		public Quaternion YZXY {
			get => new Quaternion( Y, Z, X, Y );
		}

		public Quaternion YZXZ {
			get => new Quaternion( Y, Z, X, Z );
		}

		public Quaternion YZXW {
			get => new Quaternion( Y, Z, X, W ); set {
				Y = value.X;
				Z = value.Y;
				X = value.Z;
				W = value.W;
			}
		}

		public Quaternion YZYX {
			get => new Quaternion( Y, Z, Y, X );
		}

		public Quaternion YZYY {
			get => new Quaternion( Y, Z, Y, Y );
		}

		public Quaternion YZYZ {
			get => new Quaternion( Y, Z, Y, Z );
		}

		public Quaternion YZYW {
			get => new Quaternion( Y, Z, Y, W );
		}

		public Quaternion YZZX {
			get => new Quaternion( Y, Z, Z, X );
		}

		public Quaternion YZZY {
			get => new Quaternion( Y, Z, Z, Y );
		}

		public Quaternion YZZZ {
			get => new Quaternion( Y, Z, Z, Z );
		}

		public Quaternion YZZW {
			get => new Quaternion( Y, Z, Z, W );
		}

		public Quaternion YZWX {
			get => new Quaternion( Y, Z, W, X ); set {
				Y = value.X;
				Z = value.Y;
				W = value.Z;
				X = value.W;
			}
		}

		public Quaternion YZWY {
			get => new Quaternion( Y, Z, W, Y );
		}

		public Quaternion YZWZ {
			get => new Quaternion( Y, Z, W, Z );
		}

		public Quaternion YZWW {
			get => new Quaternion( Y, Z, W, W );
		}

		public Quaternion YWXX {
			get => new Quaternion( Y, W, X, X );
		}

		public Quaternion YWXY {
			get => new Quaternion( Y, W, X, Y );
		}

		public Quaternion YWXZ {
			get => new Quaternion( Y, W, X, Z ); set {
				Y = value.X;
				W = value.Y;
				X = value.Z;
				Z = value.W;
			}
		}

		public Quaternion YWXW {
			get => new Quaternion( Y, W, X, W );
		}

		public Quaternion YWYX {
			get => new Quaternion( Y, W, Y, X );
		}

		public Quaternion YWYY {
			get => new Quaternion( Y, W, Y, Y );
		}

		public Quaternion YWYZ {
			get => new Quaternion( Y, W, Y, Z );
		}

		public Quaternion YWYW {
			get => new Quaternion( Y, W, Y, W );
		}

		public Quaternion YWZX {
			get => new Quaternion( Y, W, Z, X ); set {
				Y = value.X;
				W = value.Y;
				Z = value.Z;
				X = value.W;
			}
		}

		public Quaternion YWZY {
			get => new Quaternion( Y, W, Z, Y );
		}

		public Quaternion YWZZ {
			get => new Quaternion( Y, W, Z, Z );
		}

		public Quaternion YWZW {
			get => new Quaternion( Y, W, Z, W );
		}

		public Quaternion YWWX {
			get => new Quaternion( Y, W, W, X );
		}

		public Quaternion YWWY {
			get => new Quaternion( Y, W, W, Y );
		}

		public Quaternion YWWZ {
			get => new Quaternion( Y, W, W, Z );
		}

		public Quaternion YWWW {
			get => new Quaternion( Y, W, W, W );
		}

		public Quaternion ZXXX {
			get => new Quaternion( Z, X, X, X );
		}

		public Quaternion ZXXY {
			get => new Quaternion( Z, X, X, Y );
		}

		public Quaternion ZXXZ {
			get => new Quaternion( Z, X, X, Z );
		}

		public Quaternion ZXXW {
			get => new Quaternion( Z, X, X, W );
		}

		public Quaternion ZXYX {
			get => new Quaternion( Z, X, Y, X );
		}

		public Quaternion ZXYY {
			get => new Quaternion( Z, X, Y, Y );
		}

		public Quaternion ZXYZ {
			get => new Quaternion( Z, X, Y, Z );
		}

		public Quaternion ZXYW {
			get => new Quaternion( Z, X, Y, W ); set {
				Z = value.X;
				X = value.Y;
				Y = value.Z;
				W = value.W;
			}
		}

		public Quaternion ZXZX {
			get => new Quaternion( Z, X, Z, X );
		}

		public Quaternion ZXZY {
			get => new Quaternion( Z, X, Z, Y );
		}

		public Quaternion ZXZZ {
			get => new Quaternion( Z, X, Z, Z );
		}

		public Quaternion ZXZW {
			get => new Quaternion( Z, X, Z, W );
		}

		public Quaternion ZXWX {
			get => new Quaternion( Z, X, W, X );
		}

		public Quaternion ZXWY {
			get => new Quaternion( Z, X, W, Y ); set {
				Z = value.X;
				X = value.Y;
				W = value.Z;
				Y = value.W;
			}
		}

		public Quaternion ZXWZ {
			get => new Quaternion( Z, X, W, Z );
		}

		public Quaternion ZXWW {
			get => new Quaternion( Z, X, W, W );
		}

		public Quaternion ZYXX {
			get => new Quaternion( Z, Y, X, X );
		}

		public Quaternion ZYXY {
			get => new Quaternion( Z, Y, X, Y );
		}

		public Quaternion ZYXZ {
			get => new Quaternion( Z, Y, X, Z );
		}

		public Quaternion ZYXW {
			get => new Quaternion( Z, Y, X, W ); set {
				Z = value.X;
				Y = value.Y;
				X = value.Z;
				W = value.W;
			}
		}

		public Quaternion ZYYX {
			get => new Quaternion( Z, Y, Y, X );
		}

		public Quaternion ZYYY {
			get => new Quaternion( Z, Y, Y, Y );
		}

		public Quaternion ZYYZ {
			get => new Quaternion( Z, Y, Y, Z );
		}

		public Quaternion ZYYW {
			get => new Quaternion( Z, Y, Y, W );
		}

		public Quaternion ZYZX {
			get => new Quaternion( Z, Y, Z, X );
		}

		public Quaternion ZYZY {
			get => new Quaternion( Z, Y, Z, Y );
		}

		public Quaternion ZYZZ {
			get => new Quaternion( Z, Y, Z, Z );
		}

		public Quaternion ZYZW {
			get => new Quaternion( Z, Y, Z, W );
		}

		public Quaternion ZYWX {
			get => new Quaternion( Z, Y, W, X ); set {
				Z = value.X;
				Y = value.Y;
				W = value.Z;
				X = value.W;
			}
		}

		public Quaternion ZYWY {
			get => new Quaternion( Z, Y, W, Y );
		}

		public Quaternion ZYWZ {
			get => new Quaternion( Z, Y, W, Z );
		}

		public Quaternion ZYWW {
			get => new Quaternion( Z, Y, W, W );
		}

		public Quaternion ZZXX {
			get => new Quaternion( Z, Z, X, X );
		}

		public Quaternion ZZXY {
			get => new Quaternion( Z, Z, X, Y );
		}

		public Quaternion ZZXZ {
			get => new Quaternion( Z, Z, X, Z );
		}

		public Quaternion ZZXW {
			get => new Quaternion( Z, Z, X, W );
		}

		public Quaternion ZZYX {
			get => new Quaternion( Z, Z, Y, X );
		}

		public Quaternion ZZYY {
			get => new Quaternion( Z, Z, Y, Y );
		}

		public Quaternion ZZYZ {
			get => new Quaternion( Z, Z, Y, Z );
		}

		public Quaternion ZZYW {
			get => new Quaternion( Z, Z, Y, W );
		}

		public Quaternion ZZZX {
			get => new Quaternion( Z, Z, Z, X );
		}

		public Quaternion ZZZY {
			get => new Quaternion( Z, Z, Z, Y );
		}

		public Quaternion ZZZZ {
			get => new Quaternion( Z, Z, Z, Z );
		}

		public Quaternion ZZZW {
			get => new Quaternion( Z, Z, Z, W );
		}

		public Quaternion ZZWX {
			get => new Quaternion( Z, Z, W, X );
		}

		public Quaternion ZZWY {
			get => new Quaternion( Z, Z, W, Y );
		}

		public Quaternion ZZWZ {
			get => new Quaternion( Z, Z, W, Z );
		}

		public Quaternion ZZWW {
			get => new Quaternion( Z, Z, W, W );
		}

		public Quaternion ZWXX {
			get => new Quaternion( Z, W, X, X );
		}

		public Quaternion ZWXY {
			get => new Quaternion( Z, W, X, Y ); set {
				Z = value.X;
				W = value.Y;
				X = value.Z;
				Y = value.W;
			}
		}

		public Quaternion ZWXZ {
			get => new Quaternion( Z, W, X, Z );
		}

		public Quaternion ZWXW {
			get => new Quaternion( Z, W, X, W );
		}

		public Quaternion ZWYX {
			get => new Quaternion( Z, W, Y, X ); set {
				Z = value.X;
				W = value.Y;
				Y = value.Z;
				X = value.W;
			}
		}

		public Quaternion ZWYY {
			get => new Quaternion( Z, W, Y, Y );
		}

		public Quaternion ZWYZ {
			get => new Quaternion( Z, W, Y, Z );
		}

		public Quaternion ZWYW {
			get => new Quaternion( Z, W, Y, W );
		}

		public Quaternion ZWZX {
			get => new Quaternion( Z, W, Z, X );
		}

		public Quaternion ZWZY {
			get => new Quaternion( Z, W, Z, Y );
		}

		public Quaternion ZWZZ {
			get => new Quaternion( Z, W, Z, Z );
		}

		public Quaternion ZWZW {
			get => new Quaternion( Z, W, Z, W );
		}

		public Quaternion ZWWX {
			get => new Quaternion( Z, W, W, X );
		}

		public Quaternion ZWWY {
			get => new Quaternion( Z, W, W, Y );
		}

		public Quaternion ZWWZ {
			get => new Quaternion( Z, W, W, Z );
		}

		public Quaternion ZWWW {
			get => new Quaternion( Z, W, W, W );
		}

		public Quaternion WXXX {
			get => new Quaternion( W, X, X, X );
		}

		public Quaternion WXXY {
			get => new Quaternion( W, X, X, Y );
		}

		public Quaternion WXXZ {
			get => new Quaternion( W, X, X, Z );
		}

		public Quaternion WXXW {
			get => new Quaternion( W, X, X, W );
		}

		public Quaternion WXYX {
			get => new Quaternion( W, X, Y, X );
		}

		public Quaternion WXYY {
			get => new Quaternion( W, X, Y, Y );
		}

		public Quaternion WXYZ {
			get => new Quaternion( W, X, Y, Z ); set {
				W = value.X;
				X = value.Y;
				Y = value.Z;
				Z = value.W;
			}
		}

		public Quaternion WXYW {
			get => new Quaternion( W, X, Y, W );
		}

		public Quaternion WXZX {
			get => new Quaternion( W, X, Z, X );
		}

		public Quaternion WXZY {
			get => new Quaternion( W, X, Z, Y ); set {
				W = value.X;
				X = value.Y;
				Z = value.Z;
				Y = value.W;
			}
		}

		public Quaternion WXZZ {
			get => new Quaternion( W, X, Z, Z );
		}

		public Quaternion WXZW {
			get => new Quaternion( W, X, Z, W );
		}

		public Quaternion WXWX {
			get => new Quaternion( W, X, W, X );
		}

		public Quaternion WXWY {
			get => new Quaternion( W, X, W, Y );
		}

		public Quaternion WXWZ {
			get => new Quaternion( W, X, W, Z );
		}

		public Quaternion WXWW {
			get => new Quaternion( W, X, W, W );
		}

		public Quaternion WYXX {
			get => new Quaternion( W, Y, X, X );
		}

		public Quaternion WYXY {
			get => new Quaternion( W, Y, X, Y );
		}

		public Quaternion WYXZ {
			get => new Quaternion( W, Y, X, Z ); set {
				W = value.X;
				Y = value.Y;
				X = value.Z;
				Z = value.W;
			}
		}

		public Quaternion WYXW {
			get => new Quaternion( W, Y, X, W );
		}

		public Quaternion WYYX {
			get => new Quaternion( W, Y, Y, X );
		}

		public Quaternion WYYY {
			get => new Quaternion( W, Y, Y, Y );
		}

		public Quaternion WYYZ {
			get => new Quaternion( W, Y, Y, Z );
		}

		public Quaternion WYYW {
			get => new Quaternion( W, Y, Y, W );
		}

		public Quaternion WYZX {
			get => new Quaternion( W, Y, Z, X ); set {
				W = value.X;
				Y = value.Y;
				Z = value.Z;
				X = value.W;
			}
		}

		public Quaternion WYZY {
			get => new Quaternion( W, Y, Z, Y );
		}

		public Quaternion WYZZ {
			get => new Quaternion( W, Y, Z, Z );
		}

		public Quaternion WYZW {
			get => new Quaternion( W, Y, Z, W );
		}

		public Quaternion WYWX {
			get => new Quaternion( W, Y, W, X );
		}

		public Quaternion WYWY {
			get => new Quaternion( W, Y, W, Y );
		}

		public Quaternion WYWZ {
			get => new Quaternion( W, Y, W, Z );
		}

		public Quaternion WYWW {
			get => new Quaternion( W, Y, W, W );
		}

		public Quaternion WZXX {
			get => new Quaternion( W, Z, X, X );
		}

		public Quaternion WZXY {
			get => new Quaternion( W, Z, X, Y ); set {
				W = value.X;
				Z = value.Y;
				X = value.Z;
				Y = value.W;
			}
		}

		public Quaternion WZXZ {
			get => new Quaternion( W, Z, X, Z );
		}

		public Quaternion WZXW {
			get => new Quaternion( W, Z, X, W );
		}

		public Quaternion WZYX {
			get => new Quaternion( W, Z, Y, X ); set {
				W = value.X;
				Z = value.Y;
				Y = value.Z;
				X = value.W;
			}
		}

		public Quaternion WZYY {
			get => new Quaternion( W, Z, Y, Y );
		}

		public Quaternion WZYZ {
			get => new Quaternion( W, Z, Y, Z );
		}

		public Quaternion WZYW {
			get => new Quaternion( W, Z, Y, W );
		}

		public Quaternion WZZX {
			get => new Quaternion( W, Z, Z, X );
		}

		public Quaternion WZZY {
			get => new Quaternion( W, Z, Z, Y );
		}

		public Quaternion WZZZ {
			get => new Quaternion( W, Z, Z, Z );
		}

		public Quaternion WZZW {
			get => new Quaternion( W, Z, Z, W );
		}

		public Quaternion WZWX {
			get => new Quaternion( W, Z, W, X );
		}

		public Quaternion WZWY {
			get => new Quaternion( W, Z, W, Y );
		}

		public Quaternion WZWZ {
			get => new Quaternion( W, Z, W, Z );
		}

		public Quaternion WZWW {
			get => new Quaternion( W, Z, W, W );
		}

		public Quaternion WWXX {
			get => new Quaternion( W, W, X, X );
		}

		public Quaternion WWXY {
			get => new Quaternion( W, W, X, Y );
		}

		public Quaternion WWXZ {
			get => new Quaternion( W, W, X, Z );
		}

		public Quaternion WWXW {
			get => new Quaternion( W, W, X, W );
		}

		public Quaternion WWYX {
			get => new Quaternion( W, W, Y, X );
		}

		public Quaternion WWYY {
			get => new Quaternion( W, W, Y, Y );
		}

		public Quaternion WWYZ {
			get => new Quaternion( W, W, Y, Z );
		}

		public Quaternion WWYW {
			get => new Quaternion( W, W, Y, W );
		}

		public Quaternion WWZX {
			get => new Quaternion( W, W, Z, X );
		}

		public Quaternion WWZY {
			get => new Quaternion( W, W, Z, Y );
		}

		public Quaternion WWZZ {
			get => new Quaternion( W, W, Z, Z );
		}

		public Quaternion WWZW {
			get => new Quaternion( W, W, Z, W );
		}

		public Quaternion WWWX {
			get => new Quaternion( W, W, W, X );
		}

		public Quaternion WWWY {
			get => new Quaternion( W, W, W, Y );
		}

		public Quaternion WWWZ {
			get => new Quaternion( W, W, W, Z );
		}

		public Quaternion WWWW {
			get => new Quaternion( W, W, W, W );
		}
		#endregion

		#region Operators
		public static Quaternion operator +( Quaternion a, Quaternion b ) {
			return new Quaternion( a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W );
		}

		public static Quaternion operator -( Quaternion a ) {
			return new Quaternion( -a.X, -a.Y, -a.Z, -a.W );
		}

		public static Quaternion operator *( Quaternion a, float scalar ) {
			return new Quaternion( a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar );
		}

		public static Quaternion operator *( float scalar, Quaternion a ) {
			return new Quaternion( a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar );
		}

		public static Quaternion operator *( Quaternion a, Quaternion b ) {
			return Multiply( ref a, ref b );
		}

		public static Quaternion operator -( Quaternion a, Quaternion b ) {
			return a + -b;
		}

		public static Quaternion operator /( Quaternion a, float divident ) {
			return a * ( 1f / divident );
		}

		public static bool operator ==( Quaternion a, Quaternion b ) {
			return a.Equals( b );
		}

		public static bool operator !=( Quaternion a, Quaternion b ) {
			return !a.Equals( b );
		}

		#region Implicit
		public static implicit operator Quaternion( float[] a ) {
			if( a.Length != 4 )
				throw new System.ArgumentException( "Length of array is not compatible with Quaternion" );
			return new Quaternion( a[ 0 ], a[ 1 ], a[ 2 ], a[ 3 ] );
		}

		public static implicit operator float[]( Quaternion a ) {
			return new float[] { a.X, a.Y, a.Z, a.W };
		}
		#endregion
		#endregion

	}
}
