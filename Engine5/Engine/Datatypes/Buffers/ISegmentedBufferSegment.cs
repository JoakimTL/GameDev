using Engine.Structure.Interfaces.Buffers;

namespace Engine.Datatypes.Buffers;

public interface ISegmentedBufferSegment : IBufferSegment, IReadableBuffer, IWritableBuffer, IListenableBufferSegment, IDisposable { }
