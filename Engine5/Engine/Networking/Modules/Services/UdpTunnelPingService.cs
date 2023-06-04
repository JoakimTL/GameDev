using Engine.GlobalServices.Network;
using Engine.Networking.Packets;
using Engine.Time;

namespace Engine.Networking.Modules.Services;

public sealed class UdpTunnelPingService : Identifiable, INetworkClientService {
	private readonly NetworkConnectionService _networkConnectionService;
	private readonly UdpTunnelService _udpTunnelService;
	private readonly TickingTimer _timer;

	public UdpTunnelPingService( NetworkConnectionService networkConnectionService, UdpTunnelService udpTunnelService ) {
		this._networkConnectionService = networkConnectionService;
		this._udpTunnelService = udpTunnelService;
		_timer = new TickingTimer( "Udp Ping Service", 500 );
		_timer.Elapsed += Ping;
		_timer.Start();
	}

	private void Ping( double time, double deltaTime ) {
		if ( _networkConnectionService.RemoteTarget is not null && _networkConnectionService.NetworkId.HasValue ) 
			_udpTunnelService.Tunnel.TrySend( new UdpPing( _networkConnectionService.NetworkId.Value, time ), _networkConnectionService.RemoteTarget );
	}
}