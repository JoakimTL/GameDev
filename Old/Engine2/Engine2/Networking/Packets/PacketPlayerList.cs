using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketPlayerList : Packet {

		public PacketPlayerList( string[] nameList ) : base( nameList ) { }

		public PacketPlayerList( IPEndPoint remoteEndpoint, IReadOnlyList<byte> inData ) : base( remoteEndpoint, inData ) { }

	}
}
