using Engine.Networking.Modules.TransferLayer;
using System.Net.Sockets;

namespace Engine.Networking.Modules.Services;

public sealed class ConnectedPlayerAddressService : Identifiable, INetworkServerService {

	private readonly Dictionary<string, Dictionary<ProtocolType, NetworkTunnelBase>> _tunnels;

	public ConnectedPlayerAddressService() {
		_tunnels = new();
	}

	/// <param name="replacedTunnel">The tunnel being replaced. If no tunnel was replaced this value is null.</param>
	public void AddConnection( string name, NetworkTunnelBase tunnel, out NetworkTunnelBase? replacedTunnel ) {
		if ( !_tunnels.TryGetValue( name, out var namedTunnels ) )
			_tunnels.Add( name, namedTunnels = new() );
		namedTunnels.TryGetValue( tunnel.Protocol, out replacedTunnel );
		namedTunnels[ tunnel.Protocol ] = tunnel;
	}

	/// <returns>The cleared connections for potential cleanup</returns>
	public IEnumerable<NetworkTunnelBase> ClearConnections( string name ) {
		_tunnels.Remove( name, out var namedTunnels );
		var tunnels = namedTunnels?.Values ?? Enumerable.Empty<NetworkTunnelBase>();
		return tunnels;
	}


}