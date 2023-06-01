using Engine.Networking.Module;

namespace Engine.Networking.Modules;
public sealed class ServerModule : NetworkModuleBase<INetworkServerService> {
	public override bool IsServer => true;

	public override void Initialize() {
		throw new NotImplementedException();
	}
}

public sealed class ClientModule : NetworkModuleBase<INetworkClientService> {
	public override bool IsServer => false;


	public override void Initialize() {
		throw new NotImplementedException();
	}
}
