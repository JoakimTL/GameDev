using System.Numerics;

namespace Engine.Structure.Interfaces;

public interface IReadableBuffer<T> : IBuffer<T> where T : IBinaryInteger<T> {
	/// <summary>
	/// Returns a section of the buffer as a readable memory segment. While the returned segment can be mutated, this read won't allow mutation.
	/// </summary>
	ReadOnlyMemory<TData> Read<TData>( T offsetBytes, T length ) where TData : unmanaged;
	/// <summary>
	/// Returns a copy of the selected segment. This is much slower than <see cref="Read{TData}(T, T)"/>, but also provide a mutable version of the segment. Mutating the segment will not affect the buffer data, as this is a copy.
	/// </summary>
	TData[] Snapshot<TData>( T offsetBytes, T length ) where TData : unmanaged;
}
