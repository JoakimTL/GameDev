using System.Numerics;

namespace Engine.Structure.Interfaces;

public interface IWriteableBuffer<T> : IBuffer<T> where T : IBinaryInteger<T> {
	public delegate void BufferWrittenEvent( ulong offsetBytes, ulong lengthBytes );
	event BufferWrittenEvent? Written;
	void Write<TData>( T offsetBytes, IEnumerable<TData> data ) where TData : unmanaged;
	void Write<TData>( T offsetBytes, ReadOnlyMemory<TData> data ) where TData : unmanaged;
	void Write<TData>( T offsetBytes, ReadOnlySpan<TData> data ) where TData : unmanaged;
	void Write<TData>( T offsetBytes, ref TData data ) where TData : unmanaged;
	void Write<TData>( T offsetBytes, TData data ) where TData : unmanaged => Write( offsetBytes, ref data );
	unsafe void Write( T offsetBytes, void* data, T sizeBytes );
}
