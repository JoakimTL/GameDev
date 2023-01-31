using System.Numerics;

namespace Engine.Structure.Interfaces.Buffers;

public interface IResizeableBuffer : IBuffer {
	void Resize( ulong newSizeBytes );
}