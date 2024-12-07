using System.Diagnostics.CodeAnalysis;

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

/// <summary>
/// Allows you to cut up any buffer into smaller segments. Unlike <see cref="SegmentedSystemBuffer"/> the buffer is always contiguous.
/// </summary>
/// <typeparam name="TBuffer"></typeparam>
public sealed class SubBufferManager<TBuffer>( TBuffer hostBuffer ) where TBuffer : IBuffer<ulong>, IReadableBuffer<ulong>, IWritableBuffer<ulong> {
	private readonly TBuffer _hostBuffer = hostBuffer;
	/// <summary>
	/// Null if the host buffer does not implement <see cref="ICopyableBuffer{ulong}"/>.
	/// </summary>
	private readonly ICopyableBuffer<ulong>? _copyableHostBuffer = hostBuffer as ICopyableBuffer<ulong>;
	private readonly List<SubBuffer<TBuffer>> _subBuffers = [];
	private ulong _currentOffsetCaret = 0;
	public uint Count => (uint) this._subBuffers.Count;

	public bool TryAllocate( ulong lengthBytes, [NotNullWhen( true )] out SubBuffer<TBuffer>? subBuffer ) {
		subBuffer = null;
		if (this._currentOffsetCaret + lengthBytes > this._hostBuffer.LengthBytes)
			return false;
		subBuffer = new( this._hostBuffer, this._currentOffsetCaret, lengthBytes );
		this._currentOffsetCaret += lengthBytes;
		this._subBuffers.Add( subBuffer );
		return true;
	}

	public void Remove( SubBuffer<TBuffer> subBuffer ) {
		int indexOf = this._subBuffers.IndexOf( subBuffer );
		this._subBuffers.RemoveAt( indexOf );
		if (indexOf == this._subBuffers.Count)
			return;
		ulong moveStart = this._subBuffers[indexOf].OffsetBytes;
		ulong moveLength = this._currentOffsetCaret;
		this._currentOffsetCaret -= subBuffer.LengthBytes;
		if (this._copyableHostBuffer is not null) {
			this._copyableHostBuffer.CopyTo( this._hostBuffer, moveStart, moveStart - subBuffer.LengthBytes, moveLength );
		} else {
			this._hostBuffer.CopyTo( this._hostBuffer, moveStart, moveStart - subBuffer.LengthBytes, (int) moveLength );
		}
		for (int i = indexOf; i < this._subBuffers.Count; i++) {
			SubBuffer<TBuffer> current = this._subBuffers[ i ];
			current.SetOffsetBytes( current.OffsetBytes - subBuffer.LengthBytes );
		}
	}
}

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