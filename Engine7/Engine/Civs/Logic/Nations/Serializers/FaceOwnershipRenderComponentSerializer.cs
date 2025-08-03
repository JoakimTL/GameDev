using Civs.World;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civs.Logic.Nations.Serializers;

[Guid("38E609AB-A7AE-463F-A8E8-980CCA6ECF38")]
public sealed class TileOwnershipComponentSerializer( SerializerProvider serializerProvider, ActiveGlobeTrackingService activeGlobeTrackingService ) : SerializerBase<TileOwnershipComponent>( serializerProvider ) {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	protected override void PerformSerialization( ThreadedByteBuffer buffer, TileOwnershipComponent t ) {
		Span<byte> data = stackalloc byte[ t.OwnedFaces.Count * sizeof(uint) ];
		int i = 0;
		foreach (Face face in t.OwnedFaces) {
			MemoryMarshal.Write( data.Slice( i * sizeof( uint ), sizeof( uint ) ), face.Id );
			i++;
		}
		buffer.AddRange( data );
	}
	protected override void PerformDeserialization( ReadOnlySpan<byte> serializedData, TileOwnershipComponent target ) {
		if (_activeGlobeTrackingService.CurrentGlobe is null)
			throw new InvalidOperationException( "No active globe available." );
		target.ClearOwnership();
		for (int i = 0; i < serializedData.Length / sizeof( uint ); i++) {
			int faceId = (int) MemoryMarshal.Read<uint>( serializedData.Slice( i * sizeof( uint ), sizeof( uint ) ) );
			Face face = _activeGlobeTrackingService.CurrentGlobe.Faces[ faceId ];
			target.AddFace( face );
		}
	}

	protected override bool CanDeserializeCheck( ReadOnlySpan<byte> serializedData ) {
		return true;
	}
}
