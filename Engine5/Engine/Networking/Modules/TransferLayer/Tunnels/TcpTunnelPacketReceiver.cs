using Engine.GlobalServices;
using Engine.Networking.Modules.Services;
using System.Net;

namespace Engine.Networking.Modules.TransferLayer.Tunnels;

public sealed class TcpTunnelPacketReceiver : Identifiable {
	private readonly TcpNetworkTunnel _tcpNetworkTunnel;
	private readonly PacketReceptionService _packetReceptionService;
	private readonly PacketTypeRegistryService _packetTypeRegistryService;
	private readonly DataReceiver _dataReceiver;
	private readonly byte[] dataBuffer;
	private readonly AutoResetEvent _entry;
	private bool _active;

	protected override string UniqueNameTag => _tcpNetworkTunnel?.FullName ?? "Missing TCP tunnel";

	public TcpTunnelPacketReceiver( TcpNetworkTunnel tcpNetworkTunnel, PacketReceptionService packetReceptionService, PacketTypeRegistryService packetTypeRegistryService ) {
		_tcpNetworkTunnel = tcpNetworkTunnel;
		_packetReceptionService = packetReceptionService;
		_packetTypeRegistryService = packetTypeRegistryService;
		dataBuffer = new byte[ 1024 ];
		_dataReceiver = new();
		_dataReceiver.MessageComplete += MessageComplete;
		_entry = new( false );
		Global.Get<ThreadService>().Start( Receive, FullName );
	}

	public void Start() {
		_active = true;
		_entry.Set();
	}

	public void Stop() {
		_active = false;
	}

	private void MessageComplete( byte[] data ) {
		var ipEndPoint = _tcpNetworkTunnel.RemoteEndPoint as IPEndPoint;
		if ( ipEndPoint is null )
			this.LogWarning( "Remote endpoint missing." );
		if ( _packetTypeRegistryService.Construct( data, ipEndPoint ) is not PacketBase packet ) {
			this.LogWarning( "Unable to construct packet from data. Packet data on VERBOSE log level" );
			this.LogLine( $"Raw packet data: {string.Join( ' ', data.Select( p => p.ToString( "x2" ) ) )}", Log.Level.VERBOSE );
			this.LogLine( $"Char packet data: {string.Join( ' ', data.Select( p => (char) p ) )}", Log.Level.VERBOSE );
			return;
		}
		_packetReceptionService.ReceivedPacket( packet );
	}

	private void Receive() {
		while ( !_tcpNetworkTunnel.Disposed ) {
			if ( !_entry.WaitOne( 100 ) )
				continue;
			this.LogLine( "Receiving Tcp messages!", Log.Level.NORMAL );
			while ( _active && !_tcpNetworkTunnel.Disposed )
				if ( _tcpNetworkTunnel.TryReceive( dataBuffer, out uint length, out _ ) )
					_dataReceiver.ReceivedData( dataBuffer, length );
			this.LogLine( "Stopped receiving Tcp messages!", Log.Level.NORMAL );
		}
	}
}
