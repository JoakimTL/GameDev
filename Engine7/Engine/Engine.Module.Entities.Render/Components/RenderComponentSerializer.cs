using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Engine.Module.Render.Entities.Components;

[Guid("6E22035B-CA17-471B-806E-AD926A780940")]
public sealed class RenderComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<RenderComponent>( serializerProvider ) {
	protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, RenderComponent target ) => true;
	protected override void PerformSerialization( ThreadedByteBuffer buffer, RenderComponent t ) { }
}