using Engine.GlobalServices;
using Engine.Structure;
using Engine.Structure.Interfaces;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

	public static T ToUnmanaged<T>( this byte[] data, uint offset = 0 ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		if ( data.Length < sizeBytes + offset )
			return Log.WarningThenReturnDefault<T>( $"Unable to load {typeof( T ).Name} from data!" );
		fixed ( byte* srcPtr = data )
			return *(T*) ( srcPtr + offset );
	}

	public static T? ToUnmanagedNullable<T>( this byte[] data, uint offset = 0 ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		if ( data.Length < sizeBytes + offset )
			return Log.WarningThenReturnDefault<T?>( $"Unable to load {typeof( T ).Name} from data!" );
		fixed ( byte* srcPtr = data )
			return *(T*) ( srcPtr + offset );
	}

	public static T ToUnmanagedOrThrow<T>( this byte[] data, uint offset = 0 ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		if ( data.Length < sizeBytes + offset )
			throw new InvalidDataException( $"Data has length {data.Length}, but expected size was {sizeBytes + offset}!" );
		fixed ( byte* srcPtr = data )
			return *(T*) ( srcPtr + offset );
	}

	public static byte[] ToBytes<T>( this T data ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		byte[] returnData = new byte[ sizeBytes ];
		fixed ( byte* dstPtr = returnData )
			*(T*) dstPtr = data;
		return returnData;
	}

	public static byte[] ToBytes<T>( this Span<T> data ) where T : unmanaged {
		uint sizeBytes = (uint) ( sizeof( T ) * data.Length );
		byte[] returnData = new byte[ sizeBytes ];
		fixed ( T* srcPtr = data )
		fixed ( byte* dstPtr = returnData )
			Unsafe.CopyBlock( dstPtr, srcPtr, sizeBytes );
		return returnData;
	}

	public static string? CreateString( this byte[] data ) {
		if ( data.Length % sizeof( char ) != 0 )
			return Log.WarningThenReturnDefault<string?>( $"Length must be divisible by {sizeof( char )}!" );
		Span<char> chars;
		fixed ( byte* srcPtr = data )
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static string? CreateString( this Span<byte> data ) {
		if ( data.Length % sizeof( char ) != 0 )
			return Log.WarningThenReturnDefault<string?>( $"Length must be divisible by {sizeof( char )}!" );
		Span<char> chars;
		fixed ( byte* srcPtr = data )
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static string? CreateString( void* srcPtr, uint lengthBytes ) {
		if ( lengthBytes % sizeof( char ) != 0 )
			return Log.WarningThenReturnDefault<string?>( $"Length must be divisible by {sizeof( char )}!" );
		return new string( new Span<char>( srcPtr, (int) lengthBytes / sizeof( char ) ) );
	}

	public static string CreateStringOrThrow( this byte[] data ) {
		if ( data.Length % sizeof( char ) != 0 )
			throw new ArgumentException($"Length must be divisible by {sizeof( char )}!", nameof(data) );
		Span<char> chars;
		fixed ( byte* srcPtr = data )
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static string CreateStringOrThrow( this Span<byte> data ) {
		if ( data.Length % sizeof( char ) != 0 )
			throw new ArgumentException( $"Length must be divisible by {sizeof( char )}!", nameof( data ) );
		Span<char> chars;
		fixed ( byte* srcPtr = data )
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static string CreateStringOrThrow( void* srcPtr, uint lengthBytes ) {
		if ( lengthBytes % sizeof( char ) != 0 )
			throw new ArgumentException( $"Length must be divisible by {sizeof( char )}!", nameof( lengthBytes ) );
		return new string( new Span<char>( srcPtr, (int) lengthBytes / sizeof( char ) ) );
	}

	/// <summary>
	/// Returns a byte array with the string data inside. Size per character can be found using <see cref="sizeof(char)"/>
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public static byte[] ToBytes( this string data ) {
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

    public static void CopyInto<T, M>( this T[] source, M[] destination, uint offsetBytes = 0 ) where T : unmanaged where M : unmanaged {
        if ( source.Length * sizeof( T ) + offsetBytes > destination.Length * sizeof( M ) ) {
            Log.Warning( "Cannot copy outside the array." );
            return;
        }
        fixed ( void* dstPtr = destination )
        fixed ( void* srcPtr = source )
            Buffer.MemoryCopy( (byte*) srcPtr + offsetBytes, dstPtr, destination.Length * sizeof( M ), source.Length * sizeof( T ) - offsetBytes );
    }

    public static void CopyInto<T, M>( this Span<T> source, M[] destination, uint offsetBytes = 0 ) where T : unmanaged where M : unmanaged {
		if ( source.Length * sizeof( T ) + offsetBytes > destination.Length * sizeof( M ) ) {
			Log.Warning( "Cannot copy outside the array." );
			return;
		}
		fixed ( void* dstPtr = destination )
		fixed ( void* srcPtr = source )
			Buffer.MemoryCopy( (byte*) srcPtr + offsetBytes, dstPtr, destination.Length * sizeof( M ), source.Length * sizeof( T ) - offsetBytes );
	}

	public static void CopyInto<T, M>(this ReadOnlySpan<T> source, M[] destination, uint offsetBytes = 0) where T : unmanaged where M : unmanaged
	{
		if (source.Length * sizeof(T) + offsetBytes > destination.Length * sizeof(M))
		{
			Log.Warning("Cannot copy outside the array.");
			return;
		}
		fixed (void* dstPtr = destination)
		fixed (void* srcPtr = source)
			Buffer.MemoryCopy((byte*)srcPtr + offsetBytes, dstPtr, destination.Length * sizeof(M), source.Length * sizeof(T) - offsetBytes);
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
	public static byte[] Serialize( this ICustomizedSerializable serializable ) {
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
		Type? type = Global.Get<SerializableService>().GetFromIdentity( data.ToUnmanaged<Guid>() );
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
		if ( obj is not ICustomizedSerializable serializable )
			return Log.WarningThenReturnDefault<object?>( "Constructed type was not serializable" );
		if ( !serializable.DeserializeData( serializedData[ sizeof( Guid ).. ] ) )
			return Log.WarningThenReturnDefault<object?>( "Deserializing the serializable's data failed" );
		return serializable;
    }

    public static Vector3 Forward(this Quaternion q) => Vector3.Transform(-Vector3.UnitZ, q);

    public static Vector3 Backward(this Quaternion q) => Vector3.Transform(Vector3.UnitZ, q);

    public static Vector3 Left(this Quaternion q) => Vector3.Transform(-Vector3.UnitX, q);

    public static Vector3 Right(this Quaternion q) => Vector3.Transform(Vector3.UnitX, q);

    public static Vector3 Down(this Quaternion q) => Vector3.Transform(-Vector3.UnitY, q);

    public static Vector3 Up(this Quaternion q) => Vector3.Transform(Vector3.UnitY, q);

    public static Vector3 Cubify(this Vector3 v) => v * Math.Abs(Math.Min(Math.Min(1f / v.X, 1f / v.Y), 1f / v.Z));

    public static Quaternion DirectionVectorToQuaternion(this Vector3 v) => Quaternion.CreateFromYawPitchRoll(MathF.Atan2(-v.X, -v.Z), MathF.Asin(v.Y), 0);
}
