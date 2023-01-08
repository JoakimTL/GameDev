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

}
