using Engine.Datatypes;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets;

public class TcpLoginAck : PacketBase, IConstructablePacket {

	public NetworkId NetworkId { get; }
	public string Username { get; }

	public override ProtocolType Protocol => ProtocolType.Tcp;

	public TcpLoginAck( NetworkId id, string name ) {
		this.NetworkId = id;
		this.Username = name;
	}

	protected override byte[]? GetRequestedData() => Segmentation.Segment( NetworkId.ToBytes(), Username.ToBytes() );

	public static PacketBase? Construct( byte[] data, IPEndPoint? remote ) {
		var parts = Segmentation.ParseOrThrow( data );
		var packet = new TcpLoginAck( parts[ 0 ].ToUnmanagedOrThrow<NetworkId>(), parts[ 1 ].CreateStringOrThrow() );
		packet.SetBytes( data );
		packet.RemoteSender = remote;
		return packet;
	}
}
