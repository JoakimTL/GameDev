using System.Net.Sockets;

namespace Engine.Networking {
	public delegate void ConnectionFailureHandler( Socket s, SocketException e );
	public delegate void ConnectionClosedHandler( Connection c );
}
