using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civlike.Logic.Nations.ECS.Serializers;

[Guid( "55E94D12-5E82-4858-B26C-CEC621F02A86" )]
public sealed class PopulationCenterComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<PopulationCenterComponent>( serializerProvider ) {
	protected override void PerformSerialization( ThreadedByteBuffer buffer, PopulationCenterComponent t ) {
		buffer.AddRange( MemoryMarshal.Cast<char, byte>( t.Name.AsSpan() ) );
	}

	protected override void PerformDeserialization( ReadOnlySpan<byte> serializedData, PopulationCenterComponent target ) {
		target.SetName( MemoryMarshal.Cast<byte, char>( serializedData ).ToString() );
	}

	protected override bool CanDeserializeCheck( ReadOnlySpan<byte> serializedData ) => true;
}
