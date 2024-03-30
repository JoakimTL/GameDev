using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation.Interfaces;

namespace Engine.Math.Calculation;

/// <summary>
/// All methods that return <see cref="Bivector2{T}"/> are implemented here."/>
/// </summary>
public sealed class Bivector2Math<T> :
		ILinearMath<Bivector2<T>, T>,
		IOuterProduct<Vector2<T>, Vector2<T>, Bivector2<T>>
	where T :
		unmanaged, INumber<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Negate( in Bivector2<T> l ) => new( -l.XY );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Add( in Bivector2<T> l, in Bivector2<T> r ) => new( l.XY + r.XY );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Subtract( in Bivector2<T> l, in Bivector2<T> r ) => new( l.XY - r.XY );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Multiply( in Bivector2<T> l, T r ) => new( l.XY * r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Divide( in Bivector2<T> l, T r ) => new( l.XY / r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Bivector2<T> Wedge( in Vector2<T> l, in Vector2<T> r ) => new( l.X * r.Y - l.Y * r.X );

}
