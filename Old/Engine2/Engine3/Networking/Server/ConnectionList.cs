using Engine.MemLib;
using Engine.Networking.Packets;
using Engine.Utilities.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Engine.Networking.Server {
	public class ConnectionList : IPacketListener {

		private PacketReader reader;
		private Socket udpSocket;
		private ConnectionReceiver conReceiver;

		private ConcurrentQueue<(IPEndPoint, PacketConnection)> connectionPackets;
		private ConcurrentQueue<(IPEndPoint, PacketDisconnection)> disconnectionPackets;
		private ConcurrentQueue<(IPEndPoint, PacketProtocolPortCorrection)> protocolCorrectionPackets;

		private ConcurrentDictionary<IPEndPoint, Connection> connectionsByAdresses;
		private Dictionary<string, Connection> connectionsByName;
		internal HashSet<Connection> Connections { get; private set; }


		private ConnectionPingBroadcaster pingBroadcasters;

		private Thread sortingThread;
		private AutoResetEvent sortingEvent;

		public event NewPlayerHandler NewConnectionEvent;
		public event NewPlayerHandler LostConnectionEvent;

		private static readonly Type[] packetListeningList = {
			typeof(PacketConnection),
			typeof(PacketDisconnection),
			typeof(PacketProtocolPortCorrection),
			typeof(PacketPingTCP),
			typeof(PacketPingUDP),
			typeof(PacketMessage),
			typeof(PacketUDPForce)
		};
		public IReadOnlyCollection<Type> ListeningPacketTypes => packetListeningList;

		public ConnectionList( PacketReader reader, Socket udpSocket, ConnectionReceiver conReceiver ) {
			this.reader = reader;
			this.udpSocket = udpSocket;
			this.conReceiver = conReceiver;
			conReceiver.NewConnectionEvent += NewConnection;

			connectionPackets = new ConcurrentQueue<(IPEndPoint, PacketConnection)>();
			disconnectionPackets = new ConcurrentQueue<(IPEndPoint, PacketDisconnection)>();
			protocolCorrectionPackets = new ConcurrentQueue<(IPEndPoint, PacketProtocolPortCorrection)>();

			connectionsByAdresses = new ConcurrentDictionary<IPEndPoint, Connection>();
			connectionsByName = new Dictionary<string, Connection>();
			Connections = new HashSet<Connection>();

			sortingEvent = new AutoResetEvent( false );
			sortingThread = MemLib.Mem.Threads.StartNew( this.SortConnections, "Connection Sorter" );

			pingBroadcasters = new ConnectionPingBroadcaster( this, 1000 );

		}

		private void NewConnection( Socket s ) {
			Connection con;
			if( connectionsByAdresses.TryAdd( s.RemoteEndPoint as IPEndPoint, con = new Connection( reader ) ) ) {
				con.SetTCPSocketAsServer( s );
				con.ConnectionClosedEvent += ConnectionLost;
				MemLib.Mem.Logs.Routine.WriteLine( $"New TCP Connection from [{s.RemoteEndPoint}]!" );
			} else {
				s.Send( new PacketRejection( "Failed to connect, server was unable to add to connectionlist!" ).DataBytes.ToArray() );
			}
		}

		private void ConnectionLost( Connection c ) {
			SendAll( new PacketDisconnection( c.Name ) );
			LostConnectionEvent?.Invoke( c );
		}

		private void SortConnections() {
			while( MemLib.Mem.Threads.Running ) {
				sortingEvent.WaitOne();
				while( connectionPackets.TryDequeue( out (IPEndPoint, PacketConnection) m ) ) {
					if( connectionsByName.ContainsKey( m.Item2.Username ) ) {
						if( connectionsByAdresses.TryRemove( m.Item1, out Connection con ) )
							con.Close( new PacketRejection( "Username taken." ) );
						continue;
					}

					if( !connectionsByName.ContainsKey( m.Item2.Username ) ) {
						if( connectionsByAdresses.TryGetValue( m.Item1, out Connection con ) ) {
							connectionsByName.Add( m.Item2.Username, con );
							Connections.Add( con );
							con.SetName( m.Item2.Username );
							con.CreatePinger( 1000 );
							NewConnectionEvent?.Invoke( con );
							Send( m.Item1, new PacketPlayerList( new List<string>( connectionsByName.Keys ).ToArray() ) );
							SendAllExcept( m.Item1, m.Item2 );
						}
					}
				}
				while( disconnectionPackets.TryDequeue( out (IPEndPoint, PacketDisconnection) m ) ) {
					if( connectionsByAdresses.TryRemove( m.Item1, out Connection con ) ) {
						con.Close();
					}
					connectionsByName.Remove( m.Item2.Username );
					Connections.Remove( con );
					SendAll( m.Item2 );
				}
				while( protocolCorrectionPackets.TryDequeue( out (IPEndPoint, PacketProtocolPortCorrection) m ) ) {
					if( connectionsByAdresses.TryGetValue( m.Item1, out Connection con ) ) {
						if( m.Item2.OverrideProtocol == ProtocolType.Udp ) {
							ConnectionTunnel t = new ConnectionTunnel( udpSocket, new IPEndPoint( m.Item1.Address, m.Item2.OverridePort ), false );
							con.Add( t );
							connectionsByAdresses.TryAdd( t.RemoteEndPoint, con );
						}
					}
				}
			}
		}

		public void HandlePacket( PacketMessage m ) {
			if( m.Packet.DataType == typeof( PacketConnection ) ) {
				PacketConnection p0 = m.Packet as PacketConnection;
				connectionPackets.Enqueue( (m.RemoteEndpoint, p0) );
				sortingEvent.Set();
			} else if( m.Packet.DataType == typeof( PacketDisconnection ) ) {
				PacketDisconnection p0 = m.Packet as PacketDisconnection;
				disconnectionPackets.Enqueue( (m.RemoteEndpoint, p0) );
				sortingEvent.Set();
			} else if( m.Packet.DataType == typeof( PacketProtocolPortCorrection ) ) {
				PacketProtocolPortCorrection p0 = m.Packet as PacketProtocolPortCorrection;
				protocolCorrectionPackets.Enqueue( (m.RemoteEndpoint, p0) );
				sortingEvent.Set();
			} else if( m.Packet.DataType == typeof( PacketPingTCP ) ) {
				PacketPingTCP p0 = m.Packet as PacketPingTCP;
				if( connectionsByAdresses.TryGetValue( m.RemoteEndpoint, out Connection con ) ) {
					con.PingTCP = (float) ( (double) ( ( DateTime.Now.Ticks - p0.Time ) * 1000 ) / Stopwatch.Frequency );
				}
			} else if( m.Packet.DataType == typeof( PacketPingUDP ) ) {
				PacketPingUDP p0 = m.Packet as PacketPingUDP;
				if( connectionsByAdresses.TryGetValue( m.RemoteEndpoint, out Connection con ) ) {
					con.PingUDP = (float) ( (double) ( ( DateTime.Now.Ticks - p0.Time ) * 1000 ) / Stopwatch.Frequency );
				}
			} else if( m.Packet.DataType == typeof( PacketUserMessage ) ) {
				PacketUserMessage p0 = m.Packet as PacketUserMessage;
				Logging.Routine( $"{p0.Username} says: {p0.Message}" );
				SendAll( m.Packet );
			} else if( m.Packet.DataType == typeof( PacketUDPForce ) ) {
				Send( m.RemoteEndpoint, m.Packet );
			}
		}

		public void Send( IPEndPoint address, Packet p ) {
			if( connectionsByAdresses.TryGetValue( address, out Connection c ) ) {
				c.Send( p );
				return;
			}
			Logging.Error( "Unable to find any connection with the address " + address + "!" );
		}

		public void SendAll( Packet p ) {
			foreach( var v in Connections )
				v.Send( p );
		}

		public void SendAllExcept( IPEndPoint address, Packet p ) {
			foreach( var v in Connections )
				v.SendIfNot( address, p );
		}

	}
}
