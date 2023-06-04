using Engine.Networking.Modules.TransferLayer.Tunnels;

namespace Engine.Networking.Modules.Services;

public sealed class TcpTunnelService : Identifiable, INetworkClientService {

	private readonly TcpNetworkTunnel _tcpTunnel;

	public TcpTunnelService( SocketFactory socketFactory, PacketTypeRegistryService packetTypeRegistryService ) {
		_tcpTunnel = new( socketFactory.CreateTcp(), packetTypeRegistryService );
	}

	public TcpNetworkTunnel Tunnel => _tcpTunnel;

}