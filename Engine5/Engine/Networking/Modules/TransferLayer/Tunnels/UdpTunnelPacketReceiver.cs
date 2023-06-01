using Engine.GlobalServices;
using Engine.Networking.Module.Services;
using Engine.Networking.Modules.Services;
using System.Net;

namespace Engine.Networking.Module.TransferLayer.Tunnels;

public sealed class UdpTunnelPacketReceiver : Identifiable {
    private readonly UdpNetworkTunnel _udpNetworkTunnel;
    private readonly PacketReceptionService _packetReceptionService;
    private readonly PacketTypeRegistryService _packetTypeRegistryService;
    private readonly byte[] dataBuffer;

    protected override string UniqueNameTag => _udpNetworkTunnel?.FullName ?? "Missing UDP tunnel";

    public UdpTunnelPacketReceiver( UdpNetworkTunnel udpNetworkTunnel, PacketReceptionService packetReceptionService, PacketTypeRegistryService packetTypeRegistryService ) {
        this._udpNetworkTunnel = udpNetworkTunnel;
        this._packetReceptionService = packetReceptionService;
        this._packetTypeRegistryService = packetTypeRegistryService;
        dataBuffer = new byte[ 1024 ];
    }

    public void Start( IPEndPoint bind ) {
        this._udpNetworkTunnel.Bind( bind );
        Global.Get<ThreadService>().Start( Receive, this.FullName );
    }

    private void Receive() {
        this.LogLine( "Receiving Udp messages!", Log.Level.NORMAL );
        while ( !_udpNetworkTunnel.Disposed ) {
            if ( this._udpNetworkTunnel.TryReceive( dataBuffer, out uint length, out IPEndPoint? sender ) ) {
                var data = dataBuffer.Take( (int) length ).ToArray();
                if ( _packetTypeRegistryService.Construct( data, sender ) is not PacketBase packet ) {
                    this.LogWarning( "Unable to construct packet from data. Packet data on VERBOSE log level" );
                    this.LogLine( $"Raw packet data: {string.Join( ' ', data.Select( p => p.ToString( "x4" ) ) )}", Log.Level.VERBOSE );
                    this.LogLine( $"Char packet data: {string.Join( ' ', data.Select( p => (char) p ) )}", Log.Level.VERBOSE );
                    return;
                }
                _packetReceptionService.ReceivedPacket( packet );
            }
        }
        this.LogLine( "Stopped receiving Udp messages!", Log.Level.NORMAL );
    }
}
