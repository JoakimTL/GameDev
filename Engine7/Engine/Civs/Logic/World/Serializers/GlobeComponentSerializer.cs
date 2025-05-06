using Civs.World;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civs.Logic.World.Serializers;

[Guid( "3F3FAB8B-E11B-4DB1-A666-600ED1D232C3" )]
public sealed class GlobeComponentSerializer( SerializerProvider serializerProvider, ActiveGlobeTrackingService activeGlobeTrackingService ) : SerializerBase<GlobeComponent>( serializerProvider ) {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	protected override void PerformSerialization( ThreadedByteBuffer buffer, GlobeComponent t ) {
		Span<byte> data = stackalloc byte[ 16 ];
		MemoryMarshal.Write( data, t.Globe?.Id ?? Guid.Empty );
		buffer.Add( data );
	}

	protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, GlobeComponent target ) {
		if (serializedData.Length < 16)
			return false;
		Guid globeId = MemoryMarshal.Read<Guid>( serializedData );
		if (globeId == Guid.Empty)
			return true;
		if (_activeGlobeTrackingService.CurrentGlobe is null)
			throw new InvalidOperationException( "No active globe available." );
		if (globeId != _activeGlobeTrackingService.CurrentGlobe.Id)
			return false;
		target.SetGlobe( _activeGlobeTrackingService.CurrentGlobe );
		return true;
	}
}


[Guid( "EEC37F0A-A448-44C9-A708-97A3A0A0941E" )]
public sealed class BoundedRenderClusterComponentSerializer( SerializerProvider serializerProvider, ActiveGlobeTrackingService activeGlobeTrackingService ) : SerializerBase<BoundedRenderClusterComponent>( serializerProvider ) {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	protected override void PerformSerialization( ThreadedByteBuffer buffer, BoundedRenderClusterComponent t ) {
		Span<byte> data = stackalloc byte[ 20 ];
		MemoryMarshal.Write( data, t.Globe?.Id ?? Guid.Empty );
		MemoryMarshal.Write( data[16..], t.Cluster.Id );
		buffer.Add( data );
	}

	protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, BoundedRenderClusterComponent target ) {
		if (serializedData.Length < 20)
			return false;
		Guid globeId = MemoryMarshal.Read<Guid>( serializedData );
		if (globeId == Guid.Empty)
			return true;
		if (_activeGlobeTrackingService.CurrentGlobe is null)
			throw new InvalidOperationException( "No active globe available." );
		if (globeId != _activeGlobeTrackingService.CurrentGlobe.Id)
			return false;
		target.Set( _activeGlobeTrackingService.CurrentGlobe, MemoryMarshal.Read<int>( serializedData[ 16.. ] ) );
		return true;
	}
}