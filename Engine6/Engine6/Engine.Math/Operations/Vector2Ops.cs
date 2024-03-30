using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation;

namespace Engine.Math.Operations;

/// <summary>
/// Extension methods for <see cref="Vector2{T}"/>. Return types may vary.
/// </summary>
public static class Vector2Ops {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Negate<T>( in this Vector2<T> l ) where T : unmanaged, INumber<T> => Vector2Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Add<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumber<T> => Vector2Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Subtract<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumber<T> => Vector2Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> ScalarMultiply<T>( in this Vector2<T> l, T r ) where T : unmanaged, INumber<T> => Vector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> ScalarDivide<T>( in this Vector2<T> l, T r ) where T : unmanaged, INumber<T> => Vector2Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> MultiplyEntrywise<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumber<T> => Vector2Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> DivideEntrywise<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumber<T> => Vector2Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Wedge<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumber<T> => Bivector2Math<T>.Wedge( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumber<T> => ScalarMath<T>.Dot( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotor2<T> Multiply<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumber<T> => Rotor2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T MagnitudeSquared<T>( in this Vector2<T> l ) where T : unmanaged, INumber<T> => l.Dot( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Magnitude<T>( in this Vector2<T> l ) where T : unmanaged, IFloatingPointIeee754<T> => T.Sqrt( l.MagnitudeSquared() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Normalize<T>( in this Vector2<T> l ) where T : unmanaged, IFloatingPointIeee754<T> => l.ScalarDivide( l.Magnitude() );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Inverse<T>( in this Vector2<T> l ) where T : unmanaged, INumber<T> => l.ScalarDivide( l.MagnitudeSquared() );

	//[MethodImpl( MethodImplOptions.AggressiveInlining )]
	//public static Vector2<T> WorldTransform<T>( in this Vector2<T> v, in Matrix4x4<T> matrix ) where T : unmanaged, INumber<T> => ( v.WorldTransformVector * matrix ).GetTransformedVector2();

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Multiply<T>( in this Vector2<T> l, in Bivector2<T> r ) where T : unmanaged, INumber<T> => Vector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Floor<T>( in this Vector2<T> l ) where T : unmanaged, IFloatingPointIeee754<T>
		=> new( T.Floor( l.X ), T.Floor( l.Y ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Ceiling<T>( in this Vector2<T> l ) where T : unmanaged, IFloatingPointIeee754<T>
		=> new( T.Ceiling( l.X ), T.Ceiling( l.Y ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Round<T>( in this Vector2<T> l, int digits, MidpointRounding roundingMode ) where T : unmanaged, IFloatingPointIeee754<T>
		=> new( T.Round( l.X, digits, roundingMode ), T.Round( l.Y, digits, roundingMode ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Max<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumber<T>
		=> new( T.Max( l.X, r.X ), T.Max( l.Y, r.Y ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> Min<T>( in this Vector2<T> l, in Vector2<T> r ) where T : unmanaged, INumber<T>
		=> new( T.Min( l.X, r.X ), T.Min( l.Y, r.Y ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T SumOfParts<T>( in this Vector2<T> l ) where T : unmanaged, INumber<T>
		=> l.X + l.Y;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T ProductOfParts<T>( in this Vector2<T> l ) where T : unmanaged, INumber<T>
		=> l.X * l.Y;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool Inside<T>( in this Vector2<T> l, in AABB2<T> r ) where T : unmanaged, INumber<T>
		=> r.Minima.X <= l.X && r.Maxima.X >= l.X && r.Minima.Y <= l.Y && r.Maxima.Y >= l.Y;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool IsNegativeOrZero<T>( in this Vector2<T> l ) where T : unmanaged, INumber<T>
		=> l.X <= T.Zero || l.Y <= T.Zero;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> ReflectNormal<T>( in this Vector2<T> v, in Vector2<T> normal ) where T : unmanaged, INumber<T>
		=> normal.Multiply( v ).Multiply( normal );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<T> ReflectMirror<T>( in this Vector2<T> v, in Vector2<T> mirrorNormal ) where T : unmanaged, INumber<T>
		=> -mirrorNormal.Multiply( v ).Multiply( mirrorNormal );

	public static Vector2<T> NormalizeWithinSpace<T>( in this Vector2<T> l, T min, T max ) where T : unmanaged, INumber<T> {
		T spaceInverse = T.MultiplicativeIdentity / (max - min);
		if (spaceInverse == T.Zero)
			throw new ArgumentException( $"{nameof( min )} - {nameof( max )} is equal to 0, meaning the space can't be normalized." );
		return new( (l.X - min) * spaceInverse, (l.Y - min) * spaceInverse );
	}
}
