using Engine.Networking;

namespace Engine.Modularity.ECS.Networking.Packets;

[Protocol( System.Net.Sockets.ProtocolType.Tcp )]
public class ComponentChangedTcp : Packet {

	public byte[]? SerializedData { get; }

	public ComponentChangedTcp( SerializableComponent c ) : base( GeneratePacketData( c.Serialize() ?? Array.Empty<byte>() ) ) {
		this.SerializedData = null;
	}

	public ComponentChangedTcp( byte[] packetData ) : base( packetData ) {
		byte[][] data = GetSeparatedData();
		this.SerializedData = data[ 0 ];
	}
}
