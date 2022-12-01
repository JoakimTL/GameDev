using System.Numerics;

namespace Engine.Structure.Interfaces;

public interface IExtendableBuffer<T> : IBuffer<T> where T : IBinaryInteger<T> {
	void Extend( T bytes );
}