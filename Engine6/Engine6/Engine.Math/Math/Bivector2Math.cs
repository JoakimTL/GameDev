using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Math;

/// <summary>
/// All methods that return <see cref="Bivector2{T}"/> are implemented here."/>
/// </summary>
public sealed class Bivector2Math<T> :
		ILinearMath<Bivector2<T>, T>,
		IWedgeProduct<Vector2<T>, Vector2<T>, Bivector2<T>>
	where T :
		unmanaged, INumberBase<T> {

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
