using Engine.Structure;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Engine;

public static class EnumUtilities {
	public static int GetMaxEnumValue<T>() where T : struct, Enum
		=> Enum.GetValues<T>().Cast<int>().Max();

	public static T[] MapEnum<T>() where T : struct, Enum
		=> Enum.GetValues<T>();
}

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

	/// <summary>
	/// Gets an instance of the type. This may or may not construct a new instance, this all depends on the injector used.
	/// </summary>
	/// <param name="type">The types to get an instance of</param>
	/// <param name="injector">The injector to use. <see cref="SilentTransientInjector.Singleton"/> will be used if this is null.</param>
	/// <returns></returns>
	public static object? GetInjectedInstance( this Type type, IDependencyInjector? injector = null )
		=> ( injector ?? SilentTransientInjector.Singleton ).Get( type );

	public static T? CompileStaticMethod<T>( this MethodInfo method, params Type[] parameterTypes ) where T : Delegate {
		//var callerExpression = callerType is not null ? Expression.Parameter( callerType ) : null;
		var parameterExpressions = parameterTypes.Select( Expression.Parameter ).ToArray();
		var callExpression = Expression.Call( method, parameterExpressions );
		var lambaExpression = Expression.Lambda<T>( callExpression, parameterExpressions );
		return lambaExpression.Compile();
	}
	public static Action<T> CopileStaticPropertyAccess<T>( this PropertyInfo property ) {
		var propertyGetter = property.GetMethod ?? throw new InvalidOperationException( $"Property {property} has no getter!" );
		var callExpression = Expression.Property( null, propertyGetter );
		var lambaExpression = Expression.Lambda<Action<T>>( callExpression );
		return lambaExpression.Compile();
	}
}
