namespace Engine.Buffers;

public sealed class SubBuffer<TBuffer>( TBuffer hostBuffer, ulong offsetBytes, ulong lengthBytes ) : DisposableIdentifiable, IBufferSegment<ulong>, IReadableBuffer<ulong>, IWritableBuffer<ulong> where TBuffer : IBuffer<ulong>, IReadableBuffer<ulong>, IWritableBuffer<ulong> {
	private readonly TBuffer _hostBuffer = hostBuffer;

	public ulong OffsetBytes { get; private set; } = offsetBytes;
	public ulong LengthBytes { get; } = lengthBytes;

	public unsafe bool ReadRange<T>( Span<T> destination, ulong sourceOffsetBytes ) where T : unmanaged {
		if (this.Disposed)
			return false;
		if (sourceOffsetBytes + ((ulong) destination.Length * (uint) sizeof( T )) > this.LengthBytes)
			return false;
		return this._hostBuffer.ReadRange( destination, this.OffsetBytes + sourceOffsetBytes );
	}

	public unsafe bool ReadRange( void* dstPtr, ulong dstLengthBytes, ulong sourceOffsetBytes ) {
		if (this.Disposed)
			return false;
		if (sourceOffsetBytes + dstLengthBytes > this.LengthBytes)
			return false;
		return this._hostBuffer.ReadRange( dstPtr, dstLengthBytes, this.OffsetBytes + sourceOffsetBytes );
	}

	public unsafe bool WriteRange<T>( Span<T> source, ulong destinationOffsetBytes ) where T : unmanaged {
		if (this.Disposed)
			return false;
		if (destinationOffsetBytes + ((ulong) source.Length * (uint) sizeof( T )) > this.LengthBytes)
			return false;
		return this._hostBuffer.WriteRange( source, this.OffsetBytes + destinationOffsetBytes );
	}

	public unsafe bool WriteRange( void* srcPtr, ulong srcLengthBytes, ulong destinationOffsetBytes ) {
		if (this.Disposed)
			return false;
		if (destinationOffsetBytes + srcLengthBytes > this.LengthBytes)
			return false;
		return this._hostBuffer.WriteRange( srcPtr, srcLengthBytes, this.OffsetBytes + destinationOffsetBytes );
	}

	internal void SetOffsetBytes( ulong newOffsetBytes ) {
		if (this.OffsetBytes == newOffsetBytes)
			return;
		this.OffsetBytes = newOffsetBytes;
	}

	protected override bool InternalDispose() => true;
}
