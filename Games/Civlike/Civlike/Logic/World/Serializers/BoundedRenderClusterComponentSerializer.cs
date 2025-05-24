using Civlike.World.GameplayState;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civlike.Logic.World.Serializers;

[Guid( "EEC37F0A-A448-44C9-A708-97A3A0A0941E" )]
public sealed class BoundedRenderClusterComponentSerializer( SerializerProvider serializerProvider, ActiveGlobeTrackingService activeGlobeTrackingService ) : SerializerBase<BoundedRenderClusterComponent>( serializerProvider ) {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	protected override void PerformSerialization( ThreadedByteBuffer buffer, BoundedRenderClusterComponent t ) {
		Span<byte> data = stackalloc byte[ 20 ];
		MemoryMarshal.Write( data, t.Globe?.Id ?? Guid.Empty );
		MemoryMarshal.Write( data[16..], t.Cluster.Id );
		buffer.AddRange( data );
	}

	protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, BoundedRenderClusterComponent target ) {
		if (serializedData.Length < 20)
			return false;
		Guid globeId = MemoryMarshal.Read<Guid>( serializedData );
		if (globeId == Guid.Empty)
			return true;
		if (this._activeGlobeTrackingService.CurrentGlobe is null)
			throw new InvalidOperationException( "No active globe available." );
		if (globeId != this._activeGlobeTrackingService.CurrentGlobe.Id)
			return false;
		target.Set( this._activeGlobeTrackingService.CurrentGlobe, MemoryMarshal.Read<int>( serializedData[ 16.. ] ) );
		return true;
	}
}