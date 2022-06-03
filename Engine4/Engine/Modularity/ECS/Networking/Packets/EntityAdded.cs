using Engine.Data;
using Engine.Networking;

namespace Engine.Modularity.ECS.Networking.Packets;

[Protocol( System.Net.Sockets.ProtocolType.Tcp )]
public class RequestEntities : Packet {
	public RequestEntities( byte[] packetData ) : base( GeneratePacketData() ) { }
}

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


[Protocol( System.Net.Sockets.ProtocolType.Tcp )]
public class ComponentAdded : Packet {

	public byte[]? SerializedData { get; }

	public ComponentAdded( Component c ) : base( GeneratePacketData( c.Serialize() ?? Array.Empty<byte>() ) ) {
		this.SerializedData = null;
	}

	public ComponentAdded( byte[] packetData ) : base( packetData ) {
		byte[][] data = GetSeparatedData();
		this.SerializedData = data[ 0 ];
	}
}


[Protocol( System.Net.Sockets.ProtocolType.Tcp )]
public class ComponentChangedTcp : Packet {

	public byte[]? SerializedData { get; }

	public ComponentChangedTcp( Component c ) : base( GeneratePacketData( c.Serialize() ?? Array.Empty<byte>() ) ) {
		this.SerializedData = null;
	}

	public ComponentChangedTcp( byte[] packetData ) : base( packetData ) {
		byte[][] data = GetSeparatedData();
		this.SerializedData = data[ 0 ];
	}
}

[Protocol( System.Net.Sockets.ProtocolType.Udp )]
public class ComponentChangedUdp : Packet {

	public byte[]? SerializedData { get; }

	public ComponentChangedUdp( Component c ) : base( GeneratePacketData( c.Serialize() ?? Array.Empty<byte>() ) ) {
		this.SerializedData = null;
	}

	public ComponentChangedUdp( byte[] packetData ) : base( packetData ) {
		byte[][] data = GetSeparatedData();
		this.SerializedData = data[ 0 ];
	}
}

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