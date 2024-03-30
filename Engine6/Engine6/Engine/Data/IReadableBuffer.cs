namespace Engine.Data;

public interface IReadableBuffer : IDisposable {
	bool TryRead<T>( uint offsetByte, out T result ) where T : unmanaged;
	bool TryRead<T>( uint offsetByte, Span<T> resultStorage ) where T : unmanaged;
}
