using System.Collections.Generic;

namespace Engine.Networking {
	public interface IPacketListener {
		bool IsListening( uint iD );
		void HandlePacket( Packet p );
	}
}