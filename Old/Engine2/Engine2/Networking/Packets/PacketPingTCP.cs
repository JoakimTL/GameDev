using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets {
	public class PacketPingTCP : Packet {

		public long Time { get; private set; }

		public PacketPingTCP( long time ) : base( DataTransform.GetBytes( time ) ) {
			Time = time;
		}

		public PacketPingTCP( IPEndPoint remoteEndpoint, IReadOnlyList<byte> inData ) : base( remoteEndpoint, inData ) {
			Time = DataTransform.ToInt64( Content[ 0 ] );
		}
	}
}
