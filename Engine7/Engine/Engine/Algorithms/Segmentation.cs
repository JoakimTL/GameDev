using Engine.Buffers;
using Engine.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Algorithms;

public sealed class Segmenter : IDisposable {
	private ThreadedByteBuffer? _buffer;

	public Segmenter( ReadOnlySpan<byte> initialSegment ) {
		_buffer = Segmentation.StartSegmentation( initialSegment );
	}

	public void Append( ReadOnlySpan<byte> data ) {
		if (_buffer is null)
			throw new ObjectDisposedException( nameof( Segmenter ) );
		_buffer.AppendSegment( data );
	}

	public PooledBufferData Flush(string? name) {
		if (_buffer is null)
			throw new ObjectDisposedException( nameof( Segmenter ) );
		return _buffer.FlushSegments(name);
	}

	public void Dispose() {
		_buffer = null;
	}
}
public sealed unsafe class Desegmenter {
	private int _caret;
	private int _readsPerformed;

	public Desegmenter() {
		_caret = 0;
		_readsPerformed = 0;
	}

	public int Caret => _caret;

	public int ReadsPerformed => _readsPerformed;

	public bool ReadInto( ReadOnlySpan<byte> segmentedData, Span<byte> output, out int readBytes ) {
		readBytes = segmentedData.CopySegmentInto( output, ref _caret );
		_readsPerformed++;
		return readBytes >= 0;
	}
}

public static class Segmentation {

	//TODO write unit tests.

	public static unsafe ThreadedByteBuffer StartSegmentation( ReadOnlySpan<byte> data ) {
		ThreadedByteBuffer buffer = ThreadedByteBuffer.GetBuffer( "segmentation" );
		return buffer.AppendSegment( data );
	}

	public static unsafe ThreadedByteBuffer AppendSegment( this ThreadedByteBuffer buffer, ReadOnlySpan<byte> data ) {
		byte* header = stackalloc byte[ 4 ];
		*(int*) header = data.Length;
		buffer.Add( new Span<byte>( header, 4 ) );
		buffer.Add( data );
		return buffer;
	}

	public static unsafe PooledBufferData FlushSegments( this ThreadedByteBuffer buffer, object? tag ) => buffer.GetData( tag: tag );

	public static unsafe int DetermineSpanLength( this ReadOnlySpan<byte> segmentedData ) {
		if (segmentedData.Length < 4)
			return -1;
		int caret = 0;
		int longestLength = -1;
		while ( caret < segmentedData.Length - 4) {
			int lengthToCopy = ReadInt32( segmentedData[ caret..(caret + 4) ] );
			caret += 4;
			caret += lengthToCopy;
			if (lengthToCopy > longestLength)
				longestLength = lengthToCopy;
		}
		return longestLength;
	}

	public static unsafe int CopySegmentInto( this ReadOnlySpan<byte> segmentedData, Span<byte> output, ref int caret ) {
		if (caret > segmentedData.Length - 4)
			return -1;
		int lengthToCopy = ReadInt32( segmentedData[ caret..(caret + 4) ] );
		caret += 4;
		if (lengthToCopy > segmentedData.Length)
			return -1;
		if (lengthToCopy > output.Length)
			throw new ArgumentOutOfRangeException( nameof( output ), "Output buffer is too small to copy segment into." );
		segmentedData[ caret..(caret + lengthToCopy) ].CopyTo( output );
		caret += lengthToCopy;
		return lengthToCopy;
	}

	public static int ReadInt32( ReadOnlySpan<byte> span ) {
		if (span.Length < 4)
			throw new ArgumentOutOfRangeException( nameof( span ), "Span is too small to read an int32." );
		return MemoryMarshal.Read<int>( span );
	}
}