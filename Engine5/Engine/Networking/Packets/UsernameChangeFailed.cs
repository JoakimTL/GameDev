using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets;

public class UsernameChangeFailed : PacketBase, IConstructablePacket {

	public string Reason { get; }

	public override ProtocolType Protocol => ProtocolType.Tcp;

	public UsernameChangeFailed( string reason ) {
		this.Reason = reason;
	}

	protected override byte[]? GetRequestedData() => Reason.ToBytes();

	public static PacketBase? Construct( byte[] data, IPEndPoint? remote ) {
		var packet = new UsernameChangeFailed( data.CreateStringOrThrow() );
		packet.SetBytes( data );
		packet.RemoteSender = remote;
		return packet;
	}
}
