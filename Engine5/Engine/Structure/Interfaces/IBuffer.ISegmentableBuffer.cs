using System.Numerics;

namespace Engine.Structure.Interfaces;

public interface ISegmentableBuffer<T> : IBuffer<T> where T : IBinaryInteger<T> {
	IBufferSegment<T> CreateSegment( T segmentSize );
}
