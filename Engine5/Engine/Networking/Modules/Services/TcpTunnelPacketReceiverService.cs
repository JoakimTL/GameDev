using Engine.Networking.Module.TransferLayer.Tunnels;
using Engine.Networking.Modules.Services;
using System.Net;

namespace Engine.Networking.Module.Services;

public class TcpTunnelPacketReceiverService : Identifiable, INetworkClientService {
    private readonly TcpTunnelPacketReceiver _tcpTunnelPacketReceiver;

    public TcpTunnelPacketReceiverService( TcpTunnelService udpTunnelService, PacketReceptionService packetReceptionService, PacketTypeRegistryService packetTypeRegistryService ) {
        _tcpTunnelPacketReceiver = new( udpTunnelService.Tunnel, packetReceptionService, packetTypeRegistryService );
    }

    public void Start() => _tcpTunnelPacketReceiver.Start();
}