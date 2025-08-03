using Engine.Buffers;

namespace Engine.Serialization;

public interface ISerializer {
	public Guid Guid { get; }
	public Type Target { get; }
	int SerializeInto( ThreadedByteBuffer buffer, object t );
	void DeserializeInto( ReadOnlyMemory<byte> serializedData, object t );
	void DeserializeInto( ReadOnlySpan<byte> serializedData, object t );
	bool CanDeserialize( ReadOnlySpan<byte> serializedData );
}
