using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.Math.Operations;

public static class Vector3Casts {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<TOut> CastChecked<TIn, TOut>( in this Vector3<TIn> l ) where TIn : unmanaged, INumber<TIn> where TOut : unmanaged, INumber<TOut>
		=> new( TOut.CreateChecked( l.X ), TOut.CreateChecked( l.Y ), TOut.CreateChecked( l.Z ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<TOut> CastTruncating<TIn, TOut>( in this Vector3<TIn> l ) where TIn : unmanaged, INumber<TIn> where TOut : unmanaged, INumber<TOut>
		=> new( TOut.CreateTruncating( l.X ), TOut.CreateTruncating( l.Y ), TOut.CreateTruncating( l.Z ) );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3<TOut> CastSaturating<TIn, TOut>( in this Vector3<TIn> l ) where TIn : unmanaged, INumber<TIn> where TOut : unmanaged, INumber<TOut>
		=> new( TOut.CreateSaturating( l.X ), TOut.CreateSaturating( l.Y ), TOut.CreateSaturating( l.Z ) );
}