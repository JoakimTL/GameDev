using Engine.Utilities.Data;
using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketPlayerList : Packet {

		public IReadOnlyList<string> Usernames { get; private set; }

		public PacketPlayerList( byte[] data ) : base( data ) {
			string[] usernames = new string[ Segments.Count ];
			for( int i = 0; i < usernames.Length; i++ )
				usernames[ i ] = DataTransform.ToString( bytes, (int) Segments[ i ].Length, (int) Segments[ i ].StartIndex );
			Usernames = usernames;
		}

		public PacketPlayerList( string[] usernames ) : base( GeneratePacketData( typeof( PacketPlayerList ), DataTransform.GetByteArray( usernames ) ) ) {
			Usernames = usernames;
		}

	}
}
