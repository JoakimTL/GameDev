namespace Engine.Structure.Interfaces;

public interface IWriteableBuffer : IBuffer
{
    public delegate void BufferWrittenEvent(ulong offsetBytes, ulong lengthBytes);
    event BufferWrittenEvent Written;
    void Write<T>(ulong offsetBytes, IReadOnlyList<T> data) where T : unmanaged;
    void Write<T>(ulong offsetBytes, T[] data) where T : unmanaged;
    void Write<T>(ulong offsetBytes, Span<T> data) where T : unmanaged;
    void Write<T>(ulong offsetBytes, ReadOnlySpan<T> data) where T : unmanaged;
    void Write<T>(ulong offsetBytes, ref T data) where T : unmanaged;
    void Write<T>(ulong offsetBytes, T data) where T : unmanaged;
    unsafe void Write(ulong offsetBytes, void* data);
}
