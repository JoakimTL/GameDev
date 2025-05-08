using AutoMapper;
using Engine;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civs.Logic.Nations.Serializers;

[Guid( "BB866BF9-FC3A-4553-9350-BF09D817F57B" )]
public sealed class PlayerComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<PlayerComponent>( serializerProvider ) {
	protected override void PerformSerialization( ThreadedByteBuffer buffer, PlayerComponent t ) {
		buffer.Add( t.MapColor );
		buffer.Add( t.DiscoveredFaces.Count );
		for ( uint i = 0; i < t.DiscoveredFaces.Count; i++) 
			buffer.Add( t.DiscoveredFaces.GetByte(i) );
		buffer.Add( t.RevealedFaces.Count );
		for (uint i = 0; i < t.RevealedFaces.Count; i++)
			buffer.Add( t.RevealedFaces.GetByte( i ) );
		buffer.Add( t.Name.Length * 2 );
		buffer.AddRange( MemoryMarshal.AsBytes( t.Name.AsSpan() ) );
	}

	protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, PlayerComponent target ) {
		if (serializedData.Length < 28)
			return false;
		int cursor = 0;
		{
			target.SetColor( MemoryMarshal.Read<Vector4<float>>( serializedData ) );
			cursor += 16;
		}
		{
			int discoveredFacesByteCount = MemoryMarshal.Read<int>( serializedData[ cursor.. ] );
			cursor += 4;
			if (serializedData.Length < cursor + discoveredFacesByteCount)
				return false;
			target.SetDiscoveredFaces( serializedData[ cursor..(cursor + discoveredFacesByteCount) ] );
			cursor += discoveredFacesByteCount;
		}
		{
			int revealedFacesByteCount = MemoryMarshal.Read<int>( serializedData[ cursor.. ] );
			cursor += 4;
			if (serializedData.Length < cursor + revealedFacesByteCount)
				return false;
			target.SetRevealedFaces( serializedData[ cursor..(cursor + revealedFacesByteCount) ] );
			cursor += revealedFacesByteCount;
		}
		{
			int nameLength = MemoryMarshal.Read<int>( serializedData[ cursor.. ] );
			cursor += 4;
			if (serializedData.Length < cursor + nameLength)
				return false;
			target.SetName( MemoryMarshal.Cast<byte, char>( serializedData[ cursor..(cursor + nameLength) ] ).ToString() );
			cursor += nameLength;
		}
		return true;
	}
}