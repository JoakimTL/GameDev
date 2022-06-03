namespace Engine.Modularity.ECS.Networking;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
public class NetworkProtocolAttribute : Attribute {

	public readonly System.Net.Sockets.ProtocolType Protocol;

	public NetworkProtocolAttribute( System.Net.Sockets.ProtocolType protocol ) {
		this.Protocol = protocol;
	}
}


