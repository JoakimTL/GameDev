using Engine.Networking.Modules;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices.Network;

public sealed class NetworkAuthorityService : Identifiable, IGlobalService {

	public bool IsAuthoritative { get; private set; }

	public NetworkAuthorityService() {
		IsAuthoritative = Global.Get<ModuleContainerService>().HasModule<ServerModule>();
		Global.Get<ModuleContainerService>().ModuleAdded += ModuleAdded;
		Global.Get<ModuleContainerService>().ModuleRemoved += ModuleRemoved;
	}

	private void ModuleRemoved( Type type ) {
		if ( type.IsAssignableTo( typeof( ServerModule ) ) )
			IsAuthoritative = false;
	}

	private void ModuleAdded( Type type ) {
		if ( type.IsAssignableTo( typeof( ServerModule ) ) )
			IsAuthoritative = true;
	}
}
