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

		public ServerAdmin Server { get; private set; }

		public QuickstartServer( int port ) {
			this.port = port;
		}

		public override void Run() {

			Running = true;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			PacketManager.ScanPacketTypes();

			Server = new ServerAdmin( port );

			Entry();

			MemLib.Mem.Dispose();
			Running = false;

		}

	}
}
