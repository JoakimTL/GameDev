using System.Numerics;

namespace Engine.Structure.Interfaces;

public interface IBuffer<T> where T : IBinaryInteger<T> {
	T SizeBytes { get; }
}
