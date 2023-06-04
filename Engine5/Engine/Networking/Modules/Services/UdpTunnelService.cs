using Engine.Networking.Modules.TransferLayer.Tunnels;

namespace Engine.Networking.Modules.Services;

public sealed class UdpTunnelService : Identifiable, INetworkClientService, INetworkServerService {

	private readonly UdpNetworkTunnel _udpTunnel;

	public UdpTunnelService( SocketFactory socketFactory, PacketTypeRegistryService packetTypeRegistryService ) {
		_udpTunnel = new( socketFactory.CreateUdp(), packetTypeRegistryService );
	}

	public UdpNetworkTunnel Tunnel => _udpTunnel;

}
