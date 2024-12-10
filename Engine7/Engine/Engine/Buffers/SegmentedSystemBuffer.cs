using Engine.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Buffers;

public unsafe class SegmentedSystemBuffer : SystemBufferBase<ulong>, ICopyableBuffer<ulong> {
	private readonly List<BufferSegment> _segments = [];
	private readonly ulong _initialLengthBytes;
	private ulong _currentOffsetCaret;
	private bool _fragmented;
	private readonly BufferAutoResizeMode _autoResizeMode;

	public SegmentedSystemBuffer( ulong initialLengthBytes, BufferAutoResizeMode autoResizeMode ) : base( initialLengthBytes ) {
		this._initialLengthBytes = initialLengthBytes;
		this._autoResizeMode = autoResizeMode;
		OnDisposed += OnSelfDisposal;
	}

	private void OnSelfDisposal( IListenableDisposable disposable ) {
		foreach (BufferSegment segment in this._segments)
			segment.Dispose();
		this._segments.Clear();
	}

	public bool TryAllocate( ulong lengthBytes, [NotNullWhen( true )] out BufferSegment? segment ) {
		ObjectDisposedException.ThrowIf( Disposed, this );
		segment = null;
		ulong nextCaret = this._currentOffsetCaret + lengthBytes;
		while (nextCaret > this.LengthBytes) {
			if (this._fragmented) {
				Defragment();
				nextCaret = this._currentOffsetCaret + lengthBytes;
				continue;
			}
			if (this._autoResizeMode == BufferAutoResizeMode.DISABLED)
				return false;
			AutoExtend();
		}
		segment = new( this, this._currentOffsetCaret, lengthBytes );
		this._segments.Add( segment );
		this._currentOffsetCaret = nextCaret;
		return true;
	}

	private void AutoExtend() {
		switch (this._autoResizeMode) {
			case BufferAutoResizeMode.LINEAR:
				Extend( this._initialLengthBytes );
				break;
			case BufferAutoResizeMode.DOUBLE:
				Extend( this.LengthBytes );
				break;
		}
	}

	private void Defragment() {
		this._currentOffsetCaret = 0;
		for (int i = 0; i < this._segments.Count; i++) {
			BufferSegment segment = this._segments[ i ];
			if (segment.OffsetBytes > this._currentOffsetCaret) {
				InternalMove( segment.OffsetBytes, this._currentOffsetCaret, segment.LengthBytes );
				segment.SetOffsetBytes( this._currentOffsetCaret );
			}
			this._currentOffsetCaret += this._segments[ i ].LengthBytes;
		}
		this._fragmented = false;
		this.LogLine( $"Defragmented!", Log.Level.VERBOSE );
	}

	internal void Free( BufferSegment segment ) {
		if (Disposed)
			return;
		int index = this._segments.IndexOf( segment );
		if (index == -1)
			return;
		this._segments.RemoveAt( index );
		if (index == this._segments.Count)
			this._currentOffsetCaret = segment.OffsetBytes;
		else
			this._fragmented = true;
	}

	internal new bool ReadRange<T>( Span<T> destination, ulong sourceOffsetBytes ) where T : unmanaged => base.ReadRange( destination, sourceOffsetBytes );
	internal new bool ReadRange( void* dstPtr, ulong dstLengthBytes, ulong sourceOffsetBytes ) => base.ReadRange( dstPtr, dstLengthBytes, sourceOffsetBytes );
	internal new bool WriteRange<T>( Span<T> source, ulong destinationOffsetBytes ) where T : unmanaged => base.WriteRange( source, destinationOffsetBytes );
	internal new bool WriteRange( void* srcPtr, ulong srcLengthBytes, ulong destinationOffsetBytes ) => base.WriteRange( srcPtr, srcLengthBytes, destinationOffsetBytes );

	public new bool CopyTo<TRecipientScalar>( IWritableBuffer<TRecipientScalar> recipient, ulong srcOffsetBytes, TRecipientScalar dstOffsetBytes, TRecipientScalar bytesToCopy )
		where TRecipientScalar : unmanaged, IBinaryInteger<TRecipientScalar>, IUnsignedNumber<TRecipientScalar>
		=> base.CopyTo( recipient, srcOffsetBytes, dstOffsetBytes, bytesToCopy );

	public new bool Overwrite<TRecipientScalar>( IWriteResizableBuffer<TRecipientScalar> recipient, ulong srcOffsetBytes, TRecipientScalar bytesToCopy )
		where TRecipientScalar : unmanaged, IBinaryInteger<TRecipientScalar>, IUnsignedNumber<TRecipientScalar>
		=> base.Overwrite( recipient, srcOffsetBytes, bytesToCopy );

#if DEBUG
	internal Memory<byte> GetDebugSlice( BufferSegment bufferSegment ) => base.GetDebugSlice( nint.CreateSaturating( bufferSegment.OffsetBytes ), nint.CreateSaturating( bufferSegment.LengthBytes ) );
#endif
}