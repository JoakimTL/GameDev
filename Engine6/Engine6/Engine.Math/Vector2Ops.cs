using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Math;

namespace Engine.Math;

/// <summary>
/// Extension methods for <see cref="Vector2{T}"/>. Return types may vary.
/// </summary>
public static class Vector2Ops {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Negate<T>( in this Vector2<T> l ) where T : unmanaged, INumberBase<T> => Vector2Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Add<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumberBase<T> => Vector2Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Subtract<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumberBase<T> => Vector2Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> ScalarMultiply<T>( in this Vector2<T> l, T r ) where T : unmanaged, INumberBase<T> => Vector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> ScalarDivide<T>( in this Vector2<T> l, T r ) where T : unmanaged, INumberBase<T> => Vector2Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> MultiplyEntrywise<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumberBase<T> => Vector2Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> DivideEntrywise<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumberBase<T> => Vector2Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Wedge<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumberBase<T> => Bivector2Math<T>.Wedge( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumberBase<T> => ScalarMath<T>.Dot( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T MagnitudeSquared<T>( in this Vector2<T> l ) where T : unmanaged, INumberBase<T> => l.Dot( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Magnitude<T>( in this Vector2<T> l ) where T : unmanaged, IFloatingPointIeee754<T> => T.Sqrt( l.MagnitudeSquared() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Normalize<T>( in this Vector2<T> l ) where T : unmanaged, IFloatingPointIeee754<T> => l.ScalarDivide( l.Magnitude() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Inverse<T>( in this Vector2<T> l ) where T : unmanaged, INumberBase<T> => l.ScalarDivide( l.MagnitudeSquared() );

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static Vector2<T> WorldTransform<T>( in this Vector2<T> v, in Matrix4x4<T> matrix ) where T : unmanaged, INumberBase<T> => ( v.WorldTransformVector * matrix ).GetTransformedVector2();

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Multiply<T>( in this Vector2<T> l, in Bivector2<T> r ) where T : unmanaged, INumberBase<T> => Vector2Math<T>.Multiply( l, r );

}
