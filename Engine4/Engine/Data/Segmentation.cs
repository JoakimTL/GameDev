using System.Runtime.CompilerServices;

namespace Engine.Data;
public static class Segmentation {

	public const uint HeaderByteIndex = 0;

	public static byte[] SegmentWithPadding( uint startPadding, uint endPadding, params byte[][] data ) {
		if ( data is null )
			throw new ArgumentNullException( nameof( data ) );
		if ( data.Length == 0 ) {
			if ( startPadding + endPadding > 0 ) {
				return new byte[ startPadding + endPadding ];
			} else {
				return Array.Empty<byte>();
			}
		}
		//Header byte has two values stored inside, a 2-bit value indicating the size of the segment count (Might be greater then 256 segments), and a 2-bit value indicating the size of segment headers.

		//The segment count tells the parses how many segments to read the headers of before reading the segment data.
		//The size of the segment headers vary from 1 to 4 bytes.
		uint headerCountSizeBytes = BytesToRepresent( (uint) data.Length );
		uint headerSizeBytes = 0;
		uint totalLength = 1 + headerCountSizeBytes;
		for ( int i = 0; i < data.Length; i++ ) {
			uint byteToRepresent = BytesToRepresent( (uint) data[ i ].Length );
			if ( byteToRepresent > headerSizeBytes )
				headerSizeBytes = byteToRepresent;
			totalLength += (uint) data[ i ].Length;
		}
		totalLength += (uint) data.Length * headerSizeBytes;

		byte[] output = new byte[ totalLength + startPadding + endPadding ];
		uint offset = startPadding;
		unsafe {
			fixed ( byte* dstPtr = output ) {
				//Set preliminary data
				dstPtr[ offset++ ] = (byte) ( ( ( headerCountSizeBytes - 1 ) & 0b11 ) | ( ( ( headerSizeBytes - 1 ) & 0b11 ) << 2 ) );
				uint headerCount = (uint) data.Length;
				//Copy header count
				for ( int i = 0; i < headerCountSizeBytes; i++ )
					dstPtr[ offset++ ] = ( (byte*) &headerCount )[ i ];
				//Copy header lengths
				for ( int i = 0; i < headerCount; i++ ) {
					uint headerLen = (uint) data[ i ].Length;
					byte* headerLenPtr = ( (byte*) &headerLen );
					for ( int j = 0; j < headerSizeBytes; j++ )
						dstPtr[ offset++ ] = headerLenPtr[ j ];
				}
				//Copy data, if too slow copyblock can be used
				for ( int i = 0; i < headerCount; i++ ) {
					uint blockLength = (uint) data[ i ].Length;
					fixed ( byte* srcPtr = data[ i ] ) {
						if ( blockLength > 256 ) {
							Unsafe.CopyBlock( dstPtr + offset, srcPtr, blockLength );
							offset += blockLength;
						} else {
							for ( int j = 0; j < blockLength; j++ )
								dstPtr[ offset++ ] = srcPtr[ j ];
						}
					}
				}
			}
		}
		return output;
	}

	public static byte[] Segment( params byte[][] data ) => SegmentWithPadding( 0, 0, data );

	public static byte[][]? Parse( byte[] data, uint offset = 0 ) {
		if ( data is null )
			throw new ArgumentNullException( nameof( data ) );
		if ( data.Length == 0 ) {
			Log.Error( "Can't parse 0 bytes!" );
			return null;
		}

		if ( offset > data.Length ) {
			Log.Error( "Offset can't be greater than size of data!" );
			return null;
		}

		byte[][] blocks;
		unsafe {
			fixed ( byte* srcPtr = data ) {
				//Set preliminary data
				byte headerByte = srcPtr[ offset++ ];
				uint headerCountSize = ( headerByte & (uint) 0b11 ) + 1;
				uint headerSizeBytes = ( (uint) ( headerByte >> 2 ) & 0b11 ) + 1;
				uint headerCount = 0;
				//Copy header count
				for ( int i = 0; i < headerCountSize; i++ )
					( (byte*) &headerCount )[ i ] = srcPtr[ offset++ ];
				//Copy header lengths
				blocks = new byte[ headerCount ][];
				for ( int i = 0; i < headerCount; i++ ) {
					uint headerLen = 0;
					for ( int j = 0; j < headerSizeBytes; j++ )
						( (byte*) &headerLen )[ j ] = srcPtr[ offset++ ];
					blocks[ i ] = new byte[ headerLen ];
				}
				//Copy data, if too slow copyblock can be used
				for ( int i = 0; i < headerCount; i++ ) {
					uint blockLength = (uint) blocks[ i ].Length;
					fixed ( byte* dstPtr = blocks[ i ] ) {
						if ( blockLength > 256 ) {
							Unsafe.CopyBlock( dstPtr, srcPtr + offset, blockLength );
							offset += blockLength;
						} else {
							for ( int j = 0; j < blockLength; j++ )
								dstPtr[ j ] = srcPtr[ offset++ ];
						}
					}
				}
			}
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
