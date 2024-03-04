using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Math;

namespace Engine.Math;

/// <summary>
/// Extension methods for <see cref="Vector3{T}"/>. Return types may vary.
/// </summary>
public static class Vector3Ops {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<T> Negate<T>( in this Vector3<T> l ) where T : unmanaged, INumberBase<T> => Vector3Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<T> Add<T>( in this Vector3<T> l, in Vector3<T> r ) where T : unmanaged, INumberBase<T> => Vector3Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<T> Subtract<T>( in this Vector3<T> l, in Vector3<T> r ) where T : unmanaged, INumberBase<T> => Vector3Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<T> ScalarMultiply<T>( in this Vector3<T> l, T r ) where T : unmanaged, INumberBase<T> => Vector3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<T> ScalarDivide<T>( in this Vector3<T> l, T r ) where T : unmanaged, INumberBase<T> => Vector3Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<T> MultiplyEntrywise<T>( in this Vector3<T> l, in Vector3<T> r ) where T : unmanaged, INumberBase<T> => Vector3Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<T> DivideEntrywise<T>( in this Vector3<T> l, in Vector3<T> r ) where T : unmanaged, INumberBase<T> => Vector3Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector3<T> Multiply<T>( in this Vector3<T> l, in Bivector3<T> r ) where T : unmanaged, INumberBase<T> => Multivector3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Multiply<T>( in this Vector3<T> l, in Trivector3<T> r ) where T : unmanaged, INumberBase<T> => Bivector3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector3<T> Multiply<T>( in this Vector3<T> l, in Multivector3<T> r ) where T : unmanaged, INumberBase<T> => Multivector3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector3<T> Wedge<T>( in this Vector3<T> l, in Vector3<T> r ) where T : unmanaged, INumberBase<T> => Bivector3Math<T>.Wedge( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot<T>( in this Vector3<T> l, in Vector3<T> r ) where T : unmanaged, INumberBase<T> => ScalarMath<T>.Dot( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector3<T> Multiply<T>( in this Vector3<T> l, in Vector3<T> r ) where T : unmanaged, INumberBase<T> => Multivector3Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T MagnitudeSquared<T>( in this Vector3<T> l ) where T : unmanaged, INumberBase<T> => l.Dot( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Magnitude<T>( in this Vector3<T> l ) where T : unmanaged, IFloatingPointIeee754<T> => T.Sqrt( l.MagnitudeSquared() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<T> Normalize<T>( in this Vector3<T> l ) where T : unmanaged, IFloatingPointIeee754<T> => l.ScalarDivide( l.Magnitude() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<T> Inverse<T>( in this Vector3<T> l ) where T : unmanaged, INumberBase<T> => l.ScalarDivide( l.MagnitudeSquared() );

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static Vector3<T> WorldTransform<T>( in this Vector3<T> v, in Matrix4x4<T> matrix ) where T : unmanaged, INumberBase<T> => ( v.WorldTransformVector * matrix ).GetTransformedVector3();

}
