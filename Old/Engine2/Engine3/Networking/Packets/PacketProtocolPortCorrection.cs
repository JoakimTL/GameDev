using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets {
	public class PacketProtocolPortCorrection : Packet {

		public int OverridePort { get; private set; }
		public ProtocolType OverrideProtocol { get; private set; }

		public PacketProtocolPortCorrection( byte[] data ) : base( data ) {
			if( Segments.Count == 2 ) {
				OverrideProtocol = (ProtocolType) DataTransform.ToInt32( data, (int) Segments[ 0 ].StartIndex );
				OverridePort = DataTransform.ToInt32( data, (int) Segments[ 1 ].StartIndex );
			}
		}

		public PacketProtocolPortCorrection( ProtocolType protocol, int port ) : base( GeneratePacketData( typeof( PacketProtocolPortCorrection ), DataTransform.GetBytes( (int) protocol ), DataTransform.GetBytes( port ) ) ) {
			OverrideProtocol = protocol;
			OverridePort = port;
		}
	}
}
