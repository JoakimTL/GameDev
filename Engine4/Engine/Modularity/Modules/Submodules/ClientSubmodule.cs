using System.Net;
using Engine.Modularity.Modules.Singletons;
using Engine.Networking;

namespace Engine.Modularity.Modules.Submodules;

[Structure.ProcessAfter( typeof( ServerSubmodule ), typeof( IUpdateable ) )]
public class ClientSubmodule : Submodule {
	public ClientSubmodule() : base( false ) {
		OnInitialization += Initialized;
	}

	protected void Initialized() {
		Singleton<Userdata>().SetUsername( "Test" );
		Singleton<NetworkManager>().StartClient( new IPEndPoint( IPAddress.Loopback, 12345 ) );
	}

	protected override bool OnDispose() {
		Singleton<NetworkManager>().Dispose();
		return true;
	}
}
public class ServerSubmodule : Submodule {
	public ServerSubmodule() : base( true ) {
		OnInitialization += Initialized;
	}

	protected void Initialized() => Singleton<NetworkManager>().StartServer( 12345 );

	protected override bool OnDispose() {
		Singleton<NetworkManager>().Dispose();
		return true;
	}
}
