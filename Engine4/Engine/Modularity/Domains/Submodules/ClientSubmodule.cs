//using System.Net;
//using Engine.Modularity.Modules.Singletons;
//using Engine.Networking;
//using Engine.Structure;

//namespace Engine.Modularity.Modules.Submodules;

//[Structure.ProcessAfter( typeof( ServerSubmodule ), typeof( IUpdateable ) )]
//public class ClientSubmodule : Submodule {
//	public ClientSubmodule() : base( false ) {
//		OnInitialization += Initialized;
//	}

//	protected void Initialized() {
//		Console.WriteLine( "Gimme username: " );
//		string? username = Console.ReadLine();
//		if (username is null)
//			Console.WriteLine("weird");
//		Singleton<Userdata>().SetUsername( username ?? "NoName" );
//		Console.WriteLine("Write server IP: ");
//		string? ipString = Console.ReadLine();
//		if ( ipString is null )
//			Console.WriteLine( "really weird" );
//		IPAddress address = IPAddress.Parse( ipString ?? "127.0.0.1" );
//		Singleton<NetworkManager>().StartClient( new IPEndPoint( address, 12345 ) );
//	}

//	protected override bool OnDispose() {
//		Singleton<NetworkManager>().Dispose();
//		return true;
//	}
//}
//public class ServerSubmodule : Submodule {
//	public ServerSubmodule() : base( true ) {
//		OnInitialization += Initialized;
//	}

//	protected void Initialized() => Singleton<NetworkManager>().StartServer( 12345 );

//	protected override bool OnDispose() {
//		Singleton<NetworkManager>().Dispose();
//		return true;
//	}
//}
