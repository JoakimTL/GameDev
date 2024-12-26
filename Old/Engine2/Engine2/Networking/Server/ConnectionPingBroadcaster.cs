using Engine.MemLib;
using Engine.Networking.Packets;
using Engine.Utilities.IO;
using Engine.Utilities.Time;
using System;

namespace Engine.Networking.Server {
	class ConnectionPingBroadcaster {

		private ConnectionList connections;
		public int MS { get; private set; }

		private Timer32 broadcastTimer;

		public ConnectionPingBroadcaster( ConnectionList connections, int ms ) {
			this.connections = connections;
			MS = ms;
			broadcastTimer = new Timer32( MS );
			broadcastTimer.Elapsed += BroadcastFunc;
			broadcastTimer.Start();
		}

		private void BroadcastFunc() {
			connections.SendAll( new PacketPingTimes( this.connections.Connections ) );
		}

		public void Start() {
			broadcastTimer.Start();
		}

		public void Stop() {
			broadcastTimer.Stop();
		}
	}
}
