using Engine.GlobalServices;
using Engine.Networking.Module;
using Engine.Networking.Module.Services;
using Engine.Structure.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Networking.Modules.Services;
public sealed class ClientPacketDispatchingService : Identifiable, INetworkClientService, IUpdateable {
    private readonly NetworkMessagingService _networkMessagingService;
    private readonly NetworkConnectionService _networkConnectionService;
    private readonly TcpTunnelService _tcpTunnelService;
    private readonly UdpTunnelService _udpTunnelService;
    private readonly ConcurrentQueue<PacketBase> _outgoingPackets;

    public ClientPacketDispatchingService( NetworkMessagingService networkMessagingService, NetworkConnectionService networkConnectionService, TcpTunnelService tcpTunnelService, UdpTunnelService udpTunnelService ) {
        this._networkMessagingService = networkMessagingService;
        this._networkConnectionService = networkConnectionService;
        this._tcpTunnelService = tcpTunnelService;
        this._udpTunnelService = udpTunnelService;
        _outgoingPackets = new();
        _networkMessagingService.PacketSent += PacketSent;
    }

    private void PacketSent( PacketBase @base ) => _outgoingPackets.Enqueue( @base );

    public void Update( float time, float deltaTime ) {
        while ( _outgoingPackets.TryDequeue( out var packet ) ) {
            if ( packet.Protocol == ProtocolType.Tcp )
                _tcpTunnelService.Tunnel.TrySend( packet );
            if ( packet.Protocol == ProtocolType.Udp ) {
                var remoteTarget = _networkConnectionService.RemoteTarget;
                if ( remoteTarget is not null )
                    _udpTunnelService.Tunnel.TrySend( packet, remoteTarget );
            }
        }
    }
}
