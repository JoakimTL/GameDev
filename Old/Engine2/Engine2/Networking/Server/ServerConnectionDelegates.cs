using System.Net.Sockets;

namespace Engine.Networking.Server {
	public delegate void NewConnectionHandler( Socket s );
	public delegate void NewPlayerHandler( Connection c );
}
