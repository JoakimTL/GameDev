using System.Net;
using System.Net.Sockets;

namespace Engine.Networking;

public class NetworkConnection : DisposableIdentifiable {

	public event Action<IPEndPoint, ProtocolType, byte[]>? ReceivedTcpMessage;
	public event Action<NetworkConnection, string>? OnClosing;

	/// <summary>
	/// The network id of this network. 0 is reserved for the uninitiated or the server. Clients should never utilize networkid zero.
	/// </summary>
	public ulong NetworkId { get; private set; }
	public IPEndPoint TcpEndPoint { get; private set; }
	public IPEndPoint? UdpEndPoint { get; private set; }

	private readonly NetworkTunnelUdp _tunnelUdp;
	private readonly NetworkTunnelTcp _tunnelTcp;
	public double PingTimeSeconds { get; private set; }

	private Thread? _tcpReceiverThread;

	public NetworkConnection( NetworkTunnelUdp udpTunnel, Socket tcpSocket ) {
		this.NetworkId = 0;
		this._tunnelUdp = udpTunnel;
		this._tunnelTcp = new NetworkTunnelTcp( tcpSocket );
		this.TcpEndPoint = tcpSocket.RemoteEndPoint as IPEndPoint ?? throw new Exception( "TCP Endpoint not of type IPEndPoint" );
		this.UdpEndPoint = null;
		this._tunnelTcp.OnAbruptClosure += AbrubtClosureOfTcp;
	}

	private void AbrubtClosureOfTcp( NetworkTunnelBase tunnel, SocketError socketError ) {
		this.LogWarning( $"Socket error: {socketError}! Closing connection!" );
		Close( "Abrupt connection closure" );
	}

	internal void SetNetworkId( ulong id ) => this.NetworkId = id;
	internal void SetUDPEndpoint( IPEndPoint endpoint ) => this.UdpEndPoint = endpoint;

	/// <summary>
	/// Starts receiver for tcp connection.
	/// </summary>
	public void StartTCPReceiver() {
		if ( this._tcpReceiverThread is not null )
			return;
		this._tcpReceiverThread = Resources.GlobalService<ThreadManager>().Start( ReceiveTCP, $"Tcp Receiver" );
	}

	private void OnTcpMessageComplete( byte[] message ) => ReceivedTcpMessage?.Invoke( this.TcpEndPoint, ProtocolType.Tcp, message );

	private void ReceiveTCP() {
		DataReceiver receiver = new();
		receiver.MessageComplete += OnTcpMessageComplete;
		byte[] dataBuffer = new byte[ 1024 ];

		this.LogLine( "Receiving Tcp messages!", Log.Level.NORMAL );
		while ( !this.Disposed ) {
			if ( this._tunnelTcp.TryReceive( dataBuffer, out int length, out _ ) )
				receiver.ReceivedData( dataBuffer, length );
		}
		this.LogLine( "Stopped receiving Tcp messages!", Log.Level.NORMAL );
		receiver.MessageComplete -= OnTcpMessageComplete;
	}

	/// <summary>
	/// Closes tcp connection.
	/// </summary>
	private void Close( string reason ) {
		this._tunnelTcp.Dispose();
		OnClosing?.Invoke( this, reason );
	}

	public void Send( Packet p ) {
		if ( this.Disposed )
			return;
		ProtocolType protocol = Resources.GlobalService<PacketTypeManager>().GetPacketProtocol( p.GetType() );
		if ( protocol == ProtocolType.Udp ) {
			this._tunnelUdp.TrySend( p );
		} else {
			this._tunnelTcp.TrySend( p );
		}
	}

	protected override bool OnDispose() {
		Close( "Disposal" );
		return true;
	}

	public void SetPing( double pingSeconds ) => this.PingTimeSeconds = pingSeconds;
}
