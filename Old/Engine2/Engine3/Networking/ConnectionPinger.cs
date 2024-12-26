using Engine.MemLib;
using Engine.Networking.Packets;
using Engine.Utilities.IO;
using Engine.Utilities.Time;
using System;
using System.Threading;

namespace Engine.Networking {
	/// <summary>
	/// Sends a small packet on a regular basis.
	/// </summary>
	public class ConnectionPinger {

		private Connection connection;
		public int MS { get; private set; }

		private Timer32 pingTimer;

		public bool TCP;
		public bool UDP;
		public bool UDPF;

		public ConnectionPinger( Connection connection, int ms, bool normal ) {
			this.connection = connection;
			MS = ms;
			TCP = normal;
			UDP = normal;
			UDPF = !normal;

			pingTimer = new Timer32( MS );
			pingTimer.Elapsed += PingFunc;
			pingTimer.Start();
		}

		private void PingFunc() {
			if( UDP )
				connection.Send( new PacketPingUDP( DateTime.Now.Ticks ) );
			if( TCP )
				connection.Send( new PacketPingTCP( DateTime.Now.Ticks ) );
			if( UDPF )
				connection.Send( new PacketUDPForce() );
		}

		public void SetMS( int ms ) {
			if( ms <= 0 )
				return;
			MS = ms;
			pingTimer.SetInterval( ms );
		}

		public void Stop() {
			pingTimer.Stop();
		}
	}
}
