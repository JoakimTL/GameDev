using System.Numerics;

namespace Engine.Structure.Interfaces.Buffers;

public interface ISegmentableBuffer<T> : IBuffer where T : IBufferSegment {
	T CreateSegment( ulong segmentSize );
}
