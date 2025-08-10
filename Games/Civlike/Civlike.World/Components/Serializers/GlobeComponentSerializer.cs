using Civlike.World.State;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civlike.World.Components.Serializers;

[Guid( "3F3FAB8B-E11B-4DB1-A666-600ED1D232C3" )]
public sealed class GlobeComponentSerializer( SerializerProvider serializerProvider, GlobeStoreService globeStore ) : SerializerBase<GlobeComponent>( serializerProvider ) {
	private readonly GlobeStoreService _globeStore = globeStore;

	protected override void PerformSerialization( ThreadedByteBuffer buffer, GlobeComponent t ) {
		Span<byte> data = stackalloc byte[ 16 ];
		MemoryMarshal.Write( data, t.Globe?.Id ?? Guid.Empty );
		buffer.AddRange( data );
	}

	protected override void PerformDeserialization( ReadOnlySpan<byte> serializedData, GlobeComponent target ) {
		if (serializedData.Length < 16)
			throw new InvalidOperationException( "Invalid serialized data." );
		Guid globeId = MemoryMarshal.Read<Guid>( serializedData );
		if (globeId == Guid.Empty)
			return;
		Globe globe = _globeStore.CurrentGlobe ?? throw new InvalidOperationException( $"Globe not found." );
		if (globe.Id != globeId)
			throw new InvalidOperationException( $"Globe with id {globeId} not found." );
		target.SetGlobe( globe );
	}

	protected override bool CanDeserializeCheck( ReadOnlySpan<byte> serializedData ) {
		if (serializedData.Length < 16)
			return false;
		Guid globeId = MemoryMarshal.Read<Guid>( serializedData );
		return globeId == Guid.Empty || _globeStore.CurrentGlobe is not null && globeId == _globeStore.CurrentGlobe.Id;
	}
}
