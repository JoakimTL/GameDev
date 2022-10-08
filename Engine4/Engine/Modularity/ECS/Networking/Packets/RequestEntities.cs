using Engine.Networking;

namespace Engine.Modularity.ECS.Networking.Packets;

[Protocol( System.Net.Sockets.ProtocolType.Tcp )]
public class RequestEntities : Packet {
	public RequestEntities( byte[] packetData ) : base( GeneratePacketData() ) { }
}
