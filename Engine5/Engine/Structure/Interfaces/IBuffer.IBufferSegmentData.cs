using System.Numerics;

namespace Engine.Structure.Interfaces;

public interface IBufferSegmentData<T> : IBuffer<T> where T : IBinaryInteger<T> {
	T OffsetBytes { get; }
	public event Action<T>? OffsetChanged;
}
