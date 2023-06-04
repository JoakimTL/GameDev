using System.Net.Sockets;
using System.Net;
using Engine.Networking.Modules.Services;

namespace Engine.Networking.Modules.TransferLayer.Tunnels;

public sealed class TcpNetworkTunnel : NetworkTunnelBase {
	private readonly PacketTypeRegistryService _packetTypeRegistryService;

	public override event Action<NetworkTunnelBase, SocketError>? OnAbruptClosure;
	public override ProtocolType Protocol => ProtocolType.Tcp;

	public TcpNetworkTunnel( Socket tcpSocket, PacketTypeRegistryService packetTypeRegistryService ) : base( tcpSocket ) {
		_packetTypeRegistryService = packetTypeRegistryService;
	}

	internal bool Connect( IPEndPoint remoteTarget ) {
		if ( Disposed )
			return this.LogWarningThenReturn( $"Tunnel disposed!", false );
		try {
			_socket.Connect( remoteTarget );
			return true;
		} catch ( SocketException e ) {
			this.LogError( e );
		} catch ( Exception e ) {
			this.LogError( e );
		}
		return false;
	}

	internal void Disconnect( PacketBase? finalMessage ) {
		if ( Disposed ) {
			this.LogWarningThenReturn( $"Tunnel disposed!", false );
			return;
		}
		if ( finalMessage is not null )
			TrySend( finalMessage );
		try {
			_socket.Disconnect( true );
		} catch ( SocketException e ) {
			this.LogError( e );
		} catch ( Exception e ) {
			this.LogError( e );
		}
	}

	public override bool TryReceive( byte[] buffer, out uint length, out IPEndPoint? sender ) {
		sender = null;
		length = 0;
		if ( Disposed )
			return this.LogWarningThenReturn( $"Tunnel disposed!", false );
		try {
			length = (uint) _socket.Receive( buffer );
			sender = RemoteEndPoint as IPEndPoint;
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
		try {
			var id = _packetTypeRegistryService.GetPacketTypeId( p.GetType() );
			if ( id == -1 )
				return this.LogWarningThenReturn( $"{p} has no identification!", false );
			var data = p.GetPacketTransferData( id );
			if ( data is null )
				return this.LogWarningThenReturn( $"Data from {p} returned NULL!", false );
			_socket.Send( data );
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
		TrySend( new Packets.ConnectionFailed( "Connection closed." ) );
		_socket.Shutdown( SocketShutdown.Both );
		_socket.Close();
	}

	protected override void OnDispose() {
		Close();
		_socket.Dispose();
	}
}
