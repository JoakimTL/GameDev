namespace Engine.Modularity.ECS.Networking;

public enum ComponentBroadcastSide {
	/// <summary>
	/// This component should ONLY be broadcasted by clients
	/// </summary>
	CLIENT = 1,
	/// <summary>
	/// This component should ONLY be broadcasted by servers
	/// </summary>
	SERVER = 2,
	/// <summary>
	/// This component should be broadcasted by both clients and servers.
	/// </summary>
	BOTH = 3
}


