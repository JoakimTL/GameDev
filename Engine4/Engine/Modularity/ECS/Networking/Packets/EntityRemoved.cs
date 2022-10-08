using Engine.Data;
using Engine.Networking;

namespace Engine.Modularity.ECS.Networking.Packets;

[Protocol( System.Net.Sockets.ProtocolType.Tcp )]
public class EntityRemoved : Packet {

	public string? EntityName { get; }

	public EntityRemoved( string entityName ) : base( GeneratePacketData( DataUtils.ToBytes( entityName ) ) ) {
		this.EntityName = entityName;
	}

	public EntityRemoved( byte[] packetData ) : base( packetData ) {
		byte[][] data = GetSeparatedData();
		this.EntityName = DataUtils.ToStringUTF8( data[ 0 ] );
	}
}
