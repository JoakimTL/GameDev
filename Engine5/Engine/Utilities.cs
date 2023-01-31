using System.Runtime.CompilerServices;

namespace Engine;

public static unsafe class Extensions {
	public static T NotNull<T>( this T? val ) => val ?? throw new NullReferenceException( $"{typeof( T ).Name} was null!" );
	public static float ToFloat( this Int128 value, float fraction ) {
		long longValue = (long) value;
		if ( longValue > int.MaxValue )
			return float.PositiveInfinity;
		if ( longValue < int.MinValue )
			return float.NegativeInfinity;
		int intValue = (int) longValue;
		return intValue * fraction;
	}

	public static uint ToUint<T>( this T src ) where T : unmanaged
		=> Convert<T, uint>( src );

	public static int ToInt<T>( this T src ) where T : unmanaged
		=> Convert<T, int>( src );

	public static ulong ToUlong<T>( this T src ) where T : unmanaged
		=> Convert<T, ulong>( src );

	public static long ToLong<T>( this T src ) where T : unmanaged
		=> Convert<T, long>( src );

	public static TDestination Convert<TSource, TDestination>( this TSource src ) where TSource : unmanaged where TDestination : unmanaged {
		if ( src is TDestination dst )
			return dst;
		uint len = (uint) Math.Min( sizeof( TSource ), sizeof( TDestination ) );
		Unsafe.CopyBlock( &dst, &src, len );
		return dst;
	}
}
