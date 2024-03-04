using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Math;

/// <summary>
/// All methods that return <see cref="{T}"/> are implemented here."/>
/// </summary>
public sealed class ScalarMath<T> :
		IDotProduct<Vector2<T>, T>,
		IDotProduct<Vector3<T>, T>,
		IDotProduct<Vector4<T>, T>,
		IGeometricProduct<Bivector2<T>, Bivector2<T>, T>,
		IGeometricProduct<Trivector3<T>, Trivector3<T>, T>,
		IGeometricProduct<Quadvector4<T>, Quadvector4<T>, T>
	where T : 
		unmanaged, INumberBase<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Vector2<T> l, in Vector2<T> r ) => l.X * r.X + l.Y * r.Y;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Multiply( in Bivector2<T> l, in Bivector2<T> r ) => -(l.XY * r.XY);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Vector3<T> l, in Vector3<T> r ) => l.X * r.X + l.Y * r.Y + l.Z * r.Z;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Multiply( in Trivector3<T> l, in Trivector3<T> r ) => -(l.XYZ * r.XYZ);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Vector4<T> l, in Vector4<T> r ) => l.X * r.X + l.Y * r.Y + l.Z * r.Z + l.W * r.W;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Multiply( in Quadvector4<T> l, in Quadvector4<T> r ) => l.XYZW * r.XYZW;

}
