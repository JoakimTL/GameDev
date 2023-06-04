using Engine.Networking.Modules.TransferLayer.Tunnels;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Modules.Services;

public sealed class TcpConnectionTunnelService : Identifiable, INetworkServerService {

	private readonly ConcurrentDictionary<IPEndPoint, TcpNetworkTunnel> _tcpTunnels;
	private readonly ConnectionAccepterService _connectionAccepterService;
	private readonly PacketTypeRegistryService _packetTypeRegistryService;

	public event Action<TcpNetworkTunnel>? NewTcpTunnel;

	public TcpConnectionTunnelService( ConnectionAccepterService connectionAccepterService, PacketTypeRegistryService packetTypeRegistryService ) {
		_tcpTunnels = new();
		_connectionAccepterService = connectionAccepterService;
		_packetTypeRegistryService = packetTypeRegistryService;
		_connectionAccepterService.NewTcpSocket += NewTcpSocket;
	}

	private void NewTcpSocket( Socket socket ) {
		if ( socket.RemoteEndPoint is not IPEndPoint endpoint || _tcpTunnels.ContainsKey( endpoint ) ) {
			socket.Close();
			return;
		}
		this.LogLine( "New Tcp connection!", Log.Level.NORMAL, ConsoleColor.Green );
		TcpNetworkTunnel tunnel = new( socket, _packetTypeRegistryService );
		if ( !_tcpTunnels.TryAdd( endpoint, tunnel ) ) {
			socket.Close();
			return;
		}
		NewTcpTunnel?.Invoke( tunnel );

		//TODO
		/*
		 * Socket connects -> added as tunnel here.
		 * An instance of "NamedConnection" is created whenever a connection packet with a new login name is sent. 
		 * Tcp connection packet with name -> added into named connection.
		 * Udp connection packet with name -> added into named connection.
		 * If any connection in the named connection is closed abruptly it is assumed the connection should close.
		 * Tcp connection is abruptly closed or a disconnect packet is received -> named connection closes.
		 * Closing this connection causes the tunnels to close with an event.
		 * Closing tunnels should remove the tunnel from this list.
		 */

		//NetworkConnection connection = new( this._udpTunnel, newConnectionSocket );
		//this._connectionsByTcpEndpoint.TryAdd( endpoint, connection );
		//connection.ReceivedTcpMessage += OnMessageReceived;
		//connection.OnClosing += OnConnectionClosing;
		//connection.StartTCPReceiver();
	}
}
