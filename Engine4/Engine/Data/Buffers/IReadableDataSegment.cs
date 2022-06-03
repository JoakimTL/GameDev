namespace Engine.Data.Buffers;

public interface IReadableDataSegment : IDataSegmentInformation {
	public T Read<T>( ulong offsetBytes ) where T : unmanaged;
	public ReadOnlySpan<T> Read<T>( ulong offsetBytes, uint elementCount ) where T : unmanaged;

}
