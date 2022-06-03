using System.Net;
using System.Net.Sockets;

namespace Engine.Networking;

public class NetworkTunnelUdp : NetworkTunnelBase {

	public override event Action<NetworkTunnelBase, SocketError>? OnAbruptClosure;
	private readonly Socket _socket;

	public override ProtocolType Protocol => ProtocolType.Udp;

	public NetworkTunnelUdp(Socket udpSocket) {
		this._socket = udpSocket;
	}

	public void Bind( IPEndPoint endPoint ) => this._socket.Bind( endPoint );

	public override bool TryReceive( byte[] buffer, out int length, out IPEndPoint? sender ) {
		sender = null;
		length = -1;
		if ( this.Disposed )
			return false;
		try {
			EndPoint e = new IPEndPoint( IPAddress.None, 0 );
			length = this._socket.ReceiveFrom( buffer, ref e );
			sender = e as IPEndPoint;
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
		if ( p.RemoteTarget is null )
			return false;
		return TrySend( p, p.RemoteTarget );
	}

	public bool TrySend( Packet p, IPEndPoint endpoint ) {
		if ( this.Disposed )
			return false;
		try {
			this._socket.SendTo( p.Data.ToArray(), endpoint );
			return true;
		} catch ( SocketException ex ) {
			this.LogWarning( ex.Message );
			OnAbruptClosure?.Invoke( this, ex.SocketErrorCode );
		} catch ( Exception ex ) {
			this.LogError( ex );
		}
		return false;
	}

	private void Close() => this._socket.Close();
	protected override bool OnDispose() {
		Close();
		return true;
	}
}
