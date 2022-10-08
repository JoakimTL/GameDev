using Engine.Networking;

namespace Engine.Modularity.ECS.Networking.Packets;

[Protocol( System.Net.Sockets.ProtocolType.Tcp )]
public class EntityAdded : Packet {

	public byte[]? SerializedData { get; }

	public EntityAdded( Entity e ) : base( GeneratePacketData( Entity.Serialize( e ) ?? Array.Empty<byte>() ) ) {
		this.SerializedData = null;
	}

	public EntityAdded( byte[] packetData ) : base( packetData ) {
		byte[][] data = GetSeparatedData();
		this.SerializedData = data[ 0 ];
	}
}
