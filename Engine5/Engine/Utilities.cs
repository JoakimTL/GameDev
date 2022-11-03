namespace Engine;

public static unsafe class Extensions {
	public static T NotNull<T>( this T? val ) => val ?? throw new NullReferenceException( $"{typeof( T ).Name} was null!" );
}
