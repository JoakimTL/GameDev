using Engine.Networking;

namespace Engine.Modularity.ECS.Networking.Packets;

[Protocol( System.Net.Sockets.ProtocolType.Tcp )]
public class AllEntities : Packet {

	public byte[][] SerializedData { get; }

	public AllEntities( byte[][] serializedEntities ) : base( GeneratePacketData( serializedEntities ) ) {
		this.SerializedData = serializedEntities;
	}

	public AllEntities( byte[] packetData ) : base( packetData ) {
		byte[][] data = GetSeparatedData();
		this.SerializedData = data;
	}
}
