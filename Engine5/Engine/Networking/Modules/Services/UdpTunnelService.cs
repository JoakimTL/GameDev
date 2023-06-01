using Engine.Networking.Module.TransferLayer.Tunnels;
using Engine.Networking.Modules.Services;

namespace Engine.Networking.Module.Services;

public sealed class UdpTunnelService : Identifiable, INetworkClientService, INetworkServerService {

	private readonly UdpNetworkTunnel _udpTunnel;

    public UdpTunnelService( SocketFactory socketFactory, PacketTypeRegistryService packetTypeRegistryService )
    {
		_udpTunnel = new( socketFactory.CreateUdp(), packetTypeRegistryService );
    }

	public UdpNetworkTunnel Tunnel => _udpTunnel;

}
