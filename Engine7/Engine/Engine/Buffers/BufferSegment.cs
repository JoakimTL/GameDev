namespace Engine.Buffers;

public sealed class BufferSegment : DisposableIdentifiable, IRelocatingBufferSegment<ulong>, IReadableBuffer<ulong>, IWritableBuffer<ulong> {
	private readonly SegmentedSystemBuffer _buffer;

#if DEBUG
	public Memory<byte> DebugData => this._buffer.GetDebugSlice( this );
#endif

	public ulong OffsetBytes { get; private set; }
	public ulong LengthBytes { get; }

	public event Action<IBufferSegment<ulong>>? OffsetChanged;

	internal BufferSegment( SegmentedSystemBuffer buffer, ulong offsetBytes, ulong lengthBytes ) {
		this._buffer = buffer;
		this.OffsetBytes = offsetBytes;
		this.LengthBytes = lengthBytes;
	}

	public unsafe bool ReadRange<T>( Span<T> destination, ulong sourceOffsetBytes ) where T : unmanaged {
		if (this.Disposed)
			return false;
		if (sourceOffsetBytes + ((ulong) destination.Length * (uint) sizeof( T )) > this.LengthBytes)
			return false;
		return this._buffer.ReadRange( destination, this.OffsetBytes + sourceOffsetBytes );
	}

	public unsafe bool ReadRange( void* dstPtr, ulong dstLengthBytes, ulong sourceOffsetBytes ) {
		if (this.Disposed)
			return false;
		if (sourceOffsetBytes + dstLengthBytes > this.LengthBytes)
			return false;
		return this._buffer.ReadRange( dstPtr, dstLengthBytes, this.OffsetBytes + sourceOffsetBytes );
	}

	public unsafe bool WriteRange<T>( Span<T> source, ulong destinationOffsetBytes ) where T : unmanaged {
		if (this.Disposed)
			return false;
		if (destinationOffsetBytes + ((ulong) source.Length * (uint) sizeof( T )) > this.LengthBytes)
			return false;
		return this._buffer.WriteRange( source, this.OffsetBytes + destinationOffsetBytes );
	}

	public unsafe bool WriteRange( void* srcPtr, ulong srcLengthBytes, ulong destinationOffsetBytes ) {
		if (this.Disposed)
			return false;
		if (destinationOffsetBytes + srcLengthBytes > this.LengthBytes)
			return false;
		return this._buffer.WriteRange( srcPtr, srcLengthBytes, this.OffsetBytes + destinationOffsetBytes );
	}

	internal void SetOffsetBytes( ulong newOffsetBytes ) {
		if (this.OffsetBytes == newOffsetBytes)
			return;
		this.OffsetBytes = newOffsetBytes;
		OffsetChanged?.Invoke( this );
	}

	protected override bool InternalDispose() {
		this._buffer.Free( this );
		return true;
	}
}
