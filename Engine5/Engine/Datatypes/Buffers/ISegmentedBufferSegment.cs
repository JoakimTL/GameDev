using Engine.Structure.Interfaces.Buffers;
using System.Numerics;

namespace Engine.Datatypes.Buffers;

public interface ISegmentedBufferSegment : IBufferSegment, IReadableBuffer, IWritableBuffer, IListenableBufferSegment, IDisposable { }
