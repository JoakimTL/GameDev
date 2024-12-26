using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Engine.Networking {
	public class PacketProtocol : Attribute {

		public ProtocolType Protocol { get; private set; }

		public PacketProtocol( ProtocolType protocol ) {
			Protocol = protocol;
		}

	}
}
