using Engine.Networking.Module.TransferLayer.Tunnels;
using Engine.Networking.Modules.Services;
using System.Net;

namespace Engine.Networking.Module.Services;

public sealed class TcpTunnelService : Identifiable, INetworkClientService {

    private readonly TcpNetworkTunnel _tcpTunnel;

    public TcpTunnelService( SocketFactory socketFactory, PacketTypeRegistryService packetTypeRegistryService ) {
        _tcpTunnel = new( socketFactory.CreateTcp(), packetTypeRegistryService );
    }

    public TcpNetworkTunnel Tunnel => _tcpTunnel;

}