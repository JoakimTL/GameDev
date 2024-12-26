using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketConnection : Packet {

		public string Username { get; private set; }

		public PacketConnection( string username ) : base( username ) {
			Username = username;
		}

		public PacketConnection( IPEndPoint remoteEndpoint, IReadOnlyList<byte> inData ) : base( remoteEndpoint, inData ) {
			Username = Content[ 0 ];
		}
	}
}
