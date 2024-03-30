using System.Runtime.CompilerServices;

namespace Engine.Data;

public sealed class ReadOnlyUnmanagedStackSegment( ReadOnlyUnmanagedStack stack, uint sizeBytes, nuint offsetBytes ) : Identifiable, IDisposable, IReadableSegment {

	private ReadOnlyUnmanagedStack? _stack = stack;
	public uint SizeBytes { get; } = sizeBytes;
	public nuint OffsetBytes { get; } = offsetBytes;

	public event Action<ReadOnlyUnmanagedStackSegment>? OnDispose;

	public bool TryRead<T>( uint offsetByte, out T result ) where T : unmanaged {
		result = default;
		if (_stack is null)
			return this.LogLineThenReturn( $"Segment already disposed.", Log.Level.NORMAL, false );
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if (offsetByte + typeSizeBytes > this.SizeBytes)
			return this.LogLineThenReturn( $"Attempting to access data outside segment!", Log.Level.NORMAL, false );
		result = _stack.Read<T>( offsetByte );
		return true;
	}

	public bool TryRead<T>( uint offsetByte, Span<T> resultStorage ) where T : unmanaged {
		if (_stack is null)
			return this.LogLineThenReturn( $"Segment already disposed.", Log.Level.NORMAL, false );
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		uint requestedSizeBytes = (uint) resultStorage.Length * typeSizeBytes;
		if (offsetByte + requestedSizeBytes > this.SizeBytes)
			return this.LogLineThenReturn( $"Attempting to access data outside segment!", Log.Level.NORMAL, false );
		return _stack.TryRead( offsetByte, resultStorage );
	}

	public void Dispose() {
		if (_stack is null)
			return;
		this.OnDispose?.Invoke( this );
		this.OnDispose = null;
		_stack = null;
	}
}
