using Engine.Networking.Modules.TransferLayer.Tunnels;
using System.Net;

namespace Engine.Networking.Modules.Services;

public sealed class UdpTunnelPacketReceiverService : Identifiable, INetworkClientService, INetworkServerService {
	private readonly UdpTunnelPacketReceiver _udpTunnelPacketReceiver;

	public UdpTunnelPacketReceiverService( UdpTunnelService udpTunnelService, PacketReceptionService packetReceptionService, PacketTypeRegistryService packetTypeRegistryService ) {
		_udpTunnelPacketReceiver = new( udpTunnelService.Tunnel, packetReceptionService, packetTypeRegistryService );
	}

	public void Start( IPEndPoint bind ) => _udpTunnelPacketReceiver.Start( bind );
}
