namespace Engine.Structure.Interfaces.Buffers;

public interface IReadableBufferIndexable : IBuffer {
	/// <summary>
	/// Returns a section of the buffer as a readable memory segment. While the returned segment can be mutated, this read won't allow mutation.
	/// </summary>
	IIndexableReadOnlyBufferSegment<T> Read<T>( ulong offsetBytes, ulong lengthElements ) where T : unmanaged;
}
