using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine;

public static unsafe class DataExtensions {

	public static IEnumerable<T> RangeTo<T>( this T inclusiveStart, T exclusiveEnd ) where T : IBinaryInteger<T> {
		for (T i = inclusiveStart; i < exclusiveEnd; i += T.One)
			yield return i;
	}

	public static IEnumerable<TOut> IndexedSelect<TIn, TOut>( this IEnumerable<TIn> enumerable, Func<int, TIn, TOut> function ) {
		int index = 0;
		foreach (TIn inItem in enumerable)
			yield return function( index++, inItem );
	}

	public static void ClearThenAddRange<T>( this List<T> list, IEnumerable<T> range ) {
		list.Clear();
		list.AddRange( range );
	}

	public static string ToBinaryString<T>( this T value ) where T : unmanaged {
		int valueSizeBytes = sizeof( T );
		byte* ptr = (byte*) &value;
		return string.Join( " ", 0.RangeTo( valueSizeBytes ).Select( p => string.Join( "", 0.RangeTo( 8 ).Select( q => (ptr[ p ] >> q) & 1 ) ) ) );
	}

	public static string ToHexString<T>( this T value, bool upperCase = false ) where T : unmanaged {
		int valueSizeBytes = sizeof( T );
		byte* ptr = (byte*) &value;
		return string.Join( " ", 0.RangeTo( valueSizeBytes ).Select( p => ptr[ p ].ToString( upperCase ? "X2" : "x2" ) ) );
	}

	/// <summary>
	/// Returns the number of unmanaged values needed to represent a number of bits.
	/// </summary>
	public static int NeededForBitCount<T>( this int bitCount ) where T : unmanaged {
		int bitsPerUnmanaged = System.Runtime.InteropServices.Marshal.SizeOf<T>() * 8;
		(int quotient, int remainder) = System.Math.DivRem( bitCount, bitsPerUnmanaged );
		int unmanagedToRepresentBits = quotient;
		if (remainder > 0)
			++unmanagedToRepresentBits;
		return unmanagedToRepresentBits;
	}

	public static T ToUnmanaged<T>( this byte[] data, uint offset = 0 ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		if (data.Length < sizeBytes + offset)
			return Log.WarningThenReturnDefault<T>( $"Unable to load {typeof( T ).Name} from data!" );
		fixed (byte* srcPtr = data)
			return *(T*) (srcPtr + offset);
	}

	public static T? ToUnmanagedNullable<T>( this byte[] data, uint offset = 0 ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		if (data.Length < sizeBytes + offset)
			return Log.WarningThenReturnDefault<T?>( $"Unable to load {typeof( T ).Name} from data!" );
		fixed (byte* srcPtr = data)
			return *(T*) (srcPtr + offset);
	}

	public static T ToUnmanagedOrThrow<T>( this byte[] data, uint offset = 0 ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		if (data.Length < sizeBytes + offset)
			throw new InvalidDataException( $"Data has length {data.Length}, but expected size was {sizeBytes + offset}!" );
		fixed (byte* srcPtr = data)
			return *(T*) (srcPtr + offset);
	}

	public static byte[] ToBytes<T>( this T data ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		byte[] returnData = new byte[ sizeBytes ];
		fixed (byte* dstPtr = returnData)
			*(T*) dstPtr = data;
		return returnData;
	}

	public static byte[] ToBytes<T>( this Span<T> data ) where T : unmanaged {
		uint sizeBytes = (uint) (sizeof( T ) * data.Length);
		byte[] returnData = new byte[ sizeBytes ];
		fixed (T* srcPtr = data)
		fixed (byte* dstPtr = returnData)
			System.Runtime.CompilerServices.Unsafe.CopyBlock( dstPtr, srcPtr, sizeBytes );
		return returnData;
	}

	public static string? CreateString( this byte[] data ) {
		if (data.Length % sizeof( char ) != 0)
			return Log.WarningThenReturnDefault<string?>( $"Length must be divisible by {sizeof( char )}!" );
		Span<char> chars;
		fixed (byte* srcPtr = data)
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static string? CreateString( this Span<byte> data ) {
		if (data.Length % sizeof( char ) != 0)
			return Log.WarningThenReturnDefault<string?>( $"Length must be divisible by {sizeof( char )}!" );
		Span<char> chars;
		fixed (byte* srcPtr = data)
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static string? CreateString( void* srcPtr, uint lengthBytes ) {
		if (lengthBytes % sizeof( char ) != 0)
			return Log.WarningThenReturnDefault<string?>( $"Length must be divisible by {sizeof( char )}!" );
		return new string( new Span<char>( srcPtr, (int) lengthBytes / sizeof( char ) ) );
	}

	public static string CreateStringOrThrow( this byte[] data ) {
		if (data.Length % sizeof( char ) != 0)
			throw new ArgumentException( $"Length must be divisible by {sizeof( char )}!", nameof( data ) );
		Span<char> chars;
		fixed (byte* srcPtr = data)
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static string CreateStringOrThrow( this Span<byte> data ) {
		if (data.Length % sizeof( char ) != 0)
			throw new ArgumentException( $"Length must be divisible by {sizeof( char )}!", nameof( data ) );
		Span<char> chars;
		fixed (byte* srcPtr = data)
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static string CreateStringOrThrow( void* srcPtr, uint lengthBytes ) {
		if (lengthBytes % sizeof( char ) != 0)
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
		fixed (char* srcPtr = data)
		fixed (byte* dstPtr = returnData)
			System.Runtime.CompilerServices.Unsafe.CopyBlock( dstPtr, srcPtr, (uint) (data.Length * sizeof( char )) );
		return returnData;
	}

	public static void CopyInto<T, M>( this T source, M[] destination, uint dstOffsetBytes = 0 ) where T : unmanaged where M : unmanaged {
		if (sizeof( T ) + dstOffsetBytes > destination.Length * sizeof( M )) {
			Log.Warning( "Cannot copy outside the array." );
			return;
		}
		fixed (M* dstPtr = destination)
			*(T*) ((byte*) dstPtr + dstOffsetBytes) = source;
	}

	public static void CopyInto<T, M>( this T[] source, M[] destination, uint dstOffsetBytes = 0 ) where T : unmanaged where M : unmanaged {
		if ((source.Length * sizeof( T )) + dstOffsetBytes > destination.Length * sizeof( M )) {
			Log.Warning( "Cannot copy outside the array." );
			return;
		}
		fixed (void* dstPtr = destination)
		fixed (void* srcPtr = source)
			Buffer.MemoryCopy( srcPtr, (byte*) dstPtr + dstOffsetBytes, (destination.Length * sizeof( M )) - dstOffsetBytes, source.Length * sizeof( T ) );
	}

	public static void CopyInto<T, M>( this Span<T> source, M[] destination, uint dstOffsetBytes = 0 ) where T : unmanaged where M : unmanaged {
		if ((source.Length * sizeof( T )) + dstOffsetBytes > destination.Length * sizeof( M )) {
			Log.Warning( "Cannot copy outside the array." );
			return;
		}
		fixed (void* dstPtr = destination)
		fixed (void* srcPtr = source)
			Buffer.MemoryCopy( srcPtr, (byte*) dstPtr + dstOffsetBytes, (destination.Length * sizeof( M )) - dstOffsetBytes, source.Length * sizeof( T ) );
	}

	public static void CopyInto<T, M>( this ReadOnlySpan<T> source, M[] destination, uint dstOffsetBytes = 0 ) where T : unmanaged where M : unmanaged {
		if ((source.Length * sizeof( T )) + dstOffsetBytes > destination.Length * sizeof( M )) {
			Log.Warning( "Cannot copy outside the array." );
			return;
		}
		fixed (void* dstPtr = destination)
		fixed (void* srcPtr = source)
			Buffer.MemoryCopy( srcPtr, (byte*) dstPtr + dstOffsetBytes, (destination.Length * sizeof( M )) - dstOffsetBytes, source.Length * sizeof( T ) );
	}

	public static void CopyInto<T, M>( this T source, Span<M> destination, uint dstOffsetBytes = 0 ) where T : unmanaged where M : unmanaged {
		if (sizeof( T ) + dstOffsetBytes > destination.Length * sizeof( M )) {
			Log.Warning( "Cannot copy outside the array." );
			return;
		}
		fixed (M* dstPtr = destination)
			*(T*) ((byte*) dstPtr + dstOffsetBytes) = source;
	}


	public static byte[] ToArray( this nuint pointer, nuint lengthBytes ) {
		if (lengthBytes == 0)
			return Array.Empty<byte>();
		byte[] @returnArray = new byte[ lengthBytes ];
		fixed (void* ptr = @returnArray)
			Unsafe.CopyBlock( ptr, pointer.ToPointer(), (uint) lengthBytes );
		return @returnArray;
	}

	public static T[] ToArray<T>( this nuint pointer, nuint lengthBytes ) where T : unmanaged {
		if (lengthBytes == 0)
			return Array.Empty<T>();
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if (lengthBytes % typeSizeBytes != 0) 
			return Log.WarningThenReturn( $"Attempted to convert a pointer to an array, but the length {lengthBytes} is not divisible by {typeSizeBytes}!", Array.Empty<T>() );
		T[] @returnArray = new T[ lengthBytes / typeSizeBytes ];
		fixed (void* ptr = @returnArray)
			Unsafe.CopyBlock( ptr, pointer.ToPointer(), (uint) lengthBytes );
		return @returnArray;
	}
}