namespace Engine.Structure.Interfaces;

public interface ICustomizedSerializable
{
    static abstract Guid SerializationIdentity { get; }
    bool ShouldSerialize { get; }
    bool DeserializeData( byte[] data );
	byte[] SerializeData();
}
