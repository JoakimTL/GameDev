using Engine.Structure.Interfaces.Buffers;

namespace Engine.Datatypes.Buffers;

internal unsafe class BufferSegment : Identifiable, ISegmentedBufferSegment {

	private readonly SegmentedBuffer _underlyingBuffer;
	private readonly Func<ulong, nuint, ulong, bool> _writeFunction;

	private bool _disposed;
	public ulong OffsetBytes { get; private set; }
	public ulong SizeBytes { get; }

	public event IListenableBufferSegment.BufferSegmentOffsetEvent? OffsetChanged;
#if DEBUG
	public ReadOnlyMemory<byte> Bytes => _underlyingBuffer.GetDebugData(OffsetBytes, (uint)SizeBytes);
#endif

	internal BufferSegment( SegmentedBuffer buffer, ulong offsetBytes, ulong sizeBytes, Func<ulong, nuint, ulong, bool> writeFunction ) {
		_underlyingBuffer = buffer;
		OffsetBytes = offsetBytes;
		SizeBytes = sizeBytes;
		_writeFunction = writeFunction;
	}

	internal void SetOffset( ulong offsetBytes ) {
		if ( _disposed ) {
			this.LogWarning( "Tried to access disposed segment." );
			return;
		}
		OffsetBytes = offsetBytes;
		OffsetChanged?.Invoke( this, offsetBytes );
	}

	public IIndexableReadOnlyBufferSegment<T> Read<T>( ulong offsetBytes, ulong lengthElements ) where T : unmanaged {
		if ( _disposed ) {
			this.LogWarning( "Tried to access disposed segment." );
			return IndexableReadOnlyBufferSegment<T>.Empty;
		}
		ulong elementSize = lengthElements * (uint) sizeof( T );
		if ( !this.IsInsideSegment( offsetBytes, elementSize ) ) {
			this.LogWarning( "Tried to access data outside segment." );
			return IndexableReadOnlyBufferSegment<T>.Empty;
		}
		return _underlyingBuffer.Read<T>( OffsetBytes + offsetBytes, lengthElements );
	}

	public T ReadOne<T>( ulong offsetBytes ) where T : unmanaged {
		if ( _disposed ) {
			this.LogWarning( "Tried to access disposed segment." );
			return default;
		}
		uint elementSize = (uint) sizeof( T );
		if ( !this.IsInsideSegment( offsetBytes, elementSize ) ) {
			this.LogWarning( "Tried to access data outside segment." );
			return default;
		}
		return _underlyingBuffer.ReadOne<T>( OffsetBytes + offsetBytes );
	}

	public T[] Snapshot<T>( ulong offsetBytes, ulong lengthElements ) where T : unmanaged {
		if ( _disposed ) {
			this.LogWarning( "Tried to access disposed segment." );
			return Array.Empty<T>();
		}
		ulong elementSize = lengthElements * (uint) sizeof( T );
		if ( !this.IsInsideSegment( offsetBytes, elementSize ) ) {
			this.LogWarning( "Tried to access data outside segment." );
			return Array.Empty<T>();
		}
		return _underlyingBuffer.Snapshot<T>( OffsetBytes + offsetBytes, lengthElements );
	}

	public bool Write<T>( ulong offsetBytes, T[] data ) where T : unmanaged {
		if ( _disposed ) {
			this.LogWarning( "Tried to access disposed segment." );
			return false;
		}
		ulong elementSize = (ulong)data.Length * (uint) sizeof( T );
		fixed ( T* dataPtr = data)
			return Write( offsetBytes, dataPtr, elementSize );
	}

	public bool Write<T>( ulong offsetBytes, ReadOnlyMemory<T> data ) where T : unmanaged {
		if ( _disposed ) {
			this.LogWarning( "Tried to access disposed segment." );
			return false;
		}
		ulong elementSize = (ulong) data.Length * (uint) sizeof( T );
		using ( var pinnedMemory = data.Pin() )
			return Write( offsetBytes, pinnedMemory.Pointer, elementSize );
	}

	public bool Write<T>( ulong offsetBytes, ReadOnlySpan<T> data ) where T : unmanaged {
		if ( _disposed ) {
			this.LogWarning( "Tried to access disposed segment." );
			return false;
		}	
		ulong elementSize = (ulong) data.Length * (uint) sizeof( T );
		fixed ( T* dataPtr = data )
			return Write( offsetBytes, dataPtr, elementSize );
	}

	public bool Write<T>( ulong offsetBytes, ref T data ) where T : unmanaged {
		if ( _disposed ) {
			this.LogWarning( "Tried to access disposed segment." );
			return false;
		}
		fixed ( T* dataPtr = &data )
			return Write( offsetBytes, dataPtr, (uint)sizeof( T ) );
	}

	public bool Write<T>( ulong offsetBytes, T data ) where T : unmanaged {
		if ( _disposed ) {
			this.LogWarning( "Tried to access disposed segment." );
			return false;
		}
		return Write( offsetBytes, &data, (uint) sizeof( T ) );
	}

	public unsafe bool Write( ulong offsetBytes, void* data, ulong sizeBytes ) {
		if ( _disposed ) {
			this.LogWarning( "Tried to access disposed segment." );
			return false;
		}
		if ( !this.IsInsideSegment( offsetBytes, sizeBytes ) ) {
			this.LogWarning( "Tried to write data outside segment." );
			return false;
		}
		return _writeFunction.Invoke( offsetBytes, (nuint) data, sizeBytes );
	}

	public void Dispose() {
		_underlyingBuffer.DisposeSegment( this );
	}

	internal void SetDisposed() {
		_disposed = true;
	}
}