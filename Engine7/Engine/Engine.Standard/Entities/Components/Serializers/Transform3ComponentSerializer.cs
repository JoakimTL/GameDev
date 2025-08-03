using Engine.Buffers;
using Engine.Serialization;
using Engine.Transforms.Models;
using System.Runtime.InteropServices;

namespace Engine.Standard.Entities.Components.Serializers;

[Guid("5C65BC20-D061-4D5E-8E95-F3ADBF5A6206")]
public sealed class Transform3ComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<Transform3Component>( serializerProvider ) {
	protected override void PerformSerialization( ThreadedByteBuffer buffer, Transform3Component t ) {
		Span<byte> data = stackalloc byte[ 80 ];
		MemoryMarshal.Write( data, t.Transform.Data );
		buffer.AddRange( data );
	}

	protected override void PerformDeserialization( ReadOnlySpan<byte> serializedData, Transform3Component target ) {
		if (serializedData.Length < 80)
			throw new ArgumentException( "Invalid serialized data", nameof( serializedData ) );
		target.Transform.SetData( MemoryMarshal.Read<TransformData<Vector3<double>, Rotor3<double>, Vector3<double>>>( serializedData ) );
	}

	protected override bool CanDeserializeCheck( ReadOnlySpan<byte> serializedData ) => true;
}
