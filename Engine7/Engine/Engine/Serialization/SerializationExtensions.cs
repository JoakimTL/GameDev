using System.Runtime.InteropServices;

namespace Engine.Serialization;

public static class SerializationExtensions {
	public static Type? GetSerializerForPayload( ReadOnlyMemory<byte> serializedData ) {
		if (serializedData.Length < 16)
			return null;
		Guid receivedGuid = MemoryMarshal.Read<Guid>( serializedData.Span[ ^16.. ] );
		return TypeManager.Serializers.GetSerializerType( receivedGuid );
	}
}