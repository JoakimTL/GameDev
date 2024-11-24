namespace Engine.Buffers;

public sealed class BufferSegment : DisposableIdentifiable, IBufferSegment<ulong>, IReadableBuffer<ulong>, IWritableBuffer<ulong> {
	private readonly SegmentedSystemBuffer _buffer;

	public ulong OffsetBytes { get; private set; }
	public ulong LengthBytes { get; }

	public event Action<IBufferSegment<ulong>>? OffsetChanged;

	internal BufferSegment( SegmentedSystemBuffer buffer, ulong offsetBytes, ulong lengthBytes ) {
		_buffer = buffer;
		OffsetBytes = offsetBytes;
		LengthBytes = lengthBytes;
	}

	public unsafe bool ReadRange<T>( Span<T> destination, ulong sourceOffsetBytes ) where T : unmanaged {
		if (Disposed)
			return false;
		if (sourceOffsetBytes + ((ulong) destination.Length * (uint) sizeof( T )) > LengthBytes)
			return false;
		return _buffer.ReadRange( destination, OffsetBytes + sourceOffsetBytes );
	}

	public unsafe bool ReadRange( void* dstPtr, ulong dstLengthBytes, ulong sourceOffsetBytes ) {
		if (Disposed)
			return false;
		if (sourceOffsetBytes + dstLengthBytes > LengthBytes)
			return false;
		return _buffer.ReadRange( dstPtr, dstLengthBytes, OffsetBytes + sourceOffsetBytes );
	}

	public unsafe bool WriteRange<T>( Span<T> source, ulong destinationOffsetBytes ) where T : unmanaged {
		if (Disposed)
			return false;
		if (destinationOffsetBytes + ((ulong) source.Length * (uint) sizeof( T )) > LengthBytes)
			return false;
		if (!_buffer.WriteRange( source, OffsetBytes + destinationOffsetBytes ))
			return false;
		return true;
	}

	public unsafe bool WriteRange( void* srcPtr, ulong srcLengthBytes, ulong destinationOffsetBytes ) {
		if (Disposed)
			return false;
		if (destinationOffsetBytes + srcLengthBytes > LengthBytes)
			return false;
		if (!_buffer.WriteRange( srcPtr, srcLengthBytes, OffsetBytes + destinationOffsetBytes ))
			return false;
		return true;
	}

	internal void SetOffsetBytes( ulong newOffsetBytes ) {
		if (OffsetBytes == newOffsetBytes)
			return;
		OffsetBytes = newOffsetBytes;
		OffsetChanged?.Invoke( this );
	}

	protected override bool InternalDispose() {
		_buffer.Free( this );
		return true;
	}
}