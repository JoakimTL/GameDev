using Engine.Data;
using Engine.GlobalServices;
using Engine.Structure;
using Engine.Structure.Interfaces;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

	/// <summary>
	/// Serialized the serializable object. If data is not null, then the return and "data" will be the same region memory.
	/// </summary>
	/// <param name="serializable">Object to serialize</param>
	/// <param name="data"></param>
	/// <returns></returns>
	public static byte[] Serialize( this ISerializable serializable ) {
		byte[] serializedData = serializable.SerializeData();
		byte[] returnData = new byte[ serializedData.Length + sizeof( Guid ) ];
		fixed ( byte* dstPtr = returnData )
		fixed ( byte* srcPtr = serializedData ) {
			*(Guid*) dstPtr = Global.Get<SerializableService>().GetFromType( serializable.GetType() );
			Buffer.MemoryCopy( srcPtr, dstPtr + sizeof( Guid ), returnData.Length, serializedData.Length );
		}
		return returnData;
	}

	public static Type? GetDeserializedType( this byte[] data ) {
		if ( data.Length < sizeof( Guid ) )
			return Log.WarningThenReturnDefault<Type?>( $"Length of data too short for {nameof( Guid )}" );
		Type? type = Global.Get<SerializableService>().GetFromIdentity( data.ToUnmanagedOrDefault<Guid>() );
		if ( type is null )
			return Log.WarningThenReturnDefault<Type?>( "Couldn't find type for data" );
		return type;
	}

	public static object? Deserialize( this byte[] serializedData, IDependencyInjector? injector = null ) {
		if ( serializedData.Length <= sizeof( Guid ) )
			return null;
		object? obj;
		Type? type = GetDeserializedType( serializedData );
		if ( type is null )
			return null;
		obj = type.GetInjectedInstance( injector );
		if ( obj is null )
			return Log.WarningThenReturnDefault<object?>( "Construction of type failed" );
		if ( obj is not ISerializable serializable )
			return Log.WarningThenReturnDefault<object?>( "Constructed type was not serializable" );
		if ( !serializable.DeserializeData( serializedData[ sizeof( Guid ).. ] ) )
			return Log.WarningThenReturnDefault<object?>( "Deserializing the serializable's data failed" );
		return serializable;
	}

	/// <summary>
	/// Gets an instance of the type. This may or may not construct a new instance, this all depends on the injector used.
	/// </summary>
	/// <param name="type">The types to get an instance of</param>
	/// <param name="injector">The injector to use. <see cref="TransientInjector.Singleton"/> will be used if this is null.</param>
	/// <returns></returns>
	public static object? GetInjectedInstance( this Type type, IDependencyInjector? injector = null )
		=> ( injector ?? TransientInjector.Singleton ).Get( type );
}
