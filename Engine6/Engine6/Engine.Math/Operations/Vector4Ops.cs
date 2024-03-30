using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation;

namespace Engine.Math.Operations;

/// <summary>
/// Extension methods for <see cref="Vector4{T}"/>. Return types may vary.
/// </summary>
public static class Vector4Ops {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Negate<T>( in this Vector4<T> l ) where T : unmanaged, INumber<T> => Vector4Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Add<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumber<T> => Vector4Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Subtract<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumber<T> => Vector4Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> ScalarMultiply<T>( in this Vector4<T> l, T r ) where T : unmanaged, INumber<T> => Vector4Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> ScalarDivide<T>( in this Vector4<T> l, T r ) where T : unmanaged, INumber<T> => Vector4Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> MultiplyEntrywise<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumber<T> => Vector4Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> DivideEntrywise<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumber<T> => Vector4Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector4<T> Wedge<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumber<T> => Bivector4Math<T>.Wedge( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumber<T> => ScalarMath<T>.Dot( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T MagnitudeSquared<T>( in this Vector4<T> l ) where T : unmanaged, INumber<T> => l.Dot( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Magnitude<T>( in this Vector4<T> l ) where T : unmanaged, IFloatingPointIeee754<T> => T.Sqrt( l.MagnitudeSquared() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Normalize<T>( in this Vector4<T> l ) where T : unmanaged, IFloatingPointIeee754<T> => l.ScalarDivide( l.Magnitude() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Inverse<T>( in this Vector4<T> l ) where T : unmanaged, INumber<T> => l.ScalarDivide( l.MagnitudeSquared() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Floor<T>( in this Vector4<T> l ) where T : unmanaged, IFloatingPointIeee754<T>
		=> new( T.Floor( l.X ), T.Floor( l.Y ), T.Floor( l.Z ), T.Floor( l.W ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Ceiling<T>( in this Vector4<T> l ) where T : unmanaged, IFloatingPointIeee754<T>
		=> new( T.Ceiling( l.X ), T.Ceiling( l.Y ), T.Ceiling( l.Z ), T.Ceiling( l.W ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Round<T>( in this Vector4<T> l, int digits, MidpointRounding roundingMode ) where T : unmanaged, IFloatingPointIeee754<T>
		=> new( T.Round( l.X, digits, roundingMode ), T.Round( l.Y, digits, roundingMode ), T.Round( l.Z, digits, roundingMode ), T.Round( l.W, digits, roundingMode ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Max<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumber<T>
		=> new( T.Max( l.X, r.X ), T.Max( l.Y, r.Y ), T.Max( l.Z, r.Z ), T.Max( l.W, r.W ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector4<T> Min<T>( in this Vector4<T> l, in Vector4<T> r ) where T : unmanaged, INumber<T>
		=> new( T.Min( l.X, r.X ), T.Min( l.Y, r.Y ), T.Min( l.Z, r.Z ), T.Min( l.W, r.W ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T SumOfParts<T>( in this Vector4<T> l ) where T : unmanaged, INumber<T>
		=> l.X + l.Y + l.Z + l.W;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T ProductOfParts<T>( in this Vector4<T> l ) where T : unmanaged, INumber<T>
		=> l.X * l.Y * l.Z * l.W;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool Inside<T>( in this Vector4<T> l, in AABB4<T> r ) where T : unmanaged, INumber<T>
		=> r.Minima.X <= l.X && r.Maxima.X >= l.X
		&& r.Minima.Y <= l.Y && r.Maxima.Y >= l.Y
		&& r.Minima.Z <= l.Z && r.Maxima.Z >= l.Z
		&& r.Minima.W <= l.W && r.Maxima.W >= l.W;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool IsNegativeOrZero<T>( in this Vector4<T> l ) where T : unmanaged, INumber<T>
		=> l.X <= T.Zero || l.Y <= T.Zero || l.Z <= T.Zero || l.W <= T.Zero;

	public static Vector4<T> NormalizeWithinSpace<T>( in this Vector4<T> l, T min, T max ) where T : unmanaged, INumber<T> {
		T spaceInverse = T.MultiplicativeIdentity / (max - min);
		if (spaceInverse == T.Zero)
			throw new ArgumentException( $"{nameof( min )} - {nameof( max )} is equal to 0, meaning the space can't be normalized." );
		return new( (l.X - min) * spaceInverse, (l.Y - min) * spaceInverse, (l.Z - min) * spaceInverse, (l.W - min) * spaceInverse );
	}
}
