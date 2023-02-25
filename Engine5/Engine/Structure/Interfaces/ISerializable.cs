namespace Engine.Structure.Interfaces;

public interface ISerializable {
	static abstract Guid TypeIdentity { get; }
	bool DeserializeData( byte[] data );
	byte[] SerializeData();
}
