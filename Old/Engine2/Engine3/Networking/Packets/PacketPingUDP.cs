using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets {
	[PacketProtocol( System.Net.Sockets.ProtocolType.Udp )]
	public class PacketPingUDP : Packet {

		public long Time { get; private set; }

		public PacketPingUDP( byte[] data ) : base( data ) {
			if( Segments.Count == 1 )
				Time = DataTransform.ToInt64( bytes, (int) Segments[ 0 ].StartIndex );
		}

		public PacketPingUDP( long time ) : base( GeneratePacketData( typeof( PacketPingUDP ), DataTransform.GetBytes( time ) ) ) {
			Time = time;
		}
	}
}
