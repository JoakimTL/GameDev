namespace Engine.Data;

public interface ISerializableComponent {
	byte[]? Serialize();
	void SetFromSerializedData( byte[] data );
}