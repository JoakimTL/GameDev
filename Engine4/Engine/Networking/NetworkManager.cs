using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Engine.Modularity.Modules;
using Engine.Modularity.Modules.Singletons;
using Engine.Time;

namespace Engine.Networking;
public class NetworkManager : ModuleSingletonBase, IPacketListener {

	public event Action<NetworkConnection>? NewNamedConnection;

	private readonly HashSet<IPacketListener> _packetListeners;
	private readonly Userdata _userdata;
	private readonly SocketFactory _socketFactory;
	private readonly ConcurrentDictionary<IPEndPoint, NetworkConnection> _connectionsByTcpEndpoint;
	private readonly ConcurrentDictionary<IPEndPoint, NetworkConnection> _connectionsByUdpEndpoint;
	private readonly ConcurrentDictionary<string, NetworkConnection> _namedConnections;
	private readonly ConcurrentDictionary<ulong, NetworkConnection> _connectionsById;

	public IReadOnlyCollection<NetworkConnection> Connections => (IReadOnlyCollection<NetworkConnection>) this._namedConnections.Values;

	private NetworkTunnelUdp? _udpTunnel;
	private Thread? _udpReceiverThread;
	private Thread? _tcpAcceptingThread;
	private readonly Timer32 _pingTimer;
	private readonly HashSet<ulong> _usedNetworkIds;

	public NetworkConnection? ServerConnection => this._namedConnections.GetValueOrDefault( "_Server_" );

	public ulong ServerProvidedId { get; private set; }
	public bool IsServer { get; private set; }
	public bool Open { get; private set; }
	public string Username { get; private set; }

	public NetworkManager( Userdata userdata, SocketFactory socketFactory ) {
		this._userdata = userdata;
		this._socketFactory = socketFactory;
		this.Username = userdata.Username;
		this._connectionsByTcpEndpoint = new();
		this._connectionsByUdpEndpoint = new();
		this._namedConnections = new();
		this._connectionsById = new();
		this._packetListeners = new();
		this._usedNetworkIds = new();
		this._pingTimer = new Timer32( $"Udp pinger", 500 );
		this._pingTimer.Elapsed += OnPing;
	}

	private void OnPing() => Send( new UdpPing( this.Username, Clock64.StartupTime, null ) );

	public void AddListener( IPacketListener listener ) => this._packetListeners.Add( listener );

	public void RemoveListener( IPacketListener listener ) => this._packetListeners.Remove( listener );

	public void StartClient( IPEndPoint endpoint ) {
		//Connects the client to another IP. This includes both TCP and UDP on the same port.
		if ( this.Open ) {
			this.LogWarning( $"Connection already open as {( this.IsServer ? "Server" : "Client" )}!" );
			return;
		}
		try {
			this.IsServer = false;
			this.Open = true;
			this._udpTunnel = new NetworkTunnelUdp( this._socketFactory.CreateUdp() );
			AddListener( this );
			Socket tcpSocket = this._socketFactory.CreateTcp();
			tcpSocket.Connect( endpoint );
			StartUdpReceiver( tcpSocket.LocalEndPoint as IPEndPoint ?? throw new Exception( "TCP local endpoint is null!" ) );
			NetworkConnection connection = new( this._udpTunnel, tcpSocket );
			connection.SetName( "_Server_" );
			connection.SetUDPEndpoint( endpoint );
			connection.SetNetworkId( 0 );
			this._connectionsByTcpEndpoint.TryAdd( endpoint, connection );
			this._connectionsByUdpEndpoint.TryAdd( endpoint, connection );
			this._namedConnections.TryAdd( connection.Name, connection );
			connection.ReceivedTcpMessage += OnMessageReceived;
			connection.OnClosing += OnConnectionClosing;
			connection.StartTCPReceiver();
			if ( !this._userdata.UsernameSet )
				this.LogWarning( "Connecting without setting username!" );
			this.Username = this._userdata.Username;
			Send( new TcpLogin( this.Username ) );
			this._pingTimer.Start();
		} catch ( Exception e ) {
			this.LogError( e );
			Close();
		}
	}

	public void StartServer( ushort port ) {
		//Starts listening on a udp tunnel.
		if ( this.Open ) {
			this.LogWarning( $"Connection already open as {( this.IsServer ? "Server" : "Client" )}!" );
			return;
		}
		try {
			this.IsServer = true;
			this.Username = "_Server_";
			this.Open = true;
			this._udpTunnel = new NetworkTunnelUdp( this._socketFactory.CreateUdp() );
			StartUdpReceiver( new IPEndPoint( IPAddress.Any, port ) );
			AddListener( this );
			StartTcpConnectionAccepting();
			this._pingTimer.Start();
		} catch ( Exception e ) {
			this.LogError( e );
			Close();
		}
	}

	//x UDP Connections should only accept packets of size 512 bytes
	//x TCP Connections should acceps packets of any size. TCP Connections will also add a packet length to the first 4 bytes of the message, to give an indication when the packet is read.
	//UDP Connections should only read 512 bytes of data. Each receive can only be one packet. The data can essentially be sent directly to consumers, and does not need further processing (except for wrapping it in a packet)
	//TCP Connection should be capable of reading as much data as needed. Each tunnel must have an expandable list that can accomodate small and large packets. After the entire message has been processed, it can be wrapped in a packet and be consumed.
	//Packets should have an endpoint. If the endpoint is null the message is broadcasted. If the endpoint does not exist the packet is disgarded (with error logging)
	//Servers and client should behave the same, with registered connections to other sockets.
	//Servers will have a list of named connections, with the connection containing the endpoint to the client. Sending a packet with null endpoint will cause the packet to be broadcasted to all clients connected.
	//Clients will have a "list" of "named" connections. The only connection will be to the server. Sending a packet with null endpoint will cause the packet to be broadcasted to the server.
	//Behind the scenes the implementation should be identical for servers and clients.
	//UDP Connections only have one socket. The networkmanager will contain this connection.
	//TCP Connections will have a socket each. The networkmanager will contain these connections.
	//A UDP Connection will use the networkmanager socket to do sends and receives. For clients this will look the same as TCP, but servers will use the same socket for multiple client connections.

	//Connection failure cases:
	// - Abrubt remote connection closure
	// - Graceful remote connection closure
	// - Abrubt local connection closure
	// - Graceful local connection closure

	private void OnMessageReceived( IPEndPoint endpoint, ProtocolType protocol, byte[] data ) {
		this.LogLine( $"{( this.IsServer ? "Server" : this.Username )} received {protocol} message of length {data.Length} bytes from {endpoint}!", Log.Level.NORMAL, ConsoleColor.Blue );
		Packet? p = Resources.Get<PacketTypeManager>().GetPacketFromData( data, endpoint );
		if ( p is null )
			return;
		Type t = p.GetType();
		foreach ( IPacketListener listener in this._packetListeners ) {
			if ( listener.Listening( t ) )
				if ( ( protocol == ProtocolType.Udp && listener.ListeningUdp( endpoint ) ) || ( protocol == ProtocolType.Tcp && listener.ListeningTcp( endpoint ) ) )
					listener.NewPacket( p, t );
		}
	}

	public bool Listening( Type packetType ) => ( this.IsServer && packetType == typeof( TcpLogin ) ) || packetType == typeof( ConnectionFailed ) || packetType == typeof( UdpPing ) || packetType == typeof( TcpLoginAck ) || packetType == typeof(ClientDisconnected);

	public bool ListeningTcp( IPEndPoint endpoint ) => true;
	public bool ListeningUdp( IPEndPoint endpoint ) => true;

	public void NewPacket( Packet packet, Type packetType ) {
		if ( packet is ConnectionFailed connectionFailed ) {
			//Handling Connection failure packet!
			this.LogWarning( $"Disconnection from {connectionFailed.RemoteSender}. Reason: {connectionFailed.Reason}" );
			if ( packet.RemoteSender is not null )
				if ( this._connectionsByTcpEndpoint.TryRemove( packet.RemoteSender, out NetworkConnection? connection ) ) {
					if ( connection.HasName )
						this._namedConnections.Remove( connection.Name, out _ );
					if ( connection.UdpEndPoint is not null )
						this._connectionsByUdpEndpoint.Remove( connection.UdpEndPoint, out _ );
				}
		} else if ( packet is UdpPing udpPing ) {
			if ( udpPing.RemoteSender is null )
				return;
			if ( udpPing.Username == this.Username ) {
				//This is us receiving a response from a ping
				if ( this._connectionsByUdpEndpoint.TryGetValue( udpPing.RemoteSender, out NetworkConnection? connection ) )
					connection.SetPing( Clock64.StartupTime - udpPing.Time );
			} else {
				//This is us responding to a ping
				if ( this.IsServer ) {
					if ( !this._connectionsByUdpEndpoint.ContainsKey( udpPing.RemoteSender ) && this._namedConnections.TryGetValue( udpPing.Username, out NetworkConnection? connection ) ) {
						connection.SetUDPEndpoint( udpPing.RemoteSender );
						this._connectionsByUdpEndpoint.TryAdd( udpPing.RemoteSender, connection );
					}
				}
				udpPing.RemoteTarget = udpPing.RemoteSender;
				Send( udpPing );
			}
		} else if ( packet is TcpLoginAck tcpLoginAck ) {
			if ( tcpLoginAck.RemoteSender is null )
				return;
			if ( tcpLoginAck.Username == this.Username ) {
				//This is us receiving an ack on the login, best to remember out network id, might be important!
				this.ServerProvidedId = tcpLoginAck.NetworkId;
			} else {
				//This is someone else logging in! We should tell the end-user!
				this.LogLine( $"Player {tcpLoginAck.Username} connected!", Log.Level.NORMAL, ConsoleColor.Green );
			}
		} else if ( packet is ClientDisconnected clientDisconnection ) {
			this.LogLine( $"Player {clientDisconnection.Username} disconnected! Reason: {clientDisconnection.Reason}", Log.Level.NORMAL, ConsoleColor.Green );
		} else if ( this.IsServer && packet is TcpLogin tcpLogin ) {
			//Handling Tcp Login packet!
			if ( packet.RemoteSender is not null && this._connectionsByTcpEndpoint.TryGetValue( packet.RemoteSender, out NetworkConnection? connection ) ) {
				if ( !this._namedConnections.TryAdd( tcpLogin.Username, connection ) ) {
					connection.Send( new ConnectionFailed( "Username already taken!" ) );
				} else {
					ulong id = GetNetworkId();
					if ( this._connectionsById.TryAdd( connection.NetworkId, connection ) ) {
						connection.SetNetworkId( GetNetworkId() );
						connection.SetName( tcpLogin.Username );
						Send( new TcpLoginAck( tcpLogin.Username, connection.NetworkId ) );
						NewNamedConnection?.Invoke( connection );
					}
				}
			}
		}
	}

	private ulong GetNetworkId() {
		ulong id = unchecked((ulong) Random.Shared.NextInt64());
		while ( this._connectionsById.ContainsKey( id ) )
			id = unchecked((ulong) Random.Shared.NextInt64());
		return id;
	}

	public void Send( Packet p ) {
		ProtocolType protocol = Resources.Get<PacketTypeManager>().GetPacketProtocol( p.GetType() );
		this.LogLine( $"{( this.IsServer ? "Server" : this.Username )} sending {protocol} message {p.GetType().Name} of length {p.Data.Count} to {( p.RemoteTarget is null ? "all" : p.RemoteTarget )}!", Log.Level.NORMAL, ConsoleColor.Magenta );
		if ( protocol == ProtocolType.Udp ) {
			if ( p.RemoteTarget is not null ) {
				this._udpTunnel?.TrySend( p );
			} else {
				foreach ( NetworkConnection connection in this._connectionsByTcpEndpoint.Values )
					if ( connection.UdpEndPoint is not null )
						this._udpTunnel?.TrySend( p, connection.UdpEndPoint );
			}
		} else {
			if ( p.RemoteTarget is not null ) {
				if ( this._connectionsByTcpEndpoint.TryGetValue( p.RemoteTarget, out NetworkConnection? connection ) ) {
					connection.Send( p );
				} else {
					this.LogWarning( $"Couldn't find connection with endpoint {p.RemoteTarget}!" );
				}
			} else {
				foreach ( NetworkConnection connection in this._connectionsByTcpEndpoint.Values )
					connection.Send( p );
			}
		}
	}

	/// <summary>
	/// Starts receiver for tcp connection.
	/// </summary>
	public void StartUdpReceiver( IPEndPoint bind ) {
		if ( bind is null )
			throw new ArgumentNullException( nameof( bind ) );
		if ( this._udpReceiverThread is not null )
			return;
		if ( this._udpTunnel is null )
			throw new NullReferenceException( "Udp tunnel is null." );
		this._udpTunnel.Bind( bind );
		this._udpReceiverThread = Resources.Get<ThreadManager>().Start( ReceiveUDP, $"Udp Receiver" );
	}

	private void ReceiveUDP() {
		byte[] dataBuffer = new byte[ 512 ];
		this.LogLine( $"{( this.IsServer ? "Server" : this.Username )} receiving Udp messages!", Log.Level.NORMAL );
		while ( this.Open && this._udpTunnel is not null ) {
			if ( this._udpTunnel.TryReceive( dataBuffer, out int length, out IPEndPoint? endpoint ) && endpoint is not null )
				OnMessageReceived( endpoint, ProtocolType.Udp, dataBuffer.Take( length ).ToArray() );
		}
		this.LogLine( $"{( this.IsServer ? "Server" : this.Username )} stopped receiving Udp messages!", Log.Level.NORMAL );
	}

	public void StartTcpConnectionAccepting() {
		if ( this._tcpAcceptingThread is not null )
			return;
		this._tcpAcceptingThread = Resources.Get<ThreadManager>().Start( ReceiveConnections, $"{this._userdata.Username} Tcp Connection Accepter" );
	}

	private void ReceiveConnections() {
		Socket acceptingSocket = this._socketFactory.CreateTcp();
		using ( acceptingSocket ) {
			acceptingSocket.Bind( new IPEndPoint( IPAddress.Any, 12345 ) );
			acceptingSocket.Listen();
			while ( this.Open && this._udpTunnel is not null ) {
				try {
					Socket? newConnectionSocket = acceptingSocket.Accept();
					if ( newConnectionSocket.RemoteEndPoint is not IPEndPoint endpoint || this._connectionsByTcpEndpoint.ContainsKey( endpoint ) ) {
						newConnectionSocket.Close();
						continue;
					}
					this.LogLine( "New Tcp connection!", Log.Level.NORMAL, ConsoleColor.Green );
					NetworkConnection connection = new( this._udpTunnel, newConnectionSocket );
					this._connectionsByTcpEndpoint.TryAdd( endpoint, connection );
					connection.ReceivedTcpMessage += OnMessageReceived;
					connection.OnClosing += OnConnectionClosing;
					connection.StartTCPReceiver();
				} catch ( Exception ex ) {
					this.LogError( ex );
				}
			}
		}
	}

	private void OnConnectionClosing( NetworkConnection connection, string reason ) {
		this.LogWarning( $"Connection {connection.TcpEndPoint}/{connection.Name} closed!" );
		this._connectionsByTcpEndpoint.Remove( connection.TcpEndPoint, out _ );
		if ( connection.UdpEndPoint is not null )
			this._connectionsByUdpEndpoint.Remove( connection.UdpEndPoint, out _ );
		if ( connection.HasName )
			this._namedConnections.Remove( connection.Name, out _ );
		this._connectionsById.Remove( connection.NetworkId, out _ );
		this._pingTimer.Stop();
		connection.Dispose();
		Send( new ClientDisconnected( connection.Name, "Disconnected!" ) );
	}

	public void Close() {
		this.LogLine( $"{( this.IsServer ? "Server" : this.Username )} closing networking!", Log.Level.HIGH, ConsoleColor.DarkCyan );
		this.Open = false;
		this._udpTunnel?.Dispose();
		this._udpTunnel = null;
		foreach ( NetworkConnection? connection in this._connectionsByTcpEndpoint.Values ) {
			connection.ReceivedTcpMessage -= OnMessageReceived;
			connection.OnClosing -= OnConnectionClosing;
			connection.Dispose();
		}
		this._connectionsByTcpEndpoint.Clear();
		this._connectionsByUdpEndpoint.Clear();
		this._namedConnections.Clear();
		this._pingTimer.Stop();
		this._pingTimer.Dispose();
	}

	protected override bool OnDispose() {
		Close();
		return true;
	}
}

public class SocketFactory : ModuleSingletonBase {

	public bool IsIpv6Enabled { get; set; } = false;

	public Socket CreateTcp() {
		if ( this.IsIpv6Enabled ) {
			Socket s = new( AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp );
			s.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0 );
			return s;
		} else {
			Socket s = new( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			return s;
		}
	}

	public Socket CreateUdp() {
		if ( this.IsIpv6Enabled ) {
			Socket s = new( AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp );
			s.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0 );
			return s;
		} else {
			Socket s = new( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
			return s;
		}
	}

	protected override bool OnDispose() => true;
}