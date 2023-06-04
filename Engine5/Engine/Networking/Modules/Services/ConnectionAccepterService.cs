using Engine.GlobalServices;
using Engine.Structure.Interfaces;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Modules.Services;

public sealed class ConnectionAccepterService : Identifiable, INetworkServerService, IDisposable, IUpdateable {

	public ushort Port { get; private set; }
	private Socket _acceptingSocket;
	private Thread _thread;
	private volatile bool _disposed;
	private readonly ServerPortService _serverPortService;
	private readonly ConcurrentQueue<Socket> _incomingSockets;

	public bool HasIncomingSockets => !_incomingSockets.IsEmpty;

	/// <summary>
	/// A new tcp socket has been accepted. This is called from the module thread.
	/// </summary>
	public event Action<Socket>? NewTcpSocket;

	public ConnectionAccepterService( ServerPortService serverPortService, ThreadService threadService, SocketFactory socketFactory ) {
		_serverPortService = serverPortService;
		_incomingSockets = new();
		_acceptingSocket = socketFactory.CreateTcp();
		_disposed = false;
		_thread = threadService.Start( Thread, "Connection Accepter", false );
	}

	private void Thread() {
		_acceptingSocket.Bind( new IPEndPoint( IPAddress.Any, _serverPortService.Port ) );
		_acceptingSocket.Listen();
		this.LogLine( $"Accepting TCP connections on port {_serverPortService.Port}", Log.Level.NORMAL );
		while ( !_disposed )
			try {
				var newSocket = _acceptingSocket.Accept();
				_incomingSockets.Enqueue( newSocket );
				this.LogLine( $"New connection from {newSocket.RemoteEndPoint}!", Log.Level.VERBOSE );
			} catch ( SocketException e ) {
				if ( e.SocketErrorCode == SocketError.Interrupted ) {
					Dispose();
					break;
				}
				this.LogError( e );
			} catch ( Exception e ) {
				this.LogError( e );
				Dispose();
			}
		this.LogLine( $"Stopped accepting TCP connections", Log.Level.NORMAL );
	}

	public void Dispose() {
		if ( _disposed )
			return;
		_disposed = true;
		_acceptingSocket?.Close();
		_acceptingSocket?.Dispose();
	}

	public void Update( float time, float deltaTime ) {
		while ( _incomingSockets.TryDequeue( out var socket ) )
			NewTcpSocket?.Invoke( socket );
	}
}
