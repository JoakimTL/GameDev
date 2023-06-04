using Engine.GlobalServices;
using Engine.Networking.Modules.Services;
using System.Net;

namespace Engine.Networking.Modules.TransferLayer.Tunnels;

public sealed class UdpTunnelPacketReceiver : Identifiable {
	private readonly UdpNetworkTunnel _udpNetworkTunnel;
	private readonly PacketReceptionService _packetReceptionService;
	private readonly PacketTypeRegistryService _packetTypeRegistryService;
	private readonly byte[] dataBuffer;

	protected override string UniqueNameTag => _udpNetworkTunnel?.FullName ?? "Missing UDP tunnel";

	public UdpTunnelPacketReceiver( UdpNetworkTunnel udpNetworkTunnel, PacketReceptionService packetReceptionService, PacketTypeRegistryService packetTypeRegistryService ) {
		_udpNetworkTunnel = udpNetworkTunnel;
		_packetReceptionService = packetReceptionService;
		_packetTypeRegistryService = packetTypeRegistryService;
		dataBuffer = new byte[ 1024 ];
	}

	public void Start( IPEndPoint bind ) {
		_udpNetworkTunnel.Bind( bind );
		Global.Get<ThreadService>().Start( Receive, FullName );
	}

	private void Receive() {
		this.LogLine( "Receiving Udp messages!", Log.Level.NORMAL );
		while ( !_udpNetworkTunnel.Disposed )
			if ( _udpNetworkTunnel.TryReceive( dataBuffer, out uint length, out IPEndPoint? sender ) ) {
				byte[] data = dataBuffer[ sizeof( uint )..(int) length ];
				if ( _packetTypeRegistryService.Construct( data, sender ) is not PacketBase packet ) {
					this.LogWarning( "Unable to construct packet from data. Packet data on VERBOSE log level" );
					this.LogLine( $"Raw packet data: {string.Join( ' ', data.Select( p => p.ToString( "x2" ) ) )}", Log.Level.VERBOSE );
					this.LogLine( $"Char packet data: {string.Join( ' ', data.Select( p => (char) p ) )}", Log.Level.VERBOSE );
					return;
				}
				_packetReceptionService.ReceivedPacket( packet );
			}
		this.LogLine( "Stopped receiving Udp messages!", Log.Level.NORMAL );
	}
}
