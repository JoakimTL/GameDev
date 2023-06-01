using Engine.GlobalServices;
using Engine.Structure.Interfaces;
using System.Collections.Concurrent;

namespace Engine.Networking.Module.Services;

public sealed class PacketReceptionService : Identifiable, INetworkClientService, INetworkServerService, IUpdateable {

	private readonly ConcurrentQueue<PacketBase> _incomingPackets;
	private readonly NetworkMessagingService _networkMessagingService;

	public PacketReceptionService( NetworkMessagingService networkMessagingService ) {
		_incomingPackets = new();
		this._networkMessagingService = networkMessagingService;
	}

	public void Update( float time, float deltaTime ) {
		while ( _incomingPackets.TryDequeue( out PacketBase? packet ) )
			_networkMessagingService.ReceivedPacket( packet );
	}

	internal void ReceivedPacket( PacketBase packet ) => _incomingPackets.Enqueue( packet );

}
