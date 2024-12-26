using Engine.Utilities.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketConnection : Packet {

		public string Username { get; private set; }

		public PacketConnection( byte[] data ) : base( data ) {
			if( Segments.Count == 1 )
				Username = DataTransform.ToString( bytes, (int) Segments[ 0 ].Length, (int) Segments[ 0 ].StartIndex );
		}

		public PacketConnection( string username ) : base( GeneratePacketData( typeof( PacketConnection ), DataTransform.GetBytes( username ) ) ) {
			Username = username;
		}
	}
}
