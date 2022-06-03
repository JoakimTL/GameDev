using System.Net;
using System.Net.Sockets;

namespace Engine.Networking;

public abstract class NetworkTunnelBase : DisposableIdentifiable {
	public abstract event Action<NetworkTunnelBase, SocketError>? OnAbruptClosure;
	public abstract ProtocolType Protocol { get; }
	public abstract bool TryReceive( byte[] buffer, out int length, out IPEndPoint? sender );
	public abstract bool TrySend( Packet p );
}
