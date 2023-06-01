using Engine.Datatypes;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking.Packets;

public class UdpPing : PacketBase, IConstructablePacket {
	public string Username { get; }
	public double Time { get; }

	public override ProtocolType Protocol => ProtocolType.Udp;

	public UdpPing( string username, double time ) {
		this.Username = username;
		this.Time = time;
	}

	protected override byte[]? GetRequestedData() => Segmentation.Segment(Username.ToBytes(), Time.ToBytes());

	public static PacketBase? Construct( byte[] data, IPEndPoint? remote ) {
		var parts = Segmentation.ParseOrThrow( data );
		var packet = new UdpPing( parts[ 0 ].CreateStringOrThrow(), parts[ 1 ].ToUnmanagedOrThrow<double>() );
		packet.SetBytes( data );
		packet.RemoteSender = remote;
		return packet;
	}
}
