using Engine.Networking;
using Engine.Networking.Server;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Engine.QuickstartKit {
	public abstract class QuickstartServer : Quickstart {
		
		private int port;
		public override bool Running { get; protected set; }

		public ConnectionTunnel TunnelUDP { get; protected set; }
		public ConnectionReceiver ConnectionReceiver { get; protected set; }
		public ConnectionList ConnectionList { get; protected set; }
		public PacketReader PacketReader { get; protected set; }
		//public PlayerList PlayerList { get; protected set; }

		public QuickstartServer( int port ) {
			this.port = port;
		}

		public override void Run() {

			Running = true;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Packets();
			PacketType.ClearAndSetup();

			Socket udpSocket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
			udpSocket.Bind( new IPEndPoint( IPAddress.Any, port ) );
			PacketReader = new PacketReader();
			TunnelUDP = new ConnectionTunnel( udpSocket );
			TunnelUDP.CreateReceiver( PacketReader );
			ConnectionReceiver = new ConnectionReceiver( port, SocketType.Stream, ProtocolType.Tcp );
			ConnectionList = new ConnectionList( PacketReader, TunnelUDP.Socket, ConnectionReceiver );
			PacketReader.Add( ConnectionList );
			/*PlayerList = new PlayerList( PacketReader );
			ConnectionList.NewConnectionEvent += PlayerList.PlayerConnected;
			ConnectionList.LostConnectionEvent += PlayerList.PlayerDisconnected;*/

			Entry();

			MemLib.Mem.Dispose();
			Running = false;

		}

	}
}
