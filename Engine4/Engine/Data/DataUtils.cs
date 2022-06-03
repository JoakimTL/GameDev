using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Data;
public static class DataUtils {

	/// <summary>
	/// Returns the number of unmanaged values are needed to represent the bit count.
	/// </summary>
	public static int BitsInUnmanagedArray<T>( int bitCount ) where T : unmanaged {
		int bitsPerUnmanaged = Marshal.SizeOf<T>() * 8;
		(int quotient, int remainder) = Math.DivRem( bitCount, bitsPerUnmanaged );
		int unmanagedToRepresentBits = quotient;
		if ( remainder > 0 )
			++unmanagedToRepresentBits;
		return unmanagedToRepresentBits;
	}

	public static unsafe T? ToUnmanaged<T>( byte[] data ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		if ( data.Length != sizeBytes ) {
			Log.Warning( $"Unable to load {typeof( T ).Name} from data!" );
			return null;
		}
		fixed ( byte* srcPtr = data )
			return ( (T*) srcPtr )[ 0 ];
	}

	public static unsafe byte[] ToBytes<T>( T data ) where T : unmanaged {
		uint sizeBytes = (uint) sizeof( T );
		byte[] returnData = new byte[ sizeBytes ];
		fixed ( byte* dstPtr = returnData )
			( (T*) dstPtr )[ 0 ] = data;
		return returnData;
	}

	public static unsafe byte[] ToBytes<T>( Span<T> data ) where T : unmanaged {
		uint sizeBytes = (uint) ( sizeof( T ) * data.Length );
		byte[] returnData = new byte[ sizeBytes ];
		fixed ( T* srcPtr = data )
		fixed ( byte* dstPtr = returnData )
			Unsafe.CopyBlock( dstPtr, srcPtr, sizeBytes );
		return returnData;
	}

	public static unsafe string? ToStringUTF8( byte[] data ) {
		if ( data.Length % sizeof( char ) != 0 ) {
			Log.Warning( $"Data unaligned. Possible corruption!" );
			return null;
		}
		Span<char> chars;
		fixed ( byte* srcPtr = data )
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	public static unsafe string? ToStringUTF8( Span<byte> data ) {
		if ( data.Length % sizeof( char ) != 0 ) {
			Log.Warning( $"Data unaligned. Possible corruption!" );
			return null;
		}
		Span<char> chars;
		fixed ( byte* srcPtr = data )
			chars = new( srcPtr, data.Length / sizeof( char ) );
		return new string( chars );
	}

	/// <summary>
	/// Returns a byte array with the string data inside. Size per character can be found using <see cref="sizeof(char)"/>
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public static unsafe byte[] ToBytes( string data ) {
		byte[] returnData = new byte[ data.Length * sizeof( char ) ];
		fixed ( char* srcPtr = data )
		fixed ( byte* dstPtr = returnData )
			Unsafe.CopyBlock( dstPtr, srcPtr, (uint) ( data.Length * sizeof( char ) ) );
		return returnData;
	}
}
