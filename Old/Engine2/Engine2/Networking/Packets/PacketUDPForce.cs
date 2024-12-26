using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketUDPForce : Packet {
		public PacketUDPForce() : base( new string[ 0 ] ) { }

		public PacketUDPForce( IPEndPoint remoteEndpoint, IReadOnlyList<byte> inData ) : base( remoteEndpoint, inData ) { }
	}
}
