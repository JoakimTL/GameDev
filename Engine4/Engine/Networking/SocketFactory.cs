using System.Net.Sockets;

namespace Engine.Networking;

public class SocketFactory : ModuleService {

	public bool IsIpv6Enabled { get; set; } = false;

	public Socket CreateTcp() {
		if ( this.IsIpv6Enabled ) {
			Socket s = new( AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp );
			s.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0 );
			return s;
		} else {
			Socket s = new( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			return s;
		}
	}

	public Socket CreateUdp() {
		if ( this.IsIpv6Enabled ) {
			Socket s = new( AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp );
			s.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0 );
			return s;
		} else {
			Socket s = new( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
			return s;
		}
	}

	protected override bool OnDispose() => true;
}