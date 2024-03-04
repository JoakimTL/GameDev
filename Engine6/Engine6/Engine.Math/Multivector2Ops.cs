using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Math;

namespace Engine.Math;

public static class Multivector2Ops {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Negate<T>( in this Multivector2<T> l ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.Negate( l );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Add<T>( in this Multivector2<T> l, in Multivector2<T> r ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.Add( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Subtract<T>( in this Multivector2<T> l, in Multivector2<T> r ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.Subtract( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply<T>( in this Multivector2<T> l, in T r ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Divide<T>( in this Multivector2<T> l, in T r ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.Divide( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> MultiplyEntrywise<T>( in this Multivector2<T> l, in Multivector2<T> r ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.MultiplyEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> DivideEntrywise<T>( in this Multivector2<T> l, in Multivector2<T> r ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.DivideEntrywise( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply<T>( in this Multivector2<T> l, in Vector2<T> r ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply<T>( in this Multivector2<T> l, in Bivector2<T> r ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Multivector2<T> Multiply<T>( in this Multivector2<T> l, in Multivector2<T> r ) where T : unmanaged, INumberBase<T> => Multivector2Math<T>.Multiply( l, r );
}
