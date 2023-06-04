using Engine.GlobalServices.Network;
using Engine.Networking.Modules.TransferLayer;
using Engine.Networking.Packets;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Modules.Services;

[ProcessAfter<ClientPacketDispatchingService, IUpdateable>]
public class ClientConnectionService : Identifiable, INetworkClientService, IUpdateable {
	private readonly NetworkConnectionService _networkConnectionService;
	private readonly PacketReceptionService _packetReceptionService;
	private readonly TcpTunnelService _tcpTunnelService;
	private readonly UdpTunnelPacketReceiverService _udpTunnelPacketReceiverService;
	private readonly NetworkClientUsernameService _networkClientUsernameService;
	private readonly NetworkMessagingService _networkMessagingService;
	private readonly TcpTunnelPacketReceiverService _tcpTunnelPacketReceiverService;
	private readonly UdpTunnelService _udpTunnelService;
	private bool _hasUpdated;

	public ClientConnectionService( NetworkConnectionService networkConnectionService, PacketReceptionService packetReceptionService, TcpTunnelService tcpTunnelService, TcpTunnelPacketReceiverService tcpTunnelPacketReceiverService, UdpTunnelService udpTunnelService, UdpTunnelPacketReceiverService udpTunnelPacketReceiverService, NetworkClientUsernameService networkClientUsernameService, NetworkMessagingService networkMessagingService ) {
		_networkConnectionService = networkConnectionService;
		_packetReceptionService = packetReceptionService;
		_tcpTunnelService = tcpTunnelService;
		_tcpTunnelService.Tunnel.OnAbruptClosure += OnAbrubtClosure;
		_tcpTunnelPacketReceiverService = tcpTunnelPacketReceiverService;
		_udpTunnelService = udpTunnelService;
		_udpTunnelPacketReceiverService = udpTunnelPacketReceiverService;
		this._networkClientUsernameService = networkClientUsernameService;
		this._networkMessagingService = networkMessagingService;
		_networkMessagingService.PacketReceived += OnPacketReceived;
		_networkConnectionService.RemoteTargetChanged += RemoteTargetChanged;
		_hasUpdated = _networkConnectionService.RemoteTarget is not null;

		_tcpTunnelService.Tunnel.Bind( new IPEndPoint( IPAddress.Any, 0 ) );
		_udpTunnelPacketReceiverService.Start( _tcpTunnelService.Tunnel.LocalEndPoint as IPEndPoint ?? throw new NullReferenceException( "The local endpoint for Tcp is NULL. Unable to create local endpoint for Udp tunnel." ) );
	}

	private void OnPacketReceived( PacketBase packet ) {
		if ( packet is TcpLoginAck tcpLoginAck ) {
			_networkConnectionService.NetworkId = tcpLoginAck.NetworkId;
			this.LogLine( $"Logged in with network id: {tcpLoginAck.NetworkId}", Log.Level.NORMAL );
		}

		if ( packet is ConnectionFailed connectionFailed ) {
			_networkConnectionService.Disonnect();
			this.LogLine( $"Failed to connect to server. Reason: {connectionFailed.Reason}", Log.Level.HIGH );
		}
	}

	private void OnAbrubtClosure( NetworkTunnelBase @base, SocketError error ) {
		_networkConnectionService.Disonnect();
		this.LogLine( $"Server closed connection without warning. Error: {error}", Log.Level.HIGH );
	}

	public void Update( float time, float deltaTime ) {
		if ( !_hasUpdated )
			return;
		_hasUpdated = false;
		this.LogLine( $"Remote target changed to {_networkConnectionService.RemoteTarget?.ToString() ?? "None"}!", Log.Level.NORMAL );

		Disconnect( _networkConnectionService.NetworkId.HasValue ? new ClientDisconnected( _networkConnectionService.NetworkId.Value, "Disconnected" ) : null );

		if ( _networkConnectionService.RemoteTarget is null )
			return;

		_tcpTunnelService.Tunnel.Connect( _networkConnectionService.RemoteTarget );
		_tcpTunnelPacketReceiverService.Start();
		_tcpTunnelService.Tunnel.TrySend( new TcpLogin( _networkClientUsernameService.Username, (ushort) ( _udpTunnelService.Tunnel.LocalEndPoint as IPEndPoint ).NotNull().Port ) );
		//TODO create connection code
	}

	private void RemoteTargetChanged() {
		_hasUpdated = true;
	}

	private void Disconnect( PacketBase? packet ) {
		if ( _tcpTunnelService.Tunnel.RemoteEndPoint is null )
			return;
		_tcpTunnelPacketReceiverService.Stop();
		_tcpTunnelService.Tunnel.Disconnect( packet );
	}


}
