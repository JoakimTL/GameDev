using System;
using System.Collections.Generic;

namespace Engine.Networking {
	public interface IPacketListener {
		IReadOnlyCollection<Type> ListeningPacketTypes { get; }
		void HandlePacket( PacketMessage p );
	}
}