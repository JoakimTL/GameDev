using Engine.Networking.Packets;
using Engine.Utilities.IO;
using System;
using System.Threading;

namespace Engine.Networking {
	/// <summary>
	/// Sends a small packet on a regular basis.
	/// </summary>
	public class ConnectionPingReader : IPacketListener {

		private Connection connection;
		
		public ConnectionPingReader( Connection connection, PacketReader reader ) {
			this.connection = connection;
			reader.Add( this );
		}

		public void HandlePacket( Packet p ) {
			if (p.ID == PacketType.UDPFORCE.ID ) {
				connection.ReceivedUDPForcePacket();
			} else {
				connection.Send( p );
			}
		}

		public bool IsListening( uint iD ) {
			return iD == PacketType.PINGTCP.ID || iD == PacketType.PINGUDP.ID || iD == PacketType.UDPFORCE.ID;
		}
	}
}
