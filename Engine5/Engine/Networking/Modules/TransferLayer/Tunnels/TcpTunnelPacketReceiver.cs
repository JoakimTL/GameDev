using Engine.GlobalServices;
using Engine.Networking.Module.Services;
using Engine.Networking.Modules.Services;
using System.Net;

namespace Engine.Networking.Module.TransferLayer.Tunnels;

public sealed class TcpTunnelPacketReceiver : Identifiable {
    private readonly TcpNetworkTunnel _tcpNetworkTunnel;
    private readonly PacketReceptionService _packetReceptionService;
    private readonly PacketTypeRegistryService _packetTypeRegistryService;
    private readonly DataReceiver _dataReceiver;
    private readonly byte[] dataBuffer;

    protected override string UniqueNameTag => _tcpNetworkTunnel?.FullName ?? "Missing TCP tunnel";

    public TcpTunnelPacketReceiver( TcpNetworkTunnel tcpNetworkTunnel, PacketReceptionService packetReceptionService, PacketTypeRegistryService packetTypeRegistryService ) {
        this._tcpNetworkTunnel = tcpNetworkTunnel;
        this._packetReceptionService = packetReceptionService;
        this._packetTypeRegistryService = packetTypeRegistryService;
        dataBuffer = new byte[ 1024 ];
        _dataReceiver = new();
        _dataReceiver.MessageComplete += MessageComplete;
    }

    public void Start() {
        Global.Get<ThreadService>().Start( Receive, this.FullName );
    }

    private void MessageComplete( byte[] data ) {
        var ipEndPoint = _tcpNetworkTunnel.RemoteEndPoint as IPEndPoint;
        if ( ipEndPoint is null )
            this.LogWarning( "Remote endpoint missing." );
		if ( _packetTypeRegistryService.Construct( data, ipEndPoint ) is not PacketBase packet ) {
            this.LogWarning( "Unable to construct packet from data. Packet data on VERBOSE log level" );
            this.LogLine( $"Raw packet data: {string.Join( ' ', data.Select( p => p.ToString( "x4" ) ) )}", Log.Level.VERBOSE );
            this.LogLine( $"Char packet data: {string.Join( ' ', data.Select( p => (char) p ) )}", Log.Level.VERBOSE );
            return;
        }
        _packetReceptionService.ReceivedPacket( packet );
    }

    private void Receive() {
        this.LogLine( "Receiving Tcp messages!", Log.Level.NORMAL );
        while ( !_tcpNetworkTunnel.Disposed ) {
            if ( this._tcpNetworkTunnel.TryReceive( dataBuffer, out uint length, out _ ) )
                _dataReceiver.ReceivedData( dataBuffer, length );
        }
        this.LogLine( "Stopped receiving Tcp messages!", Log.Level.NORMAL );
    }
}
