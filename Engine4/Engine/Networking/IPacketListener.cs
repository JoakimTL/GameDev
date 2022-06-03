using System.Net;

namespace Engine.Networking;

public interface IPacketListener {
	/// <summary>
	/// Returns true if this listener is listening for this packet type.
	/// </summary>
	bool Listening( Type packetType );
	bool ListeningTcp( IPEndPoint endpoint );
	bool ListeningUdp( IPEndPoint endpoint );
	void NewPacket( Packet packet, Type packetType );
}