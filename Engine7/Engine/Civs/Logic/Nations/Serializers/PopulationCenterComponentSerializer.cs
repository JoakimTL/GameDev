using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civs.Logic.Nations.Serializers;

[Guid("55E94D12-5E82-4858-B26C-CEC621F02A86")]
public sealed class PopulationCenterComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<PopulationCenterComponent>( serializerProvider ) {
	protected override void PerformSerialization( ThreadedByteBuffer buffer, PopulationCenterComponent t ) {
		// No data to serialize
	}
	protected override void PerformDeserialization( ReadOnlySpan<byte> serializedData, PopulationCenterComponent target ) {
		// No data to deserialize
	}

	protected override bool CanDeserializeCheck( ReadOnlySpan<byte> serializedData ) => true;
}
