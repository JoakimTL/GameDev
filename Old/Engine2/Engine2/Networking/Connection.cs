using Engine.MemLib;
using Engine.Networking.Packets;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Engine.Networking {
	public class Connection {

		public string Name { get; private set; }

		public bool IsServerSide { get; private set; }

		private Dictionary<ProtocolType, ConnectionTunnel> tunnels;
		private PacketReader reader;
		public PacketReader PacketHandler { get; private set; }

		public ConnectionPinger Pinger { get; private set; }

		public ConnectionPingReader PingReader { get; private set; }

		private Thread connectingThread;

		public event ConnectionClosedHandler ConnectionClosedEvent;

		public float PingTCP, PingUDP;

		public Connection( PacketReader reader ) {
			this.reader = reader;
			tunnels = new Dictionary<ProtocolType, ConnectionTunnel>();
			IsServerSide = false;
			PingTCP = -1;
			PingUDP = -1;
		}

		public void ConnectAsClient( string name, IPEndPoint address ) {
			if( tunnels.Count > 0 || connectingThread != null )
				return;
			if( !SetName( name ) )
				return;

			connectingThread = MemLib.Mem.Threads.StartNew( delegate () {
				ConnectionTunnel tcpTunnel = new ConnectionTunnel( SocketType.Stream, ProtocolType.Tcp );
				if( tcpTunnel.ConnectTCP( address, name ) ) {
					tunnels.Add( tcpTunnel.Socket.ProtocolType, tcpTunnel );
					tcpTunnel.ConnectionFailureEvent += ConnectionFailure;
					tcpTunnel.CreateReceiver( reader );

					ConnectionTunnel udpTunnel = new ConnectionTunnel( SocketType.Dgram, ProtocolType.Udp );
					udpTunnel.SetRemote( address );
					tunnels.Add( udpTunnel.Socket.ProtocolType, udpTunnel );
					udpTunnel.ConnectionFailureEvent += ConnectionFailure;
					udpTunnel.CreateReceiver( reader );
					CreateUDPForcer( reader );

					tcpTunnel.Send( new PacketProtocolPortCorrection( ProtocolType.Udp, ( udpTunnel.Socket.LocalEndPoint as IPEndPoint ).Port ) );
					CreatePingReader();
					IsServerSide = false;
				}
				connectingThread = null;
			}, "Connecting Thread" );
		}

		private void CreateUDPForcer( PacketReader reader ) {
			if( !( Pinger is null ) )
				return;
			Pinger = new ConnectionPinger( this, 100, false );
		}

		internal void ReceivedUDPForcePacket() {
			Pinger.Stop();
		}

		public void SetTCPSocketAsServer( Socket tcpSocket ) {
			ConnectionTunnel tcpTunnel = new ConnectionTunnel( tcpSocket );
			tunnels.Add( tcpTunnel.Socket.ProtocolType, tcpTunnel );
			tcpTunnel.ConnectionFailureEvent += ConnectionFailure;
			tcpTunnel.CreateReceiver( reader );
			IsServerSide = true;
		}

		public bool SetName( string name ) {
			if( !( Name is null ) )
				return false;
			Name = name;
			return true;
		}

		public void Add( ConnectionTunnel tunnel ) {
			if( tunnels.Count == 0 )
				return;
			if( tunnels.ContainsKey( tunnel.Socket.ProtocolType ) )
				return;
			tunnels.Add( tunnel.Socket.ProtocolType, tunnel );
			tunnel.ConnectionFailureEvent += ConnectionFailure;
		}

		public PacketReceiver CreatePacketReceiver( ProtocolType protocol ) {
			if( tunnels.TryGetValue( protocol, out ConnectionTunnel tunnel ) )
				return tunnel.CreateReceiver( reader );
			return null;
		}

		public ConnectionTunnel this[ ProtocolType protocol ] {
			get {
				if( tunnels.TryGetValue( protocol, out ConnectionTunnel tunnel ) )
					return tunnel;
				return null;
			}
		}

		public void CreatePinger( int ms ) {
			if( Pinger != null )
				Pinger.Stop();
			Pinger = new ConnectionPinger( this, ms, true );
		}

		public void CreatePingReader() {
			if( PingReader != null )
				reader.Remove( PingReader );
			PingReader = new ConnectionPingReader( this, reader );
		}

		public void Send( Packet p ) {
			if( tunnels.TryGetValue( p.Type.Protocol, out ConnectionTunnel tunnel ) ) {
				tunnel.Send( p );
			}
		}

		public void SendIfNot( IPEndPoint address, Packet p ) {
			if( tunnels.TryGetValue( p.Type.Protocol, out ConnectionTunnel tunnel ) ) {
				if( tunnel.RemoteEndPoint != address )
					tunnel.Send( p );
			}
		}

		private void ConnectionFailure( Socket s, SocketException e ) {
			Close();
		}

		public void Close() {
			if( tunnels.Count == 0 )
				return;
			foreach( var kv in tunnels ) {
				kv.Value.Stop();
			}
			tunnels.Clear();
			ConnectionClosedEvent?.Invoke( this );
			Name = null;
		}

		public void Close( Packet p ) {
			if( tunnels.Count == 0 )
				return;
			Send( p );
			foreach( var kv in tunnels ) {
				kv.Value.Stop();
			}
			tunnels.Clear();
			ConnectionClosedEvent?.Invoke( this );
			Name = null;
		}
	}
}
