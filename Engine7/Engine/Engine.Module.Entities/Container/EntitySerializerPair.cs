using Engine.Serialization;

namespace Engine.Module.Entities.Container;

public sealed class EntitySerializerPair( Entity entity, SerializerProvider serializerProvider ) {
	public Entity Entity { get; } = entity;
	public SerializerProvider SerializerProvider { get; } = serializerProvider;
}