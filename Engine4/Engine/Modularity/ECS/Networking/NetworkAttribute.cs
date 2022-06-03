using System.Net.Sockets;

namespace Engine.Modularity.ECS.Networking;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
public class NetworkAttribute : Attribute {

	/// <summary>
	/// Which side should this component be present on? Determines if a client will accept a component from the server, or if a server will accept a component from the client.<br />
	/// </summary>
	public readonly ComponentNetworkPresenceSide Presence;
	/// <summary>
	/// When transfering update packets, this protocol will be used.<br />
	/// </summary>
	public readonly ProtocolType UpdateProtocol;
	/// <summary>
	/// Which side can broadcast updates to this component.<br />
	/// An example would be player input, which only clients can change. Servers will accept changes, and can't broadcast them.
	/// </summary>
	public readonly ComponentBroadcastSide Broadcast;

	public NetworkAttribute( ComponentBroadcastSide broadcastSide, ProtocolType updateProtocol, ComponentNetworkPresenceSide presenceSide = ComponentNetworkPresenceSide.BOTH ) {
		this.Broadcast = broadcastSide;
		this.UpdateProtocol = updateProtocol;
		this.Presence = presenceSide;
	}
}


