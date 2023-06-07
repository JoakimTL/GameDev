using System.Net.Sockets;
using System.Net;
using Engine.Networking.Modules.Services;
using Engine.Networking.Modules.TransferLayer.Tunnels;

namespace Engine.Networking.Modules.TransferLayer;

public sealed class ClientConnection : Identifiable, IDisposable {

	private readonly TcpNetworkTunnel _tcpTunnel;
	private readonly TcpTunnelPacketReceiver _tcpTunnelPacketReceiver;
	private readonly UdpNetworkTunnel _udpTunnel;
	public IPEndPoint TcpRemoteTarget { get; private set; }
	public IPEndPoint? UdpRemoteTarget { get; private set; }
	public string? Username { get; private set; }
	public NetworkId NetworkId { get; }
	public bool Disposed { get; private set; }

	public SocketError? Error { get; private set; }

	public event Action<ClientConnection>? ConnectionClosed;

	public ClientConnection( NetworkId networkId, TcpNetworkTunnel tcpTunnel, UdpNetworkTunnel udpTunnel, PacketReceptionService packetReceptionService, PacketTypeRegistryService packetTypeRegistryService ) {
		TcpRemoteTarget = tcpTunnel.RemoteEndPoint as IPEndPoint ?? throw new ArgumentException( "Socket must have an endpoint!" );
		NetworkId = networkId;
		_tcpTunnel = tcpTunnel;
		_tcpTunnel.OnAbruptClosure += AbruptClosure;
		_udpTunnel = udpTunnel;
		_udpTunnel.OnAbruptClosure += AbruptClosure;
		_tcpTunnelPacketReceiver = new( _tcpTunnel, packetReceptionService, packetTypeRegistryService );
		_tcpTunnelPacketReceiver.Start();
	}

	private void AbruptClosure( NetworkTunnelBase @base, SocketError error ) {
		Error = error;
		Dispose();
	}

	internal void SetUsername( string username ) => Username = username;

	internal void SetUdpRemoteTarget( IPEndPoint remoteTarget ) => UdpRemoteTarget = remoteTarget;

	public bool TrySend( PacketBase packet )
		=> !Disposed
			? packet.Protocol switch {
				ProtocolType.Tcp => _tcpTunnel.TrySend( packet ),
				ProtocolType.Udp => UdpRemoteTarget is not null && _udpTunnel.TrySend( packet, UdpRemoteTarget ),
				_ => this.LogWarningThenReturn( $"Unable to send packet {packet}", false ),
			}
			: this.LogWarningThenReturn( $"Connection disposed!", false );

	public void Dispose() {
		if ( Disposed )
			return;
		Disposed = true;
		_tcpTunnelPacketReceiver.Stop();
		_tcpTunnel.Dispose();
		ConnectionClosed?.Invoke( this );
		GC.SuppressFinalize( this );
	}
}