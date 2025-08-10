using Civlike.World;
using Civlike.World.State;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civlike.Logic.Nations.ECS.Serializers;

[Guid("38E609AB-A7AE-463F-A8E8-980CCA6ECF38")]
public sealed class FaceOwnershipComponentSerializer( SerializerProvider serializerProvider, GlobeStoreService globeStore ) : SerializerBase<FaceOwnershipComponent>( serializerProvider ) {
	private readonly GlobeStoreService _globeStore = globeStore;

	protected override void PerformSerialization( ThreadedByteBuffer buffer, FaceOwnershipComponent t ) {
		int guidSizeBytes = Marshal.SizeOf<Guid>();
		Span<byte> data = stackalloc byte[ t.OwnedTiles.Count * sizeof(uint) + guidSizeBytes ];
		MemoryMarshal.Write( data[ ..guidSizeBytes ], t.GlobeId );
		int i = 0;
		foreach (Tile tile in t.OwnedTiles) {
			MemoryMarshal.Write( data.Slice( i * sizeof( uint ) + guidSizeBytes, sizeof( uint ) ), tile.Id );
			i++;
		}
		buffer.AddRange( data );
	}

	protected override void PerformDeserialization( ReadOnlySpan<byte> serializedData, FaceOwnershipComponent target ) {
		int guidSizeBytes = Marshal.SizeOf<Guid>();
		Guid globeId = MemoryMarshal.Read<Guid>( serializedData[ ..guidSizeBytes ] );
		Globe globe = _globeStore.CurrentGlobe ?? throw new InvalidOperationException( $"Globe not found." );
		if (globe.Id != globeId)
			throw new InvalidOperationException( $"Globe with id {globeId} not found." );

		target.ClearOwnership();
		for (int i = 0; i < serializedData.Length / sizeof( uint ); i++) {
			int tileId = (int) MemoryMarshal.Read<uint>( serializedData.Slice( i * sizeof( uint ), sizeof( uint ) ) );
			Tile tile = globe.Tiles[ tileId ];
			target.AddFace( tile );
		}
	}

	protected override bool CanDeserializeCheck( ReadOnlySpan<byte> serializedData ) {
		int guidSizeBytes = Marshal.SizeOf<Guid>();
		Guid globeId = MemoryMarshal.Read<Guid>( serializedData[ ..guidSizeBytes ] );
		return _globeStore.CurrentGlobe is not null && _globeStore.CurrentGlobe.Id == globeId;
	}
}
