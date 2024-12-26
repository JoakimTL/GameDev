using Engine.Utilities.Data;
using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketRejection : Packet {

		public string Message { get; private set; }

		public PacketRejection( byte[] data ) : base( data ) {
			if( Segments.Count == 1 )
				Message = DataTransform.ToString( bytes, (int) Segments[ 0 ].Length, (int) Segments[ 0 ].StartIndex );
		}

		public PacketRejection( string message ) : base( GeneratePacketData( typeof( PacketRejection ), DataTransform.GetBytes( message ) ) ) {
			Message = message;
		}

	}
}
