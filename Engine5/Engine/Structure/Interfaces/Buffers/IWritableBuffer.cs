namespace Engine.Structure.Interfaces.Buffers;

public interface IWritableBuffer : IBuffer {
	/// <returns>True if write was successful</returns>
	bool Write<T>( ulong offsetBytes, T[] data ) where T : unmanaged;
	/// <returns>True if write was successful</returns>
	bool Write<T>( ulong offsetBytes, ReadOnlyMemory<T> data ) where T : unmanaged;
	/// <returns>True if write was successful</returns>
	bool Write<T>( ulong offsetBytes, ReadOnlySpan<T> data ) where T : unmanaged;
	/// <returns>True if write was successful</returns>
	bool Write<T>( ulong offsetBytes, T data ) where T : unmanaged;
	/// <returns>True if write was successful</returns>
	unsafe bool Write( ulong offsetBytes, void* data, ulong sizeBytes );
}
