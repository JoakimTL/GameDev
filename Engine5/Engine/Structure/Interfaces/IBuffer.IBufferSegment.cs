using System.Numerics;

namespace Engine.Structure.Interfaces;

public interface IBufferSegment<T> : IBufferSegmentData<T>, IWriteableBuffer<T>, IReadableBuffer<T> where T : IBinaryInteger<T> { }
