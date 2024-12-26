using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Engine.Networking.Server {
	public class ServerAdmin {

		private int port;
		public ConnectionTunnel TunnelUDP { get; protected set; }
		public ConnectionReceiver ConnectionReceiver { get; protected set; }
		public ConnectionList ConnectionList { get; protected set; }
		public PacketReader PacketReader { get; protected set; }
		//public PlayerList PlayerList { get; protected set; }

		public ServerAdmin( int port ) {
			this.port = port;
			Socket udpSocket = new Socket( AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp );
			udpSocket.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0 );
			udpSocket.Bind( new IPEndPoint( IPAddress.IPv6Any, port ) );
			PacketReader = new PacketReader();
			TunnelUDP = new ConnectionTunnel( udpSocket );
			TunnelUDP.CreateReceiver( PacketReader );
			ConnectionReceiver = new ConnectionReceiver( port, SocketType.Stream, ProtocolType.Tcp );
			ConnectionList = new ConnectionList( PacketReader, TunnelUDP.Socket, ConnectionReceiver );
			PacketReader.Add( ConnectionList );
			//PlayerList = new PlayerList( PacketReader );
			//ConnectionList.NewConnectionEvent += PlayerList.PlayerConnected;
			//ConnectionList.LostConnectionEvent += PlayerList.PlayerDisconnected;
		}
	}
}
