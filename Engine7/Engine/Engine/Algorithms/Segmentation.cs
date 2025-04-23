using Engine.Buffers;
using Engine.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Algorithms;

public sealed class Segmenter : IDisposable {
	private ThreadedByteBuffer? _buffer;

	public Segmenter( ReadOnlySpan<byte> initialSegment ) {
		_buffer = Segmentation.Start( initialSegment );
	}

	public void Append( ReadOnlySpan<byte> data ) {
		if (_buffer is null)
			throw new ObjectDisposedException( nameof( Segmenter ) );
		_buffer.Append( data );
	}

	public PooledBufferData Flush() {
		if (_buffer is null)
			throw new ObjectDisposedException( nameof( Segmenter ) );
		return _buffer.Flush( );
	}

	public void Dispose() {
		_buffer = null;
	}
}
public sealed class Desegmenter {
	private readonly ReadOnlyMemory<byte> _data;
	private int _caret;

	public Desegmenter( ReadOnlyMemory<byte> data ) {
		_data = data;
		_caret = 0;
		RequiredSpanLength = data.DetermineSpanLength();
	}

	public int RequiredSpanLength { get; }

	public int ReadInto( Span<byte> output ) => _data.CopySegmentInto( output, ref _caret );
}

public static class Segmentation {

	//TODO write unit tests.

	public static unsafe ThreadedByteBuffer Start( ReadOnlySpan<byte> data ) {
		var buffer = ThreadedByteBuffer.GetBuffer("segmentation");
		return buffer.Append( data );
	}

	public static unsafe ThreadedByteBuffer Append( this ThreadedByteBuffer buffer, ReadOnlySpan<byte> data ) {
		byte* header = stackalloc byte[ 4 ];
		*(int*) header = data.Length;
		buffer.Add( new Span<byte>( header, 4 ) );
		buffer.Add( data );
		return buffer;
	}

	public static unsafe PooledBufferData Flush( this ThreadedByteBuffer buffer ) {
		PooledBufferData result = buffer.GetData( 4 );
		using (System.Buffers.MemoryHandle memoryHandle = result.Payload.Pin()) {
			int currentLength;
			int longestLength = 0;
			int caret = 0;
			while (caret < result.Payload.Length - 8 /*4 for reading header, 4 for last header*/) {
				currentLength = *(int*) ((byte*) memoryHandle.Pointer + caret);
				if (currentLength > longestLength)
					longestLength = currentLength;
				caret += currentLength + 4;
			}
			//Set the last header to the longest length, this makes it extremely easy to find out how long your buffer needs to be without allocating.
			*(int*) ((byte*) memoryHandle.Pointer + result.Payload.Length - 4) = longestLength;
		}
		return result;
	}

	public static unsafe int DetermineSpanLength( this ReadOnlyMemory<byte> segmentedData ) {
		if (segmentedData.Length < 8)
			return -1;
		return MemoryMarshal.Read<int>( segmentedData.Span[ ^4.. ] );
	}

	public static unsafe int CopySegmentInto( this ReadOnlyMemory<byte> segmentedData, Span<byte> output, ref int caret ) {
		if (caret > segmentedData.Length - 4)
			return -1;
		int lengthToCopy = ReadInt32( segmentedData[caret..4].Span );
		caret += 4;
		if (lengthToCopy > segmentedData.Length - 4)
			return -1;
		if (lengthToCopy > output.Length)
			throw new ArgumentOutOfRangeException( nameof( output ), "Output buffer is too small to copy segment into." );
		segmentedData[ caret..(caret + lengthToCopy) ].Span.CopyTo( output );
		caret += lengthToCopy;
		return lengthToCopy;
	}

	public static int ReadInt32( ReadOnlySpan<byte> span ) {
		if (span.Length < 4)
			throw new ArgumentOutOfRangeException( nameof( span ), "Span is too small to read an int32." );
		return System.Runtime.InteropServices.MemoryMarshal.Read<int>( span );
	}
}