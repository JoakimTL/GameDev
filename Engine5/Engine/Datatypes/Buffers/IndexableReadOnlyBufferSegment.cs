using Engine.Structure.Interfaces.Buffers;
using System.Numerics;

namespace Engine.Datatypes.Buffers;

internal unsafe class IndexableReadOnlyBufferSegment<T> : Identifiable, IIndexableReadOnlyBufferSegment<T> where T : unmanaged {

	public static readonly IndexableReadOnlyBufferSegment<T> Empty = new( null!, 0u, 0u );

	public ulong OffsetBytes { get; }

	public ulong SizeBytes { get; }

	public ulong ElementCount { get; }

	protected readonly IReadableBuffer _underlyingBuffer;

	public IndexableReadOnlyBufferSegment( IReadableBuffer underlying, ulong size, ulong offset ) {
		_underlyingBuffer = underlying;
		SizeBytes = size;
		OffsetBytes = offset;
		ElementCount = SizeBytes / (uint) sizeof( T );
	}

	public T this[ ulong index ] {
		get {
			uint elementSize = (uint)sizeof( T );
			ulong localOffset = index * elementSize;
			if ( !this.IsInsideSegment( localOffset, elementSize ) ) {
				this.LogWarning( "Tried to access data outside segment." );
				return default;
			}
			return _underlyingBuffer.ReadOne<T>( localOffset + OffsetBytes );
		}
	}

	public D ReadOne<D>( ulong offsetBytes ) where D : unmanaged {
		ulong elementSize = (uint) sizeof( D );
		ulong localOffset = offsetBytes;
		if ( !this.IsInsideSegment( localOffset, elementSize ) ) {
			this.LogWarning( "Tried to access data outside segment." );
			return default;
		}
		return _underlyingBuffer.ReadOne<D>( localOffset + OffsetBytes );
	}

	public D[] Snapshot<D>( ulong offsetBytes, ulong lengthElements ) where D : unmanaged {
		ulong arraySize = lengthElements * (uint) sizeof( D );
		ulong localOffset = offsetBytes;
		if ( !this.IsInsideSegment( localOffset, arraySize ) ) {
			this.LogWarning( "Tried to access data outside segment." );
			return Array.Empty<D>();
		}
		return _underlyingBuffer.Snapshot<D>( localOffset + OffsetBytes, arraySize );
	}
}