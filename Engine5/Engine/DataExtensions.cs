using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
}
