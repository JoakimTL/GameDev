using Engine.Datatypes;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets;

public class UdpPing : PacketBase, IConstructablePacket {

	public NetworkId NetworkId { get; }
	public double Time { get; }

	public override ProtocolType Protocol => ProtocolType.Udp;

	public UdpPing( NetworkId networkId, double time ) {
		this.NetworkId = networkId;
		this.Time = time;
	}

	protected override byte[]? GetRequestedData() => Segmentation.Segment(NetworkId.ToBytes(), Time.ToBytes());

	public static PacketBase? Construct( byte[] data, IPEndPoint? remote ) {
		var parts = Segmentation.ParseOrThrow( data );
		var packet = new UdpPing( parts[ 0 ].ToUnmanagedOrThrow<NetworkId>(), parts[ 1 ].ToUnmanagedOrThrow<double>() );
		packet.SetBytes( data );
		packet.RemoteSender = remote;
		return packet;
	}
}
