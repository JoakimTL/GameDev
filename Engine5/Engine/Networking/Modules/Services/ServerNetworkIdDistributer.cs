namespace Engine.Networking.Module.Services;

public class ServerNetworkIdDistributer : Identifiable, INetworkServerService {

	private readonly Random _random;
	private readonly HashSet<ulong> _networkIds;

    public ServerNetworkIdDistributer()
    {
		_random = new();
		_networkIds = new();
	}

    public ulong NewConnection() {

		ulong id = unchecked((ulong) _random.NextInt64());
		while ( !this._networkIds.Add( id ) )
			id = unchecked((ulong) _random.NextInt64());
		return id;
	}

	public void ConnectionClosed( ulong networkId ) => _networkIds.Remove( networkId );

}
