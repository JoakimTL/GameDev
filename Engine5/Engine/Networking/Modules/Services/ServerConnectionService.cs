using Engine.GlobalServices;
using Engine.Networking.Module.TransferLayer;
using Engine.Networking.Module.TransferLayer.Tunnels;
using Engine.Networking.Modules.Services;
using Engine.Networking.Packets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Module.Services;

public class ServerConnectionService : Identifiable, INetworkServerService {

	private readonly NetworkMessagingService _networkMessagingService;
	private readonly TcpConnectionTunnelService _tcpConnectionTunnelService;
	private readonly UdpTunnelService _udpTunnelService;
	private readonly PacketReceptionService _packetReceptionService;
	private readonly PacketTypeRegistryService _packetTypeRegistryService;
	private readonly ServerNetworkIdDistributer _serverNetworkIdDistributer;
	private readonly Dictionary<IPEndPoint, ClientConnection> _clientConnectionsByTcpRemote;
	private readonly Dictionary<IPEndPoint, ClientConnection> _clientConnectionsByUdpRemote;
	private readonly Dictionary<string, ClientConnection> _clientConnectionsByUsername;
	private readonly Dictionary<ulong, ClientConnection> _clientConnectionsByNetworkId;

	public ServerConnectionService( NetworkMessagingService networkMessagingService, TcpConnectionTunnelService tcpConnectionTunnelService, UdpTunnelService udpTunnelService, PacketReceptionService packetReceptionService, PacketTypeRegistryService packetTypeRegistryService, ServerNetworkIdDistributer serverNetworkIdDistributer ) {
		this._networkMessagingService = networkMessagingService;
		this._networkMessagingService.PacketReceived += PacketReceived;
		this._tcpConnectionTunnelService = tcpConnectionTunnelService;
		this._udpTunnelService = udpTunnelService;
		this._packetReceptionService = packetReceptionService;
		this._packetTypeRegistryService = packetTypeRegistryService;
		this._serverNetworkIdDistributer = serverNetworkIdDistributer;
		this._tcpConnectionTunnelService.NewTcpTunnel += NewTcpTunnel;
		_clientConnectionsByTcpRemote = new();
		_clientConnectionsByUdpRemote = new();
		_clientConnectionsByUsername = new();
		_clientConnectionsByNetworkId = new();
	}

	private void PacketReceived( PacketBase @base ) {
		if ( @base is Packets.TcpLogin tcpLogin ) {
			if ( tcpLogin.RemoteSender is null || !_clientConnectionsByTcpRemote.TryGetValue( tcpLogin.RemoteSender, out ClientConnection? clientConnection ) ) {
				this.LogWarning( "Unable to get remote sender or connection from remote sender." );
				return;
			}
			if ( _clientConnectionsByUsername.TryAdd( tcpLogin.Username, clientConnection ) ) {
				_networkMessagingService.SendPacket( new TcpLoginAck( tcpLogin.Username, clientConnection.NetworkId ) );
			} else {
				clientConnection.TrySend( new ConnectionFailed( "Username already taken." ) );
				clientConnection.Dispose();
			}
		}
		if ( @base is Packets.UdpPing udpPing ) {
			if ( udpPing.RemoteSender is null ) {
				this.LogWarning( $"Udp ping has no remote sender." );
				return;
			}
			if ( !_clientConnectionsByUsername.TryGetValue( udpPing.Username, out ClientConnection? clientConnection ) ) {
				this.LogWarning( $"Unable to get connection from usernam {udpPing.Username}." );
				return;
			}
			_clientConnectionsByUdpRemote.Add( udpPing.RemoteSender, clientConnection );
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
		_networkMessagingService.SendPacket( new ClientDisconnected( connection.NetworkId, "Connection closed" ) ); //TODO: MAKE MORE SPECIFIC

	}

    internal void SendPacket( PacketBase packet ) {
		if (packet.RemoteTarget is not null) {
			ClientConnection? connection = null;
			if (packet.Protocol == System.Net.Sockets.ProtocolType.Tcp ) {
				_clientConnectionsByTcpRemote.TryGetValue( packet.RemoteTarget, out connection );
            }
			if (packet.Protocol == System.Net.Sockets.ProtocolType.Udp ) {
				_clientConnectionsByUdpRemote.TryGetValue( packet.RemoteTarget, out connection );
            }

            if ( connection is not null && !connection.TrySend( packet ) )
                this.LogWarning( $"Sending packet {packet} to {connection.NetworkId}({connection.Username}) failed!" );
        } else {
			foreach ( var connection in _clientConnectionsByNetworkId.Values )
				if ( !connection.TrySend( packet ) )
					this.LogWarning( $"Sending packet {packet} to {connection.NetworkId}({connection.Username}) failed!" );
        }
    }
}
