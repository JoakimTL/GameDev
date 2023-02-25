namespace Engine.Structure.Interfaces.Buffers;

public interface IListenableWriteableBuffer : IBuffer {
	public delegate void BufferWrittenEvent( ulong offsetBytes, ulong lengthBytes );
	event BufferWrittenEvent? Written;
}
