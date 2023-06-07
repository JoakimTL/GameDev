using Engine.GlobalServices.Network;
using Engine.Networking.Modules.TransferLayer;
using Engine.Networking.Modules.TransferLayer.Tunnels;
using Engine.Networking.Packets;
using System.Net;

namespace Engine.Networking.Modules.Services;

public class ServerConnectionService : Identifiable, INetworkServerService {

	private readonly NetworkMessagingService _networkMessagingService;
	private readonly TcpConnectionTunnelService _tcpConnectionTunnelService;
	private readonly UdpTunnelService _udpTunnelService;
	private readonly UdpTunnelPacketReceiverService _udpTunnelPacketReceiverService;
	private readonly PacketReceptionService _packetReceptionService;
	private readonly PacketTypeRegistryService _packetTypeRegistryService;
	private readonly ServerNetworkIdDistributer _serverNetworkIdDistributer;
	private readonly ServerPortService _serverPortService;
	private readonly Dictionary<IPEndPoint, ClientConnection> _clientConnectionsByTcpRemote;
	private readonly Dictionary<IPEndPoint, ClientConnection> _clientConnectionsByUdpRemote;
	private readonly Dictionary<string, ClientConnection> _clientConnectionsByUsername;
	private readonly Dictionary<NetworkId, ClientConnection> _clientConnectionsByNetworkId;

	public ServerConnectionService( NetworkMessagingService networkMessagingService, TcpConnectionTunnelService tcpConnectionTunnelService, UdpTunnelService udpTunnelService, UdpTunnelPacketReceiverService udpTunnelPacketReceiverService, PacketReceptionService packetReceptionService, PacketTypeRegistryService packetTypeRegistryService, ServerNetworkIdDistributer serverNetworkIdDistributer, ServerPortService serverPortService ) {
		_networkMessagingService = networkMessagingService;
		_networkMessagingService.PacketReceived += PacketReceived;
		_tcpConnectionTunnelService = tcpConnectionTunnelService;
		_udpTunnelService = udpTunnelService;
		this._udpTunnelPacketReceiverService = udpTunnelPacketReceiverService;
		_packetReceptionService = packetReceptionService;
		_packetTypeRegistryService = packetTypeRegistryService;
		_serverNetworkIdDistributer = serverNetworkIdDistributer;
		this._serverPortService = serverPortService;
		_tcpConnectionTunnelService.NewTcpTunnel += NewTcpTunnel;
		_clientConnectionsByTcpRemote = new();
		_clientConnectionsByUdpRemote = new();
		_clientConnectionsByUsername = new();
		_clientConnectionsByNetworkId = new();
		_udpTunnelPacketReceiverService.Start( new IPEndPoint( IPAddress.Any, _serverPortService.Port ) );
	}

	private void PacketReceived( PacketBase @base ) {
		if ( @base is TcpLogin tcpLogin ) {
			if ( tcpLogin.RemoteSender is null || !_clientConnectionsByTcpRemote.TryGetValue( tcpLogin.RemoteSender, out ClientConnection? clientConnection ) ) {
				this.LogWarning( "Unable to get remote sender or connection from remote sender." );
				return;
			}
			if ( _clientConnectionsByUsername.TryAdd( tcpLogin.Username, clientConnection ) && _clientConnectionsByUdpRemote.TryAdd( new IPEndPoint( tcpLogin.RemoteSender.Address, tcpLogin.UdpPort ), clientConnection ) ) {
				clientConnection.SetUdpRemoteTarget( new IPEndPoint( tcpLogin.RemoteSender.Address, tcpLogin.UdpPort ) );
				clientConnection.SetUsername( tcpLogin.Username );
				_networkMessagingService.SendPacket( new TcpLoginAck( clientConnection.NetworkId, tcpLogin.Username ) );
				this.LogLine( $"Client {tcpLogin.Username} logged in with network id {clientConnection.NetworkId}.", Log.Level.NORMAL );
			} else {
				clientConnection.TrySend( new ConnectionFailed( "Username already taken." ) );
				clientConnection.Dispose();
			}
		}
		if ( @base is UdpPing udpPing ) {
			if ( udpPing.RemoteSender is null ) {
				this.LogWarning( $"Udp ping has no remote sender." );
				return;
			}
			if ( !_clientConnectionsByNetworkId.TryGetValue( udpPing.NetworkId, out ClientConnection? clientConnection ) ) {
				this.LogWarning( $"Unable to get connection from username {udpPing.NetworkId}." );
				return;
			}
			this.LogLine( $"Received udp ping from {udpPing.RemoteSender}.", Log.Level.VERBOSE );
			udpPing.RemoteTarget = udpPing.RemoteSender;
			_networkMessagingService.SendPacket( udpPing );
		}
	}

	private void NewTcpTunnel( TcpNetworkTunnel tunnel ) {
		ClientConnection connection = new( _serverNetworkIdDistributer.NewConnection(), tunnel, _udpTunnelService.Tunnel, _packetReceptionService, _packetTypeRegistryService );
		connection.ConnectionClosed += ConnectionClosed;
		_clientConnectionsByTcpRemote.Add( connection.TcpRemoteTarget, connection );
		_clientConnectionsByNetworkId.Add( connection.NetworkId, connection );
	}

	private void ConnectionClosed( ClientConnection connection ) {
		_serverNetworkIdDistributer.ConnectionClosed( connection.NetworkId );
		_clientConnectionsByNetworkId.Remove( connection.NetworkId );
		_clientConnectionsByTcpRemote.Remove( connection.TcpRemoteTarget );
		if ( connection.UdpRemoteTarget is not null )
			_clientConnectionsByUdpRemote.Remove( connection.UdpRemoteTarget );
		if ( connection.Username is not null )
			_clientConnectionsByUsername.Remove( connection.Username );
		_networkMessagingService.SendPacket( new ClientDisconnected( connection.NetworkId, connection.Error.ToString() ?? "Disconnected" ) ); //TODO: MAKE MORE SPECIFIC

	}

	internal void SendPacket( PacketBase packet ) {
		if ( packet.RemoteTarget is not null ) {
			ClientConnection? connection = null;
			if ( packet.Protocol == System.Net.Sockets.ProtocolType.Tcp )
				_clientConnectionsByTcpRemote.TryGetValue( packet.RemoteTarget, out connection );
			if ( packet.Protocol == System.Net.Sockets.ProtocolType.Udp )
				_clientConnectionsByUdpRemote.TryGetValue( packet.RemoteTarget, out connection );

			if ( connection is not null && !connection.TrySend( packet ) )
				this.LogWarning( $"Sending packet {packet} to {connection.NetworkId}({connection.Username}) failed!" );
		} else
			foreach ( var connection in _clientConnectionsByNetworkId.Values )
				if ( !connection.TrySend( packet ) )
					this.LogWarning( $"Sending packet {packet} to {connection.NetworkId}({connection.Username}) failed!" );
	}
}
