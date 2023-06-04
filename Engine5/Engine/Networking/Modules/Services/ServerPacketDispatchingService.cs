using Engine.GlobalServices.Network;
using Engine.Structure.Interfaces;
using System.Collections.Concurrent;

namespace Engine.Networking.Modules.Services;

public sealed class ServerPacketDispatchingService : Identifiable, INetworkServerService, IUpdateable {
    private readonly NetworkMessagingService _networkMessagingService;
    private readonly ServerConnectionService _serverConnectionService;
    private readonly ConcurrentQueue<PacketBase> _outgoingPackets;

    public ServerPacketDispatchingService( NetworkMessagingService networkMessagingService, ServerConnectionService serverConnectionService ) {
        this._networkMessagingService = networkMessagingService;
        this._serverConnectionService = serverConnectionService;
        _outgoingPackets = new();
        _networkMessagingService.PacketSent += PacketSent;
    }

    private void PacketSent( PacketBase @base ) => _outgoingPackets.Enqueue( @base );

    public void Update( float time, float deltaTime ) {
        while ( _outgoingPackets.TryDequeue( out var packet ) )
            _serverConnectionService.SendPacket( packet );
    }
}
