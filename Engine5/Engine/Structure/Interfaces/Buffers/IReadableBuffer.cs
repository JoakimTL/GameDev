namespace Engine.Structure.Interfaces.Buffers;

public interface IReadableBuffer : IBuffer {
	/// <summary>
	/// Returns one element from the buffer.
	/// </summary>
	T ReadOne<T>( ulong offsetBytes ) where T : unmanaged;
	/// <summary>
	/// Returns a copy of the selected segment. This is much slower than <see cref="Read{TData}(T, T)"/>, but also provide a mutable version of the segment. Mutating the segment will not affect the buffer data, as this is a copy.
	/// </summary>
	T[] Snapshot<T>( ulong offsetBytes, ulong lengthElements ) where T : unmanaged;
}
