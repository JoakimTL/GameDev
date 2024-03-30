using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Operations;

public static class Vector2Casts {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<TOut> CastChecked<TIn, TOut>( in this Vector2<TIn> l ) where TIn : unmanaged, INumber<TIn> where TOut : unmanaged, INumber<TOut>
		=> new( TOut.CreateChecked( l.X ), TOut.CreateChecked( l.Y ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<TOut> CastTruncating<TIn, TOut>( in this Vector2<TIn> l ) where TIn : unmanaged, INumber<TIn> where TOut : unmanaged, INumber<TOut>
		=> new( TOut.CreateTruncating( l.X ), TOut.CreateTruncating( l.Y ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector2<TOut> CastSaturating<TIn, TOut>( in this Vector2<TIn> l ) where TIn : unmanaged, INumber<TIn> where TOut : unmanaged, INumber<TOut>
		=> new( TOut.CreateSaturating( l.X ), TOut.CreateSaturating( l.Y ) );

}
