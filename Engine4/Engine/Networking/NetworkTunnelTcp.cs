using System.Net;
using System.Net.Sockets;

namespace Engine.Networking;

public class NetworkTunnelTcp : NetworkTunnelBase {

	public override event Action<NetworkTunnelBase, SocketError>? OnAbruptClosure;
	private readonly Socket _socket;

	public override ProtocolType Protocol => ProtocolType.Tcp;

	public NetworkTunnelTcp( Socket tcpSocket ) {
		this._socket = tcpSocket;
	}

	public override bool TryReceive( byte[] buffer, out int length, out IPEndPoint? sender ) {
		sender = null;
		length = -1;
		if ( this.Disposed )
			return false;
		try {
			length = this._socket.Receive( buffer );
			sender = this._socket.RemoteEndPoint as IPEndPoint;
			return true;
		} catch ( SocketException ex ) {
			this.LogWarning( ex.Message );
			OnAbruptClosure?.Invoke( this, ex.SocketErrorCode );
		} catch ( Exception ex ) {
			this.LogError( ex );
		}
		return false;
	}

	public override bool TrySend( Packet p ) {
		if ( this.Disposed )
			return false;
		try {
			unsafe {
				byte* bytes = stackalloc byte[ p.Data.Count + sizeof( int ) ];
				( (int*) bytes )[ 0 ] = p.Data.Count + sizeof( int );
				p.CopyDataTo( bytes, sizeof( int ) );
				Span<byte> data = new( bytes, p.Data.Count + sizeof(int) );
				this._socket.Send( data );
			}
			return true;
		} catch ( SocketException ex ) {
			this.LogWarning( ex.Message );
			OnAbruptClosure?.Invoke( this, ex.SocketErrorCode );
		} catch ( Exception ex ) {
			this.LogError( ex );
		}
		return false;
	}

	private void Close() {
		TrySend( new ConnectionFailed( "Connection closed." ) );
		this._socket.Shutdown( SocketShutdown.Both );
		this._socket.Close();
	}

	protected override bool OnDispose() {
		Close();
		this._socket.Dispose();
		return true;
	}
}
