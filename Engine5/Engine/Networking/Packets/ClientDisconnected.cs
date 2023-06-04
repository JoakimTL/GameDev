using Engine.Datatypes;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets;

public class ClientDisconnected : PacketBase, IConstructablePacket {

	public NetworkId NetworkId { get; }
	public string Reason { get; }

	public override ProtocolType Protocol => ProtocolType.Tcp;

	public ClientDisconnected( NetworkId networkId, string reason ) {
		this.NetworkId = networkId;
		this.Reason = reason;
	}

	protected override byte[]? GetRequestedData() => Segmentation.Segment( NetworkId.ToBytes(), Reason.ToBytes() );

	public static PacketBase? Construct( byte[] data, IPEndPoint? remote ) {
		var parts = Segmentation.ParseOrThrow( data );
		var packet = new ClientDisconnected( parts[ 0 ].ToUnmanagedOrThrow<NetworkId>(), parts[ 1 ].CreateStringOrThrow() );
		packet.SetBytes( data );
		packet.RemoteSender = remote;
		return packet;
	}
}
