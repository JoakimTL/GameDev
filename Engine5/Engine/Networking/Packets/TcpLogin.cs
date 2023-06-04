using Engine.Datatypes;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets;

public class TcpLogin : PacketBase, IConstructablePacket {

	public string Username { get; }

	public ushort UdpPort { get; }

	public override ProtocolType Protocol => ProtocolType.Tcp;

	public TcpLogin( string name, ushort udpPort ) {
		this.Username = name;
		this.UdpPort = udpPort;
	}

	protected override byte[]? GetRequestedData() => Segmentation.Segment( Username.ToBytes(), UdpPort.ToBytes() );

	public static PacketBase? Construct( byte[] data, IPEndPoint? remote ) {
		var segments = Segmentation.ParseOrThrow( data );
		var packet = new TcpLogin( segments[ 0 ].CreateStringOrThrow(), segments[ 1 ].ToUnmanagedOrThrow<ushort>() );
		packet.SetBytes( data );
		packet.RemoteSender = remote;
		return packet;
	}
}
