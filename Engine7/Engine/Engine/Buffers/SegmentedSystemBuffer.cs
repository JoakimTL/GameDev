using Engine.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Buffers;

public unsafe class SegmentedSystemBuffer( ulong initialLengthBytes, BufferAutoResizeMode autoResizeMode ) : SystemBufferBase<ulong>( initialLengthBytes ) {
	private readonly List<BufferSegment> _segments = [];
	private readonly ulong _initialLengthBytes = initialLengthBytes;
	private ulong _currentOffsetCaret;
	private bool _fragmented;
	private readonly BufferAutoResizeMode _autoResizeMode = autoResizeMode;

	public bool TryAllocate( ulong lengthBytes, [NotNullWhen( true )] out BufferSegment? segment ) {
		segment = null;
		ulong nextCaret = _currentOffsetCaret + lengthBytes;
		while (nextCaret > LengthBytes) {
			if (_fragmented) {
				Defragment();
				nextCaret = _currentOffsetCaret + lengthBytes;
				continue;
			}
			if (_autoResizeMode == BufferAutoResizeMode.DISABLED)
				return false;
			AutoExtend();
		}
		segment = new( this, _currentOffsetCaret, lengthBytes );
		_segments.Add( segment );
		_currentOffsetCaret = nextCaret;
		return true;
	}

	private void AutoExtend() {
		switch (_autoResizeMode) {
			case BufferAutoResizeMode.LINEAR:
				Extend( _initialLengthBytes );
				break;
			case BufferAutoResizeMode.DOUBLE:
				Extend( LengthBytes );
				break;
		}
	}

	private void Defragment() {
		_currentOffsetCaret = 0;
		for (int i = 0; i < _segments.Count; i++) {
			BufferSegment segment = _segments[ i ];
			if (segment.OffsetBytes > _currentOffsetCaret) {
				InternalMove( segment.OffsetBytes, _currentOffsetCaret, segment.LengthBytes );
				segment.SetOffsetBytes( _currentOffsetCaret );
			}
			_currentOffsetCaret += _segments[ i ].LengthBytes;
		}
		_fragmented = false;
		this.LogLine( $"Defragmented!", Log.Level.VERBOSE );
	}

	internal void Free( BufferSegment segment ) {
		int index = _segments.IndexOf( segment );
		if (index == -1)
			return;
		_segments.RemoveAt( index );
		if (index == _segments.Count)
			_currentOffsetCaret = segment.OffsetBytes;
		else
			_fragmented = true;
	}

	internal new bool ReadRange<T>( Span<T> destination, ulong sourceOffsetBytes ) where T : unmanaged => base.ReadRange( destination, sourceOffsetBytes );
	internal new bool ReadRange( void* dstPtr, ulong dstLengthBytes, ulong sourceOffsetBytes ) => base.WriteRange( dstPtr, dstLengthBytes, sourceOffsetBytes );
	internal new bool WriteRange<T>( Span<T> source, ulong destinationOffsetBytes ) where T : unmanaged => base.WriteRange( source, destinationOffsetBytes );
	internal new bool WriteRange( void* srcPtr, ulong srcLengthBytes, ulong destinationOffsetBytes ) => base.WriteRange( srcPtr, srcLengthBytes, destinationOffsetBytes );
}
