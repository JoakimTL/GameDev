using Engine.GlobalServices;
using Engine.Structure.Interfaces;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace Engine.Networking.Module.Services;

public sealed class ConnectionAccepterService : Identifiable, INetworkServerService, IDisposable, IUpdateable {

	public ushort Port { get; private set; }
	private Socket _acceptingSocket;
	private Thread _thread;
	private volatile bool _disposed;
	private readonly ServerPortService _serverPortService;
	private readonly ConcurrentQueue<Socket> _incomingSockets;

	/// <summary>
	/// A new tcp socket has been accepted. This is called from the module thread.
	/// </summary>
	public event Action<Socket>? NewTcpSocket;

	public ConnectionAccepterService( ServerPortService serverPortService, ThreadService threadService, SocketFactory socketFactory ) {
		this._serverPortService = serverPortService;
		_incomingSockets = new();
		_acceptingSocket = socketFactory.CreateTcp();
        _disposed = false;
		_thread = threadService.Start( Thread, "Connection Accepter" );
	}

	private void Thread() {
		_acceptingSocket.Bind( new IPEndPoint( IPAddress.Any, _serverPortService.Port ) );
		_acceptingSocket.Listen();
		while ( !_disposed ) {
			try {
				var newSocket = _acceptingSocket.Accept();
				_incomingSockets.Enqueue( newSocket );
			} catch ( SocketException e ) {
				this.LogError( e );
			} catch ( Exception e ) {
				this.LogError( e );
				Dispose();
			}
		}
	}

	public void Dispose() {
		_acceptingSocket?.Dispose();
		_disposed = true;
	}

	public void Update( float time, float deltaTime ) {
		while(_incomingSockets.TryDequeue( out var socket ) ) 
			NewTcpSocket?.Invoke( socket );
	}
}
