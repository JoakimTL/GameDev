namespace Engine.Data.Buffers;

public interface IReadableDataSegment : IDataSegmentInformation {
	T Read<T>( ulong offsetBytes ) where T : unmanaged;
	Memory<T> Read<T>( ulong offsetBytes, uint elementCount ) where T : unmanaged;
}
