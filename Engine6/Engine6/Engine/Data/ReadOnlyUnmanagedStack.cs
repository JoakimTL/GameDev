using System.Runtime.CompilerServices;

namespace Engine.Data;

/// <summary>
/// A stack allowing for allocation of then read only data. When all segments are disposed the stack is automatically flushed and can be reused without reallocation.
/// </summary>
public unsafe sealed class ReadOnlyUnmanagedStack( uint sizeBytes ) : UnmanagedData( sizeBytes ) {

	private readonly List<ReadOnlyUnmanagedStackSegment> _livingSegments = [];
	public nuint BytesUsed { get; private set; } = 0;
	private bool _flushing;

	/// <summary>
	/// Attempts to allocate a segment with the specified data. Returns false if there is not enough space.
	/// </summary>
	public bool TryAllocate<T>( Span<T> data, [System.Diagnostics.CodeAnalysis.NotNullWhen( true )] out IReadableSegment? segment ) where T : unmanaged {
		segment = null;
		if (Disposed)
			return this.LogWarningThenReturn( "Already disposed.", false );

		uint dataSizeBytes = (uint) data.Length * (uint) Unsafe.SizeOf<T>();
		if (dataSizeBytes + BytesUsed > SizeBytes)
			return false;

		fixed (void* srcPtr = data)
			Unsafe.CopyBlock( (byte*) Pointer + BytesUsed, srcPtr, dataSizeBytes );

		ReadOnlyUnmanagedStackSegment newSegment = new( this, dataSizeBytes, BytesUsed );
		BytesUsed += dataSizeBytes;
		newSegment.OnDispose += OnSegmentDispose;
		_livingSegments.Add( newSegment );
		segment = newSegment;
		return true;
	}

	private void OnSegmentDispose( ReadOnlyUnmanagedStackSegment segment ) {
		if (Disposed)
			return;
		segment.OnDispose -= OnSegmentDispose;
		if (_flushing)
			return;
		_livingSegments.Remove( segment );
		if (_livingSegments.Count == 0)
			Flush();
	}

	public void Flush() {
		if (_flushing)
			return;
		_flushing = true;
		foreach (ReadOnlyUnmanagedStackSegment segment in _livingSegments)
			segment.Dispose();
		_livingSegments.Clear();
		BytesUsed = 0;
		_flushing = false;
	}

	protected override void OnDispose() {
		Flush();
	}

}
