namespace Engine.Structure.Interfaces;

public interface IReadableBuffer : IBuffer
{
    /// <summary>
    /// Returns a section of the buffer as a readable memory segment. While the returned segment can be mutated, this read won't allow mutation.
    /// </summary>
    ReadOnlyMemory<T> Read<T>(ulong offsetBytes, ulong length) where T : unmanaged;
    /// <summary>
    /// Returns a copy of the selected segment. This is much slower than <see cref="Read{T}(ulong, ulong)"/>, but also provide a mutable version of the segment. Mutating the segment will not affect the buffer data, as this is a copy.
    /// </summary>
    T[] Snapshot<T>(ulong offsetBytes, ulong length) where T : unmanaged;
}
