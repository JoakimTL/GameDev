using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Engine.Module.Entities.Render.Components;

[Guid("6E22035B-CA17-471B-806E-AD926A780940")]
public sealed class RenderComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<RenderComponent>( serializerProvider ) {
	protected override void PerformDeserialization( ReadOnlySpan<byte> serializedData, RenderComponent target ) { }
	protected override void PerformSerialization( ThreadedByteBuffer buffer, RenderComponent t ) { }
	protected override bool CanDeserializeCheck( ReadOnlySpan<byte> serializedData ) => true;
}