using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets {
	public class PacketPingTCP : Packet {

		public long Time { get; private set; }

		public PacketPingTCP( byte[] data ) : base( data ) {
			if( Segments.Count == 1 )
				Time = DataTransform.ToInt64( bytes, (int) Segments[ 0 ].StartIndex );
		}

		public PacketPingTCP( long time ) : base( GeneratePacketData( typeof( PacketPingTCP ), DataTransform.GetBytes( time ) ) ) {
			Time = time;
		}
	}
}
