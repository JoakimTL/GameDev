using Engine.GlobalServices;
using Engine.Networking.Module;
using Engine.Networking.Module.Services;
using Engine.Structure.Interfaces;
using System.Collections.Concurrent;

namespace Engine.Networking.Modules.Services;

public sealed class ServerPacketDispatchingService : Identifiable, INetworkServerService, IUpdateable {
    private readonly NetworkMessagingService _networkMessagingService;
    private readonly NetworkConnectionService _networkConnectionService;
    private readonly ServerConnectionService _serverConnectionService;
    private readonly ConcurrentQueue<PacketBase> _outgoingPackets;

    public ServerPacketDispatchingService( NetworkMessagingService networkMessagingService, NetworkConnectionService networkConnectionService, ServerConnectionService serverConnectionService ) {
        this._networkMessagingService = networkMessagingService;
        this._networkConnectionService = networkConnectionService;
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
