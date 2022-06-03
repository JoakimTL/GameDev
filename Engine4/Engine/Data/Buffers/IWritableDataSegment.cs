namespace Engine.Data.Buffers;

public interface IWritableDataSegment : IDisposable, IDataSegmentInformation {
	public void Write<T>( ulong offsetBytes, T data ) where T : unmanaged;
	public void Write<T>( ulong offsetBytes, Span<T> data ) where T : unmanaged;
	public void Write<T>( ulong offsetBytes, ReadOnlySpan<T> data ) where T : unmanaged;
}
