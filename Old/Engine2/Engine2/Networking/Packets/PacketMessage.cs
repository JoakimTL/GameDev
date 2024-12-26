using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketMessage : Packet {

		public string Username { get; private set; }
		public string Message { get; private set; }

		public PacketMessage( string username, string message ) : base( username, message ) {
			Username = username;
			Message = message;
		}

		public PacketMessage( IPEndPoint remoteEndpoint, IReadOnlyList<byte> inData ) : base( remoteEndpoint, inData ) {
			Username = Content[ 0 ];
			Message = Content[ 1 ];
		}
	}
}
