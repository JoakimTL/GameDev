using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Networking.Client {
	public class ClientConnection {

		public PacketReader Reader { get; private set; }
		public Connection Connection { get; private set; }

		public ClientConnection() {
			Reader = new PacketReader();
			Connection = new Connection( Reader );
		}

	}
}
