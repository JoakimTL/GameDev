using Engine.Buffers;
using Engine.Serialization;
using Engine.Transforms.Models;
using System.Runtime.InteropServices;

namespace Engine.Standard.Entities.Components.Serializers;

[Guid("0AEBD128-7466-43F5-88B2-9C2345D9CD10")]
public sealed class Transform2ComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<Transform2Component>( serializerProvider ) {
	protected override void PerformSerialization( ThreadedByteBuffer buffer, Transform2Component t ) {
		Span<byte> data = stackalloc byte[ 40 ];
		MemoryMarshal.Write( data, t.Transform.Data );
		buffer.AddRange( data );
	}

	protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, Transform2Component target ) {
		if (serializedData.Length < 40)
			return false;
		target.Transform.SetData( MemoryMarshal.Read<TransformData<Vector2<double>, double, Vector2<double>>>( serializedData ) );
		return true;
	}
}
