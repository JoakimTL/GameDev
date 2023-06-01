using System.Runtime.CompilerServices;

namespace Engine.Datatypes;

public static class Segmentation {

	public const uint HeaderByteIndex = 0;

	public static byte[] SegmentWithPadding( uint startPaddingBytes, uint endPaddingBytes, IReadOnlyList<byte[]> data ) {
		if ( data is null )
			throw new ArgumentNullException( nameof( data ) );
		if ( data.Count == 0 )
			if ( startPaddingBytes + endPaddingBytes > 0 )
				return new byte[ startPaddingBytes + endPaddingBytes ];
			else
				return Array.Empty<byte>();
		//Header byte has two values stored inside, a 2-bit value indicating the size of the segment count (Might be greater than 256 segments), and a 2-bit value indicating the size of segment headers.

		//The segment count tells the parses how many segments to read the headers of before reading the segment data.
		//The size of the segment headers vary from 1 to 4 bytes.
		uint headerCountSizeBytes = BytesToRepresent( (uint) data.Count );
		uint headerSizeBytes = 0;
		uint totalLength = 1 + headerCountSizeBytes;
		for ( int i = 0; i < data.Count; i++ ) {
			uint byteToRepresent = BytesToRepresent( (uint) data[ i ].Length );
			if ( byteToRepresent > headerSizeBytes )
				headerSizeBytes = byteToRepresent;
			totalLength += (uint) data[ i ].Length;
		}
		totalLength += (uint) data.Count * headerSizeBytes;

		uint finalLength = totalLength + startPaddingBytes + endPaddingBytes;
		byte[] output = new byte[ finalLength ];
		unsafe {
			fixed ( byte* dstPtr = output ) {
				uint offset = startPaddingBytes;
				//Set preliminary data
				dstPtr[ offset++ ] = (byte) ( headerCountSizeBytes - 1 & 0b11 | ( headerSizeBytes - 1 & 0b11 ) << 2 );
				uint headerCount = (uint) data.Count;
				//Copy header count
				for ( int i = 0; i < headerCountSizeBytes; i++ )
					dstPtr[ offset++ ] = ( (byte*) &headerCount )[ i ];
				//Copy header lengths
				for ( int i = 0; i < headerCount; i++ ) {
					uint headerLen = (uint) data[ i ].Length;
					byte* headerLenPtr = (byte*) &headerLen;
					for ( int j = 0; j < headerSizeBytes; j++ )
						dstPtr[ offset++ ] = headerLenPtr[ j ];
				}
				//Copy data
				for ( int i = 0; i < headerCount; i++ ) {
					uint blockLength = (uint) data[ i ].Length;
					fixed ( byte* srcPtr = data[ i ] )
						Unsafe.CopyBlock( dstPtr + offset, srcPtr, blockLength );
					offset += blockLength;
				}
			}
		}
		return output;
	}

	public static byte[] Segment( IReadOnlyList<byte[]> data ) => SegmentWithPadding( 0, 0, data );
	public static byte[] Segment( params byte[][] data ) => SegmentWithPadding( 0, 0, data );

	public static byte[][]? Parse( byte[] data, uint offsetBytes = 0 ) {
		if ( data.Length == 0 )
			return Log.WarningThenReturn( "No data to parse.", Array.Empty<byte[]>() );

		unsafe {
			fixed ( byte* srcPtr = data )
				return Parse( srcPtr, (uint) data.Length, offsetBytes );
		}
	}
	public static byte[][] ParseOrThrow( byte[] data, uint offsetBytes = 0 ) {
		if ( data.Length == 0 )
			return Log.WarningThenReturn("No data to parse.", Array.Empty<byte[]>());

		unsafe {
			fixed ( byte* srcPtr = data )
				return ParseOrThrow( srcPtr, (uint) data.Length, offsetBytes );
		}
	}

	private static unsafe byte[][]? Parse( byte* srcPtr, uint maxLength, uint offsetBytes ) {
		if ( offsetBytes > maxLength )
			return Log.WarningThenReturnDefault<byte[][]>( "Offset can't be greater than size of data!" );
		return ParseInternal( srcPtr, maxLength, offsetBytes );
	}


	private static unsafe byte[][] ParseOrThrow( byte* srcPtr, uint maxLength, uint offsetBytes ) {
		if ( offsetBytes > maxLength )
			throw new ArgumentException( "Offset can't be greater than size of data!" );
		return ParseInternal( srcPtr, maxLength, offsetBytes );
	}

	private static unsafe byte[][] ParseInternal( byte* srcPtr, uint maxLength, uint offsetBytes ) {
		//Set preliminary data
		byte headerByte = srcPtr[ offsetBytes++ ];
		uint headerCountSize = ( headerByte & (uint) 0b11 ) + 1;
		uint headerSizeBytes = ( (uint) ( headerByte >> 2 ) & 0b11 ) + 1;
		uint headerCount = 0;
		//Copy header count
		for ( int i = 0; i < headerCountSize; i++ )
			( (byte*) &headerCount )[ i ] = srcPtr[ offsetBytes++ ];

		//Copy header lengths
		var blocks = new byte[ headerCount ][];
		for ( int i = 0; i < headerCount; i++ ) {
			uint headerLen = 0;
			for ( int j = 0; j < headerSizeBytes; j++ )
				( (byte*) &headerLen )[ j ] = srcPtr[ offsetBytes++ ];
			blocks[ i ] = new byte[ headerLen ];
		}
		//Copy data
		for ( int i = 0; i < headerCount; i++ ) {
			uint blockLength = (uint) blocks[ i ].Length;
			fixed ( byte* dstPtr = blocks[ i ] )
				Unsafe.CopyBlock( dstPtr, srcPtr + offsetBytes, blockLength );
			offsetBytes += blockLength;
		}
		return blocks;
	}

	/// <summary>
	/// Gets the number of bytes necessary to represent the data in the number. A value between 1 and 4.
	/// </summary>
	/// <param name="number"></param>
	/// <returns>A value between 1 and 4.</returns>
	public static uint BytesToRepresent( uint number ) {
		uint count = 0;
		while ( number > 0 ) {
			number >>= 8;
			count++;
		}
		return count;
	}

}