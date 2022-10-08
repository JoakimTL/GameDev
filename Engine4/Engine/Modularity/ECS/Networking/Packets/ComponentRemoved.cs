using Engine.Networking;

namespace Engine.Modularity.ECS.Networking.Packets;

[Protocol( System.Net.Sockets.ProtocolType.Tcp )]
public class ComponentRemoved : Packet {

	public Guid EntityGuid { get; }
	public Type? ComponentType { get; }

	public ComponentRemoved( Component c ) : base( GeneratePacketData( c.Parent.Guid.ToByteArray(), IdentificationRegistry.Get( c.GetType() )?.ToByteArray() ?? Array.Empty<byte>() ) ) {
		this.EntityGuid = c.Parent.Guid;
		this.ComponentType = c.GetType();
	}

	public ComponentRemoved( byte[] packetData ) : base( packetData ) {
		byte[][] data = GetSeparatedData();
		this.EntityGuid = new Guid( data[ 0 ] );
		this.ComponentType = IdentificationRegistry.Get( new Guid( data[ 1 ] ) );
	}
}