using Engine.Networking.Module.TransferLayer;
using Engine.Networking.Module.TransferLayer.Tunnels;

namespace Engine.Networking.Module.Services;

public sealed class TcpTunnelListenerService : Identifiable, INetworkServerService {
	private readonly TcpConnectionTunnelService _connectionTunnelService;

	public TcpTunnelListenerService( TcpConnectionTunnelService connectionTunnelService ) {
		this._connectionTunnelService = connectionTunnelService;
		this._connectionTunnelService.NewTcpTunnel += NewTunnel;
	}

	private void NewTunnel( NetworkTunnelBase obj ) {
		if ( obj is not TcpNetworkTunnel tcpTunnel )
			return;
	}
}
