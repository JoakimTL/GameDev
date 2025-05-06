using Engine;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civs.Logic.Nations.Serializers;

[Guid("BB866BF9-FC3A-4553-9350-BF09D817F57B")]
public sealed class PlayerComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<PlayerComponent>( serializerProvider ) {
	protected override void PerformSerialization( ThreadedByteBuffer buffer, PlayerComponent t ) {
		Span<byte> data = stackalloc byte[ 16 ];
		MemoryMarshal.Write( data, t.MapColor );
		buffer.Add( data );
	}
	protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, PlayerComponent target ) {
		if (serializedData.Length < 16)
			return false;
		target.SetColor( MemoryMarshal.Read<Vector4<float>>( serializedData ) );
		return true;
	}
}