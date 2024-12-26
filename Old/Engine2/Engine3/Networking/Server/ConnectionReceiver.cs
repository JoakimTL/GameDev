using Engine.MemLib;
using Engine.Utilities.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Engine.Networking.Server {
	public class ConnectionReceiver {

		private Socket receivingSocket;
		private Thread receivingThread;

		public event NewConnectionHandler NewConnectionEvent;

		public ConnectionReceiver( int port, SocketType socketType, ProtocolType protocol ) {
			receivingSocket = new Socket( AddressFamily.InterNetworkV6, socketType, protocol );
			receivingSocket.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false );
			receivingSocket.Bind( new IPEndPoint( IPAddress.IPv6Any, port ) );
			receivingSocket.Listen( 1 );
			receivingThread = Mem.Threads.StartNew( this.AcceptSockets, "Socket Receiver" );
		}

		private void AcceptSockets() {
			try {
				while( Mem.Threads.Running ) {
					Socket s = receivingSocket.Accept();
					NewConnectionEvent?.Invoke( s );
				}
			} catch {
				Mem.Logs.Routine.WriteLine( "Accepting Socket Closed!" );
			}
		}
	}
}
