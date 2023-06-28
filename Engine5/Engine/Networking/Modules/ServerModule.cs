using Engine.Networking.Modules.Services;

namespace Engine.Networking.Modules;
public sealed class ServerModule : NetworkModuleBase<INetworkServerService> {
	public override bool IsServer => true;

	public override bool SystemEssential => true;

    public override void Initialize() => Get<ServerPacketDispatchingService>();
}
