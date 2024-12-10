using System.Diagnostics.CodeAnalysis;

namespace Engine.Buffers;

/// <summary>
/// Allows you to cut up any buffer into smaller segments. Unlike <see cref="SegmentedSystemBuffer"/> the buffer is always contiguous.
/// </summary>
/// <typeparam name="TBuffer"></typeparam>
public sealed class SubBufferManager<TBuffer> where TBuffer : IBuffer<ulong>, IReadableBuffer<ulong>, IWritableBuffer<ulong>, IListenableDisposable {
	private readonly TBuffer _hostBuffer;
	/// <summary>
	/// Null if the host buffer does not implement <see cref="ICopyableBuffer{ulong}"/>.
	/// </summary>
	private readonly ICopyableBuffer<ulong>? _copyableHostBuffer;
	private readonly List<SubBuffer<TBuffer>> _subBuffers = [];
	private ulong _currentOffsetCaret = 0;

	public SubBufferManager( TBuffer hostBuffer ) {
		this._hostBuffer = hostBuffer;
		this._copyableHostBuffer = hostBuffer as ICopyableBuffer<ulong>;
		hostBuffer.OnDisposed += OnHostBufferDisposed;
	}

	private void OnHostBufferDisposed( IListenableDisposable disposable ) {
		_subBuffers.Clear();
	}

	public uint Count => (uint) this._subBuffers.Count;

	public bool TryAllocate( ulong lengthBytes, [NotNullWhen( true )] out SubBuffer<TBuffer>? subBuffer ) {
		subBuffer = null;
		if (_hostBuffer.Disposed)
			return false;
		if (this._currentOffsetCaret + lengthBytes > this._hostBuffer.LengthBytes)
			return false;
		subBuffer = new( this._hostBuffer, this._currentOffsetCaret, lengthBytes );
		this._currentOffsetCaret += lengthBytes;
		this._subBuffers.Add( subBuffer );
		return true;
	}

	public void Remove( SubBuffer<TBuffer> subBuffer ) {
		if (_hostBuffer.Disposed)
			return;
		int indexOf = this._subBuffers.IndexOf( subBuffer );
		this._subBuffers.RemoveAt( indexOf );
		this._currentOffsetCaret -= subBuffer.LengthBytes;
		if (indexOf == this._subBuffers.Count)
			return;
		ulong moveStart = this._subBuffers[ indexOf ].OffsetBytes;
		ulong moveLength = this._currentOffsetCaret;
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
