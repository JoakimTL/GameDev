using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets;

public class UsernameChange : PacketBase, IConstructablePacket {

	public string NewUsername { get; }

	public override ProtocolType Protocol => ProtocolType.Tcp;

	public UsernameChange( string name ) {
		this.NewUsername = name;
	}

	protected override byte[]? GetRequestedData() => NewUsername.ToBytes();

	public static PacketBase? Construct( byte[] data, IPEndPoint? remote ) {
		var packet = new UsernameChange( data.CreateStringOrThrow() );
		packet.SetBytes( data );
		packet.RemoteSender = remote;
		return packet;
	}
}
