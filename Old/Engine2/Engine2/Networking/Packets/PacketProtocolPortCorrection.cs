using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets {
	public class PacketProtocolPortCorrection : Packet {

		public int Port { get; private set; }
		public ProtocolType Protocol { get; private set; }

		public PacketProtocolPortCorrection( ProtocolType protocol, int port ) : base( DataTransform.GetBytes( (int) protocol ), DataTransform.GetBytes( port ) ) {
			Protocol = protocol;
			Port = port;
		}

		public PacketProtocolPortCorrection( IPEndPoint remoteEndpoint, IReadOnlyList<byte> inData ) : base( remoteEndpoint, inData ) {
			Protocol = (ProtocolType) DataTransform.ToInt32( Content[ 0 ] );
			Port = DataTransform.ToInt32( Content[ 1 ] );
		}
	}
}
