using System.Numerics;

namespace Engine;
public static class NumericsExtensions {
	public static Vector3 Forward( this Quaternion q ) => Vector3.Transform( -Vector3.UnitZ, q );

	public static Vector3 Backward( this Quaternion q ) => Vector3.Transform( Vector3.UnitZ, q );

	public static Vector3 Left( this Quaternion q ) => Vector3.Transform( -Vector3.UnitX, q );

	public static Vector3 Right( this Quaternion q ) => Vector3.Transform( Vector3.UnitX, q );

	public static Vector3 Down( this Quaternion q ) => Vector3.Transform( -Vector3.UnitY, q );

	public static Vector3 Up( this Quaternion q ) => Vector3.Transform( Vector3.UnitY, q );

	public static Vector3 Cubify( this Vector3 v ) => v * Math.Abs( Math.Min( Math.Min( 1f / v.X, 1f / v.Y ), 1f / v.Z ) );

	public static Quaternion DirectionVectorToQuaternion( this Vector3 v ) => Quaternion.CreateFromYawPitchRoll( MathF.Atan2( -v.X, -v.Z ), MathF.Asin( v.Y ), 0 );
}
