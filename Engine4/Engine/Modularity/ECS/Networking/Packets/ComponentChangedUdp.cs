using Engine.Networking;

namespace Engine.Modularity.ECS.Networking.Packets;

[Protocol( System.Net.Sockets.ProtocolType.Udp )]
public class ComponentChangedUdp : Packet {

	public byte[]? SerializedData { get; }

	public ComponentChangedUdp( SerializableComponent c ) : base( GeneratePacketData( c.Serialize() ?? Array.Empty<byte>() ) ) {
		this.SerializedData = null;
	}

	public ComponentChangedUdp( byte[] packetData ) : base( packetData ) {
		byte[][] data = GetSeparatedData();
		this.SerializedData = data[ 0 ];
	}
}
