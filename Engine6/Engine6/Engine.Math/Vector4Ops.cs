using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Math;

namespace Engine.Math;

/// <summary>
/// Extension methods for <see cref="Vector4{T}"/>. Return types may vary.
/// </summary>
public static class Vector4Ops {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Negate<T>( in this Vector4<T> l ) where T : unmanaged, INumberBase<T> => Vector4Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Add<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumberBase<T> => Vector4Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Subtract<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumberBase<T> => Vector4Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> ScalarMultiply<T>( in this Vector4<T> l, T r ) where T : unmanaged, INumberBase<T> => Vector4Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> ScalarDivide<T>( in this Vector4<T> l, T r ) where T : unmanaged, INumberBase<T> => Vector4Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> MultiplyEntrywise<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumberBase<T> => Vector4Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> DivideEntrywise<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumberBase<T> => Vector4Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector4<T> Wedge<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumberBase<T> => Bivector4Math<T>.Wedge( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumberBase<T> => ScalarMath<T>.Dot( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T MagnitudeSquared<T>( in this Vector4<T> l ) where T : unmanaged, INumberBase<T> => l.Dot( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Magnitude<T>( in this Vector4<T> l ) where T : unmanaged, IFloatingPointIeee754<T> => T.Sqrt( l.MagnitudeSquared() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Normalize<T>( in this Vector4<T> l ) where T : unmanaged, IFloatingPointIeee754<T> => l.ScalarDivide( l.Magnitude() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Inverse<T>( in this Vector4<T> l ) where T : unmanaged, INumberBase<T> => l.ScalarDivide( l.MagnitudeSquared() );

}
