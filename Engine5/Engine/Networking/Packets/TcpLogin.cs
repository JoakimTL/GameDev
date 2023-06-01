using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets;

public class TcpLogin : PacketBase, IConstructablePacket {
	public string Username { get; }

	public override ProtocolType Protocol => ProtocolType.Tcp;

	public TcpLogin( string name ) {
		this.Username = name;
	}

	protected override byte[]? GetRequestedData() => Username.ToBytes();

	public static PacketBase? Construct( byte[] data, IPEndPoint? remote ) {
		var packet = new TcpLogin( data.CreateStringOrThrow() );
		packet.SetBytes( data );
		packet.RemoteSender = remote;
		return packet;
	}
}
