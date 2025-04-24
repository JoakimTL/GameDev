using Engine.Buffers;

namespace Engine.Serialization;

public interface ISerializer {
	public Guid Guid { get; }
	public Type Target { get; }
	int SerializeInto( ThreadedByteBuffer buffer, object t );
	bool DeserializeInto( ReadOnlyMemory<byte> serializedData, object t );
	bool DeserializeInto( ReadOnlySpan<byte> serializedData, object t );
}
