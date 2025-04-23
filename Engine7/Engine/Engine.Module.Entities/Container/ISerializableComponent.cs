using Engine.Buffers;

namespace Engine.Module.Entities.Container;

public interface ISerializableComponent {
	void Serialize( ThreadedByteBuffer buffer );
	void Deserialize( Span<byte> reader );
}