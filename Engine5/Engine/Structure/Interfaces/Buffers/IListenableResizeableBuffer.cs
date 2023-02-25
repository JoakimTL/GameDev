namespace Engine.Structure.Interfaces.Buffers;

public interface IListenableResizeableBuffer : IBuffer {
	public delegate void BufferResizeEvent( ulong newSizeBytes );
	event BufferResizeEvent? Resized;
}
