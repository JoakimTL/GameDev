using System.Numerics;

namespace Engine.Structure.Interfaces.Buffers;

public interface IIndexableReadOnlyBufferSegment<T> : IIndexableReadOnlyBuffer<T>, IBufferSegment where T : unmanaged { }