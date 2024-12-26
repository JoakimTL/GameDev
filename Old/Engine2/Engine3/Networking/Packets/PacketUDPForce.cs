using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	[PacketProtocol( System.Net.Sockets.ProtocolType.Udp )]
	public class PacketUDPForce : Packet {

		public PacketUDPForce( byte[] data ) : base( data ) { }

		public PacketUDPForce() : base( GeneratePacketData( typeof( PacketUDPForce ) ) ) { }

	}
}
