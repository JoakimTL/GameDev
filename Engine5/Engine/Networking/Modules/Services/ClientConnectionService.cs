using Engine.GlobalServices;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using System.Net;

namespace Engine.Networking.Module.Services;

[ProcessAfter<IUpdateable>]
public class ClientConnectionService : Identifiable, INetworkClientService, IUpdateable {
	private readonly NetworkConnectionService _networkConnectionService;
	private readonly PacketReceptionService _packetReceptionService;
	private readonly TcpTunnelService _tcpTunnelService;
	private readonly UdpTunnelPacketReceiverService _udpTunnelPacketReceiverService;
	private readonly TcpTunnelPacketReceiverService _tcpTunnelPacketReceiverService;
	private readonly UdpTunnelService _udpTunnelService;
	private bool _hasUpdated;

	private ulong networkId;

	public ClientConnectionService( NetworkConnectionService networkConnectionService, PacketReceptionService packetReceptionService, TcpTunnelService tcpTunnelService, TcpTunnelPacketReceiverService tcpTunnelPacketReceiverService, UdpTunnelService udpTunnelService, UdpTunnelPacketReceiverService udpTunnelPacketReceiverService ) {
		this._networkConnectionService = networkConnectionService;
		this._packetReceptionService = packetReceptionService;
		this._tcpTunnelService = tcpTunnelService;
		this._tcpTunnelPacketReceiverService = tcpTunnelPacketReceiverService;
		this._udpTunnelService = udpTunnelService;
		this._udpTunnelPacketReceiverService = udpTunnelPacketReceiverService;
		this._networkConnectionService.RemoteTargetChanged += RemoteTargetChanged;

		_tcpTunnelService.Tunnel.Bind( new IPEndPoint( IPAddress.Any, 0 ) );
		_udpTunnelService.Tunnel.Bind( _tcpTunnelService.Tunnel.LocalEndPoint ?? throw new NullReferenceException( "The local endpoint for Tcp is NULL. Unable to create local endpoint for Udp tunnel." ) );
	}

	public void Update( float time, float deltaTime ) {
		if ( !_hasUpdated )
			return;
		this.LogLine( $"Remote target changed to {_networkConnectionService.RemoteTarget?.ToString() ?? "None"}!", Log.Level.NORMAL );

		if ( _tcpTunnelService.Tunnel.RemoteEndPoint is not null ) {

			_tcpTunnelService.Tunnel.Disconnect();
		}
		if ( _networkConnectionService.RemoteTarget is null )
			return;

		_tcpTunnelService.Tunnel.Connect( _networkConnectionService.RemoteTarget );
		//TODO create connection code
	}

	private void RemoteTargetChanged() {
		_hasUpdated = true;
	}


}
