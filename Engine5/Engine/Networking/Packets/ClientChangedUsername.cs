using Engine.Datatypes;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets;

public class ClientChangedUsername : PacketBase, IConstructablePacket {

	public NetworkId NetworkId { get; }
	public string NewUsername { get; }

	public override ProtocolType Protocol => ProtocolType.Tcp;

	public ClientChangedUsername( NetworkId networkId, string newUsername ) {
		this.NetworkId = networkId;
		this.NewUsername = newUsername;
	}

	protected override byte[]? GetRequestedData() => Segmentation.Segment( NetworkId.ToBytes(), NewUsername.ToBytes() );

	public static PacketBase? Construct( byte[] data, IPEndPoint? remote ) {
		var parts = Segmentation.ParseOrThrow( data );
		var packet = new ClientChangedUsername( parts[ 0 ].ToUnmanagedOrThrow<NetworkId>(), parts[1 ].CreateStringOrThrow() );
		packet.SetBytes( data );
		packet.RemoteSender = remote;
		return packet;
	}
}
