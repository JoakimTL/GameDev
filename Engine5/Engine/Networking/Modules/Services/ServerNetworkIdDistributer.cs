namespace Engine.Networking.Modules.Services;

public class ServerNetworkIdDistributer : Identifiable, INetworkServerService {

	private readonly Random _random;
	private readonly HashSet<uint> _networkIds;

	public ServerNetworkIdDistributer() {
		_random = new();
		_networkIds = new();
	}

	public NetworkId NewConnection() {
		NetworkId id = new( unchecked((uint) _random.Next()) );
		while ( !_networkIds.Add( id ) )
			id = new( unchecked((uint) _random.Next()) );
		this.LogText( $"Network id #{id} dispatched.", Log.Level.VERBOSE );
		return id;
	}

	public void ConnectionClosed( uint networkId ) {
		_networkIds.Remove( networkId );
		this.LogText( $"Network id #{networkId} released.", Log.Level.VERBOSE );
	}
}
