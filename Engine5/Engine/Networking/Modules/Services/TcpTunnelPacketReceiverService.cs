using Engine.Networking.Modules.TransferLayer.Tunnels;

namespace Engine.Networking.Modules.Services;

public class TcpTunnelPacketReceiverService : Identifiable, INetworkClientService {
	private readonly TcpTunnelPacketReceiver _tcpTunnelPacketReceiver;

	public TcpTunnelPacketReceiverService( TcpTunnelService udpTunnelService, PacketReceptionService packetReceptionService, PacketTypeRegistryService packetTypeRegistryService ) {
		_tcpTunnelPacketReceiver = new( udpTunnelService.Tunnel, packetReceptionService, packetTypeRegistryService );
	}

	public void Start() => _tcpTunnelPacketReceiver.Start();
	public void Stop() => _tcpTunnelPacketReceiver.Stop();
}