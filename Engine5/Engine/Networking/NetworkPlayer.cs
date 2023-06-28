namespace Engine.Networking;

public sealed class NetworkPlayer : Identifiable {

	public string Username { get; private set; }
	public ulong NetworkId { get; }

	public NetworkPlayer( ulong networkId, string username ) {
		this.NetworkId = networkId;
		this.Username = username;
	}

    internal void SetUsername( string username ) => this.Username = username;//TODO: Raise event?

}