using AutoMapper;
using Engine;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civs.Logic.Nations.Serializers;

[Guid( "BB866BF9-FC3A-4553-9350-BF09D817F57B" )]
public sealed class PlayerComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<PlayerComponent>( serializerProvider ) {
	protected override void PerformSerialization( ThreadedByteBuffer buffer, PlayerComponent t ) {
		Span<byte> data = stackalloc byte[ 16 ];
		MemoryMarshal.Write( data, t.MapColor );
		buffer.Add( data );
		data = stackalloc byte[ 4 ];
		MemoryMarshal.Write( data, t.DiscoveredFaces.Count );
		buffer.Add( data );
		foreach (uint face in t.DiscoveredFaces) {
			MemoryMarshal.Write( data, face );
			buffer.Add( data );
		}
		MemoryMarshal.Write( data, t.RevealedFaces.Count );
		buffer.Add( data );
		foreach (uint face in t.RevealedFaces) {
			MemoryMarshal.Write( data, face );
			buffer.Add( data );
		}
		MemoryMarshal.Write( data, t.Name.Length * 2 );
		buffer.Add( MemoryMarshal.AsBytes( t.Name.AsSpan() ) );
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
			int discoveredFacesCount = MemoryMarshal.Read<int>( serializedData[ cursor.. ] );
			cursor += 4;
			if (serializedData.Length < cursor + discoveredFacesCount * 4)
				return false;
			ReadOnlySpan<uint> discoveredFaces = MemoryMarshal.Cast<byte, uint>( serializedData[ cursor..(cursor + discoveredFacesCount * 4) ] );
			target.SetDiscoveredFaces( discoveredFaces );
			cursor += discoveredFacesCount * 4;
		}
		{
			int revealedFacesCount = MemoryMarshal.Read<int>( serializedData[ cursor.. ] );
			cursor += 4;
			if (serializedData.Length < cursor + revealedFacesCount * 4)
				return false;
			ReadOnlySpan<uint> revealedFaces = MemoryMarshal.Cast<byte, uint>( serializedData[ cursor..(cursor + revealedFacesCount * 4) ] );
			target.SetRevealedFaces( revealedFaces );
			cursor += revealedFacesCount * 4;
		}
		return true;
	}
}