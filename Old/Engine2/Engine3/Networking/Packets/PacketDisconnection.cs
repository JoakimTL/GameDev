using Engine.Utilities.Data;
using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketDisconnection : Packet {

		public string Username { get; private set; }

		public PacketDisconnection( byte[] data ) : base( data ) {
			if( Segments.Count == 1 )
				Username = DataTransform.ToString( bytes, (int) Segments[ 0 ].Length, (int) Segments[ 0 ].StartIndex );
		}

		public PacketDisconnection( string username ) : base( GeneratePacketData( typeof( PacketDisconnection ), DataTransform.GetBytes( username ) ) ) {
			Username = username;
		}

	}
}
