using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketRejection : Packet {
		
		public string Message { get; private set; }

		public PacketRejection( string message ) : base( message ) {
			Message = message;
		}

		public PacketRejection( IPEndPoint remoteEndpoint, IReadOnlyList<byte> inData ) : base( remoteEndpoint, inData ) {
			Message = Content[ 0 ];
		}
	}
}
