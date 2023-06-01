using Engine.Datatypes;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets;

public class TcpLoginAck : PacketBase, IConstructablePacket {
	public string Username { get; }
	public ulong NetworkId { get; }

	public override ProtocolType Protocol => ProtocolType.Tcp;

	public TcpLoginAck( string name, ulong id ) {
		this.Username = name;
		this.NetworkId = id;
	}

	protected override byte[]? GetRequestedData() => Segmentation.Segment( Username.ToBytes(), NetworkId.ToBytes() );

	public static PacketBase? Construct( byte[] data, IPEndPoint? remote ) {
		var parts = Segmentation.ParseOrThrow( data );
		var packet = new TcpLoginAck( parts[ 0 ].CreateStringOrThrow(), parts[ 1 ].ToUnmanagedOrThrow<ulong>() );
		packet.SetBytes( data );
		packet.RemoteSender = remote;
		return packet;
	}
}
