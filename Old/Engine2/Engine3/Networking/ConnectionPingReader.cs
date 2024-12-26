using Engine.Networking.Packets;
using Engine.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Engine.Networking {
	/// <summary>
	/// Sends a small packet on a regular basis.
	/// </summary>
	public class ConnectionPingReader : IPacketListener {

		private Connection connection;

		private static readonly Type[] packetListeningList = {
			typeof(PacketPingTCP),
			typeof(PacketPingUDP),
			typeof(PacketUDPForce)
		};
		public IReadOnlyCollection<Type> ListeningPacketTypes => packetListeningList;

		public ConnectionPingReader( Connection connection, PacketReader reader ) {
			this.connection = connection;
			reader.Add( this );
		}

		public void HandlePacket( PacketMessage m ) {
			if( m.Packet.DataType == typeof( PacketUDPForce ) ) {
				connection.ReceivedUDPForcePacket();
			} else {
				connection.Send( m.Packet );
			}
		}
	}
}
