namespace Engine.Structure.Interfaces.Buffers;

public interface IListenableBufferSegment : IBufferSegment {
	public delegate void BufferSegmentOffsetEvent( object segment, ulong newOffsetBytes );
	public event BufferSegmentOffsetEvent? OffsetChanged;
}
