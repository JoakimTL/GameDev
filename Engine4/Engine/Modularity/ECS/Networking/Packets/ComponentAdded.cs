using Engine.Networking;

namespace Engine.Modularity.ECS.Networking.Packets;

[Protocol( System.Net.Sockets.ProtocolType.Tcp )]
public class ComponentAdded : Packet {

	public byte[]? SerializedData { get; }

	public ComponentAdded( SerializableComponent c ) : base( GeneratePacketData( c.Serialize() ?? Array.Empty<byte>() ) ) {
		this.SerializedData = null;
	}

	public ComponentAdded( byte[] packetData ) : base( packetData ) {
		byte[][] data = GetSeparatedData();
		this.SerializedData = data[ 0 ];
	}
}
