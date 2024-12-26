using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Engine.Networking {
	public class PacketMessage {

		public Packet Packet { get; private set; }
		public IPEndPoint RemoteEndpoint { get; private set; }

		public PacketMessage( Packet p, IPEndPoint e ) {
			Packet = p;
			RemoteEndpoint = e;
		}

	}
}
