using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace Engine.Networking.Modules.TransferLayer;
public abstract class NetworkTunnelBase : Identifiable, IDisposable {
	public abstract event Action<NetworkTunnelBase, SocketError>? OnAbruptClosure;
	public abstract ProtocolType Protocol { get; }
	protected readonly Socket _socket;
	public bool Disposed { get; private set; }

	protected NetworkTunnelBase( Socket socket ) {
		_socket = socket;
	}

#if DEBUG
	~NetworkTunnelBase() {
		Debug.Fail( "Tunnel not disposed!" );
	}
#endif

	public void Bind( EndPoint endPoint ) {
		_socket.Bind( endPoint );
		this.LogLine( $"Set local endpoint to {_socket.LocalEndPoint}!", Log.Level.NORMAL );
	}

	public EndPoint? LocalEndPoint => _socket.LocalEndPoint;
	public EndPoint? RemoteEndPoint => _socket.RemoteEndPoint;

	public abstract bool TryReceive( byte[] buffer, out uint length, out IPEndPoint? sender );
	public abstract bool TrySend( PacketBase p );

	public void Dispose() {
		OnDispose();
		Disposed = true;
		GC.SuppressFinalize( this );
	}

	protected virtual void OnDispose() { }
}
