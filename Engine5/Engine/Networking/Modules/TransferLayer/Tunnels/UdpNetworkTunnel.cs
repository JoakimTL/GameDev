using System.Net.Sockets;
using System.Net;
using Engine.Networking.Modules.Services;

namespace Engine.Networking.Module.TransferLayer.Tunnels;

public class UdpNetworkTunnel : NetworkTunnelBase {
	private readonly PacketTypeRegistryService _packetTypeRegistryService;

	public override event Action<NetworkTunnelBase, SocketError>? OnAbruptClosure;

	public override ProtocolType Protocol => ProtocolType.Udp;

	public UdpNetworkTunnel( Socket udpSocket, PacketTypeRegistryService packetTypeRegistryService ) : base( udpSocket ) {
		this._packetTypeRegistryService = packetTypeRegistryService;
	}

	public override bool TryReceive( byte[] buffer, out uint length, out IPEndPoint? sender ) {
		sender = null;
		length = 0;
		if ( Disposed )
			return this.LogWarningThenReturn( $"Tunnel disposed!", false );
		try {
			EndPoint e = new IPEndPoint( IPAddress.None, 0 );
			length = (uint) _socket.ReceiveFrom( buffer, ref e );
			sender = e as IPEndPoint;
			return true;
		} catch ( SocketException ex ) {
			this.LogWarning( ex.Message );
			OnAbruptClosure?.Invoke( this, ex.SocketErrorCode );
		} catch ( ObjectDisposedException ) {
			this.LogWarning( "Socket disposed!" );
		} catch ( Exception ex ) {
			this.LogError( ex );
		}
		return false;
	}

	public override bool TrySend( PacketBase p ) {
		if ( Disposed )
			return this.LogWarningThenReturn( $"Tunnel disposed!", false );
		if ( p.RemoteTarget is null )
			return this.LogWarningThenReturn( $"Packet must have a remote target!", false );
		return TrySend( p, p.RemoteTarget );
	}

	public bool TrySend( PacketBase p, IPEndPoint endpoint ) {
		if ( Disposed )
			return this.LogWarningThenReturn( $"Tunnel disposed!", false );
		try {
			var id = _packetTypeRegistryService.GetPacketTypeId( p.GetType() );
			if ( id == -1 )
				return this.LogWarningThenReturn( $"{p} has no identification!", false );
			var data = p.GetPacketTransferData( id );
			if ( data is null )
				return this.LogWarningThenReturn( $"Data from {p} returned NULL!", false );
			_socket.SendTo( data, endpoint );
			return true;
		} catch ( SocketException ex ) {
			this.LogWarning( ex.Message );
			OnAbruptClosure?.Invoke( this, ex.SocketErrorCode );
		} catch ( ObjectDisposedException ) {
			this.LogWarning( "Socket disposed!" );
		} catch ( Exception ex ) {
			this.LogError( ex );
		}
		return false;
	}

	private void Close() {
		_socket.Shutdown( SocketShutdown.Both );
		_socket.Close();
	}

	protected override void OnDispose() {
		Close();
		_socket.Dispose();
	}
}
