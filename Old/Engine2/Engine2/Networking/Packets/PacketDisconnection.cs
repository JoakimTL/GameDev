using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketDisconnection : Packet {
		
		public string Username { get; private set; }

		public PacketDisconnection( string username ) : base( username ) {
			Username = username;
		}

		public PacketDisconnection( IPEndPoint remoteEndpoint, IReadOnlyList<byte> inData ) : base( remoteEndpoint, inData ) {
			Username = Content[ 0 ];
		}
	}
}
