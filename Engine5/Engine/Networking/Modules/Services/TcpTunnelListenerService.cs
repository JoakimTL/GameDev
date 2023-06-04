using Engine.Networking.Modules.TransferLayer;
using Engine.Networking.Modules.TransferLayer.Tunnels;

namespace Engine.Networking.Modules.Services;

public sealed class TcpTunnelListenerService : Identifiable, INetworkServerService {
	private readonly TcpConnectionTunnelService _connectionTunnelService;

	public TcpTunnelListenerService( TcpConnectionTunnelService connectionTunnelService ) {
		_connectionTunnelService = connectionTunnelService;
		_connectionTunnelService.NewTcpTunnel += NewTunnel;
	}

	private void NewTunnel( NetworkTunnelBase obj ) {
		if ( obj is not TcpNetworkTunnel tcpTunnel )
			return;
	}
}
