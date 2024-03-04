using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Math;

namespace Engine.Math;

/// <summary>
/// Extension methods for <see cref="Bivector2{T}"/>. Return types may vary.
/// </summary>
public static class Bivector2Ops {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Negate<T>( in this Bivector2<T> l ) where T : unmanaged, INumberBase<T> => Bivector2Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Add<T>( in this Bivector2<T> l, in Bivector2<T> r ) where T : unmanaged, INumberBase<T> => Bivector2Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Subtract<T>( in this Bivector2<T> l, in Bivector2<T> r ) where T : unmanaged, INumberBase<T> => Bivector2Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> ScalarMultiply<T>( in this Bivector2<T> l, T r ) where T : unmanaged, INumberBase<T> => Bivector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> ScalarDivide<T>( in this Bivector2<T> l, T r ) where T : unmanaged, INumberBase<T> => Bivector2Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Multiply<T>( in this Bivector2<T> l, in Vector2<T> r ) where T : unmanaged, INumberBase<T> => Vector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Multiply<T>( in this Bivector2<T> l, in Bivector2<T> r ) where T : unmanaged, INumberBase<T> => ScalarMath<T>.Multiply( l, r );

}