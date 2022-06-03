using Engine.Modularity.Modules;

namespace Engine.Modularity.ECS.Networking;
public class ClientEntityControllerManager : ModuleSingletonBase {
	protected override bool OnDispose() => true;
}
