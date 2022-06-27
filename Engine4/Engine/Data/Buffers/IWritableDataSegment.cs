namespace Engine.Data.Buffers;

public interface IWritableDataSegment : IDisposable, IDataSegmentInformation {
	void Write<T>( ulong offsetBytes, T data ) where T : unmanaged;
	unsafe void Write<T>( ulong offsetBytes, T* data, uint elementCount ) where T : unmanaged;
	void Write<T>( ulong offsetBytes, Span<T> data ) where T : unmanaged;
	void Write<T>( ulong offsetBytes, ReadOnlySpan<T> data ) where T : unmanaged;
	void Write<T>( ulong offsetBytes, Memory<T> data ) where T : unmanaged;
	void Write<T>( ulong offsetBytes, ReadOnlyMemory<T> data ) where T : unmanaged;
}
