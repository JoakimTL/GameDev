using Engine.GlobalServices;
using Engine.Structure;
using Engine.Structure.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine;
public static unsafe class DataExtensions {

	/// <summary>
	/// Returns the number of unmanaged values are needed to represent the bit count.
	/// </summary>
	public static int BitsInUnmanagedArray<T>( this int bitCount ) where T : unmanaged {
		int bitsPerUnmanaged = Marshal.SizeOf<T>() * 8;
		(int quotient, int remainder) = Math.DivRem( bitCount, bitsPerUnmanaged );
		int unmanagedToRepresentBits = quotient;
		if ( remainder > 0 )
			++unmanagedToRepresentBits;
		return unmanagedToRepresentBits;
	}

	public static unsafe T? ToUnmanaged<T>( this byte[] data, uint offset = 0 ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		if ( data.Length < sizeBytes + offset )
			return Log.WarningThenReturnDefault<T?>( $"Unable to load {typeof( T ).Name} from data!" );
		fixed ( byte* srcPtr = data )
			return *(T*) ( srcPtr + offset );
	}

	public static unsafe T ToUnmanagedOrDefault<T>( this byte[] data, uint offset = 0 ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		if ( data.Length < sizeBytes + offset )
			return Log.WarningThenReturn( $"Unable to load {typeof( T ).Name} from data!", default( T ) );
		fixed ( byte* srcPtr = data )
			return *(T*) ( srcPtr + offset );
	}

	public static unsafe byte[] ToBytes<T>( this T data ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		byte[] returnData = new byte[ sizeBytes ];
		fixed ( byte* dstPtr = returnData )
			*(T*) dstPtr = data;
		return returnData;
	}

	public static unsafe byte[] ToBytes<T>( this Span<T> data ) where T : unmanaged {
		uint sizeBytes = (uint) ( sizeof( T ) * data.Length );
		byte[] returnData = new byte[ sizeBytes ];
		fixed ( T* srcPtr = data )
		fixed ( byte* dstPtr = returnData )
			Unsafe.CopyBlock( dstPtr, srcPtr, sizeBytes );
		return returnData;
	}

	public static unsafe string? CreateString( this byte[] data ) {
		if ( data.Length % sizeof( char ) != 0 )
			return Log.WarningThenReturnDefault<string?>( $"Data unaligned. Possible corruption!" );
		Span<char> chars;
		fixed ( byte* srcPtr = data )
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static unsafe string? CreateString( this Span<byte> data ) {
		if ( data.Length % sizeof( char ) != 0 )
			return Log.WarningThenReturnDefault<string?>( $"Data unaligned. Possible corruption!" );
		Span<char> chars;
		fixed ( byte* srcPtr = data )
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static unsafe string? CreateString( void* srcPtr, uint lengthBytes ) {
		if ( lengthBytes % sizeof( char ) != 0 )
			return Log.WarningThenReturnDefault<string?>( $"Length must be divisible by {sizeof( char )}!" );
		return new string( new Span<char>( srcPtr, (int) lengthBytes / sizeof( char ) ) );
	}

	/// <summary>
	/// Returns a byte array with the string data inside. Size per character can be found using <see cref="sizeof(char)"/>
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public static unsafe byte[] ToBytes( this string data ) {
		byte[] returnData = new byte[ data.Length * sizeof( char ) ];
		fixed ( char* srcPtr = data )
		fixed ( byte* dstPtr = returnData )
			Unsafe.CopyBlock( dstPtr, srcPtr, (uint) ( data.Length * sizeof( char ) ) );
		return returnData;
	}

	public static void CopyInto<T, M>( this T source, M[] destination, uint offsetBytes = 0 ) where T : unmanaged where M : unmanaged {
		if ( sizeof( T ) + offsetBytes > destination.Length * sizeof( M ) ) {
			Log.Warning( "Cannot copy outside the array." );
			return;
		}
		fixed ( M* dstPtr = destination )
			*(T*) ( (byte*) dstPtr + offsetBytes ) = source;
	}

	public static void CopyInto<T, M>( this ReadOnlySpan<T> source, M[] destination, uint offsetBytes = 0 ) where T : unmanaged where M : unmanaged {
		if ( source.Length * sizeof( T ) + offsetBytes > destination.Length * sizeof( M ) ) {
			Log.Warning( "Cannot copy outside the array." );
			return;
		}
		fixed ( void* dstPtr = destination )
		fixed ( void* srcPtr = source )
			Buffer.MemoryCopy( (byte*) srcPtr + offsetBytes, dstPtr, destination.Length * sizeof( M ), source.Length * sizeof( T ) - offsetBytes );
	}

	public static void CopyInto<T, M>( this T source, Span<M> destination, uint offsetBytes = 0 ) where T : unmanaged where M : unmanaged {
		if ( sizeof( T ) + offsetBytes > destination.Length * sizeof( M ) ) {
			Log.Warning( "Cannot copy outside the array." );
			return;
		}
		fixed ( M* dstPtr = destination )
			*(T*) ( (byte*) dstPtr + offsetBytes ) = source;
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
}
