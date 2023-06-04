using Engine.Networking.Modules.Services;

namespace Engine.Networking.Modules;

public sealed class ClientModule : NetworkModuleBase<INetworkClientService> {
	public override bool IsServer => false;

	public override bool SystemEssential => false;

	public override void Initialize() {
		Get<ClientPacketDispatchingService>();
		Get<ClientConnectionService>();
		Get<UdpTunnelPingService>();
	}
}
