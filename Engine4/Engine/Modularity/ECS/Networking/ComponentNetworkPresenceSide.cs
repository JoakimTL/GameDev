namespace Engine.Modularity.ECS.Networking;

public enum ComponentNetworkPresenceSide {
	/// <summary>
	/// This component should ONLY be on clients
	/// </summary>
	CLIENT = 1,
	/// <summary>
	/// This component should ONLY be on servers
	/// </summary>
	SERVER = 2,
	/// <summary>
	/// This component should be present on both clients and servers.
	/// </summary>
	BOTH = 3
}


