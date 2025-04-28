using Civs.World.NewWorld;
using Engine;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civs.Logic.Nations.Serializers;

[Guid( "0DCD4B16-4A22-4ED3-9EC6-5715284CCC51" )]
public sealed class FaceOwnershipRenderComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<FaceOwnershipRenderComponent>( serializerProvider ) {
	protected override void PerformSerialization( ThreadedByteBuffer buffer, FaceOwnershipRenderComponent t ) {
		Span<byte> data = stackalloc byte[ 16 ];
		MemoryMarshal.Write( data, t.Color );
		buffer.Add( data );
	}
	protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, FaceOwnershipRenderComponent target ) {
		if (serializedData.Length < 16)
			return false;
		target.SetColor( MemoryMarshal.Read<Vector4<float>>( serializedData ) );
		return true;
	}
}

[Guid("38E609AB-A7AE-463F-A8E8-980CCA6ECF38")]
public sealed class FaceOwnershipComponentSerializer( SerializerProvider serializerProvider, ActiveGlobeTrackingService activeGlobeTrackingService ) : SerializerBase<FaceOwnershipComponent>( serializerProvider ) {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	protected override void PerformSerialization( ThreadedByteBuffer buffer, FaceOwnershipComponent t ) {
		Span<byte> data = stackalloc byte[ t.OwnedFaces.Count * sizeof(uint) ];
		int i = 0;
		foreach (Face face in t.OwnedFaces) {
			MemoryMarshal.Write( data.Slice( i * sizeof( uint ), 4 ), face.Id );
			i++;
		}
		buffer.Add( data );
	}
	protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, FaceOwnershipComponent target ) {
		if (_activeGlobeTrackingService.CurrentGlobe is null)
			throw new InvalidOperationException( "No active globe available." );
		target.ClearOwnership();
		for (int i = 0; i < serializedData.Length / sizeof( uint ); i++) {
			int faceId = (int) MemoryMarshal.Read<uint>( serializedData.Slice( i * sizeof( uint ), 4 ) );
			Face face = _activeGlobeTrackingService.CurrentGlobe.Faces[ faceId ];
			target.AddFace( face );
		}
		return true;
	}
}
