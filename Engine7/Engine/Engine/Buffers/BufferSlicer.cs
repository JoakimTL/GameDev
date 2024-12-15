using System.Diagnostics.CodeAnalysis;

namespace Engine.Buffers;

/// <summary>
/// Allows you to cut up any buffer into smaller segments. Unlike <see cref="SegmentedSystemBuffer"/> the buffer is always contiguous.
/// </summary>
/// <typeparam name="TBuffer"></typeparam>
public sealed class BufferSlicer<TBuffer> where TBuffer : IBuffer<ulong>, IReadableBuffer<ulong>, IWritableBuffer<ulong>, IListenableDisposable {
	private readonly TBuffer _hostBuffer;
	private readonly ulong _sliceSizeBytes;

	/// <summary>
	/// Null if the host buffer does not implement <see cref="ICopyableBuffer{ulong}"/>.
	/// </summary>
	private readonly ICopyableBuffer<ulong>? _copyableHostBuffer;
	private readonly BufferSlice<TBuffer>[] _slices;
	private int _currentUnoccupiedSliceIndex = 0;

	public BufferSlicer( TBuffer hostBuffer, ulong sliceSizeBytes ) {
		if (hostBuffer.LengthBytes % sliceSizeBytes != 0)
			throw new ArgumentException( "The host buffer length must be a multiple of the slice size." );
		this._hostBuffer = hostBuffer;
		this._sliceSizeBytes = sliceSizeBytes;
		this._copyableHostBuffer = hostBuffer as ICopyableBuffer<ulong>;
		this._slices = new BufferSlice<TBuffer>[ hostBuffer.LengthBytes / sliceSizeBytes ];
		for (int i = 0; i < this._slices.Length; i++) {
			this._slices[ i ] = new( hostBuffer, (ulong) i * sliceSizeBytes, sliceSizeBytes, i );
			this._slices[ i ].OnFreed += OnSliceFreed;

		}
	}

	public uint Count => (uint) this._slices.Length;

	public bool TryAllocate( ulong lengthBytes, [NotNullWhen( true )] out BufferSlice<TBuffer>? slice ) {
		slice = null;
		if (_currentUnoccupiedSliceIndex == _slices.Length)
			return false;

		slice = _slices[ _currentUnoccupiedSliceIndex ];
		slice.Occupied = true;
		_currentUnoccupiedSliceIndex++;
		return true;
	}

	private void OnSliceFreed( int sliceIndex ) {
		BufferSlice<TBuffer> freedSlice = this._slices[ sliceIndex ];
		freedSlice.Occupied = false;

		for (int i = sliceIndex; i < _currentUnoccupiedSliceIndex - 1; i++) {
			this._slices[ i ] = this._slices[ i + 1 ];
			this._slices[ i ].Index = i;
			this._slices[ i ].SetOffsetBytes( _sliceSizeBytes * (uint) i );
		}

		this._slices[ _currentUnoccupiedSliceIndex - 1 ] = freedSlice;
		freedSlice.Index = _currentUnoccupiedSliceIndex - 1;
		freedSlice.SetOffsetBytes( _sliceSizeBytes * (uint) freedSlice.Index );

		_currentUnoccupiedSliceIndex--;

		if (_currentUnoccupiedSliceIndex == sliceIndex)
			return;

		ulong moveSrcOffsetBytes = (uint) (sliceIndex + 1) * _sliceSizeBytes;
		ulong moveDstOffsetBytes = moveSrcOffsetBytes - _sliceSizeBytes;
		ulong moveLengthBytes = (ulong) (_currentUnoccupiedSliceIndex - sliceIndex) * _sliceSizeBytes;
		if (this._copyableHostBuffer is not null) {
			this._copyableHostBuffer.CopyTo( this._hostBuffer, moveSrcOffsetBytes, moveDstOffsetBytes, moveLengthBytes );
		} else {
			this._hostBuffer.CopyTo( this._hostBuffer, moveSrcOffsetBytes, moveDstOffsetBytes, (int) moveLengthBytes );
		}
	}
}
