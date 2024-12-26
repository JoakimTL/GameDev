using Engine.Utilities.Data;
using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketUserMessage : Packet {

		public string Username { get; private set; }
		public string Message { get; private set; }

		public PacketUserMessage( byte[] data ) : base( data ) {
			if( Segments.Count == 2 ) {
				Username = DataTransform.ToString( bytes, (int) Segments[ 0 ].Length, (int) Segments[ 0 ].StartIndex );
				Message = DataTransform.ToString( bytes, (int) Segments[ 1 ].Length, (int) Segments[ 1 ].StartIndex );
			}
		}

		public PacketUserMessage( string username, string message ) : base( GeneratePacketData( typeof( PacketUserMessage ), DataTransform.GetBytes( username ), DataTransform.GetBytes( message ) ) ) {
			Username = username;
			Message = message;
		}
	}
}
