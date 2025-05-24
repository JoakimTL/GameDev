using Civlike.World.GameplayState;
using Engine.Buffers;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Civlike.Logic.World.Serializers;

[Guid( "3F3FAB8B-E11B-4DB1-A666-600ED1D232C3" )]
public sealed class GlobeComponentSerializer( SerializerProvider serializerProvider, ActiveGlobeTrackingService activeGlobeTrackingService ) : SerializerBase<GlobeComponent>( serializerProvider ) {
	private readonly ActiveGlobeTrackingService _activeGlobeTrackingService = activeGlobeTrackingService;

	protected override void PerformSerialization( ThreadedByteBuffer buffer, GlobeComponent t ) {
		Span<byte> data = stackalloc byte[ 16 ];
		MemoryMarshal.Write( data, t.Globe?.Id ?? Guid.Empty );
		buffer.AddRange( data );
	}

	protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, GlobeComponent target ) {
		if (serializedData.Length < 16)
			return false;
		Guid globeId = MemoryMarshal.Read<Guid>( serializedData );
		if (globeId == Guid.Empty)
			return true;
		if (this._activeGlobeTrackingService.CurrentGlobe is null)
			throw new InvalidOperationException( "No active globe available." );
		if (globeId != this._activeGlobeTrackingService.CurrentGlobe.Id)
			return false;
		target.SetGlobe( this._activeGlobeTrackingService.CurrentGlobe );
		return true;
	}
}
