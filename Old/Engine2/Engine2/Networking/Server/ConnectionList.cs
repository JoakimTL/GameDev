using Engine.MemLib;
using Engine.Networking.Packets;
using Engine.Utilities.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Engine.Networking.Server {
	public class ConnectionList : IPacketListener {

		private PacketReader reader;
		private Socket udpSocket;
		private ConnectionReceiver conReceiver;

		private ConcurrentQueue<PacketConnection> connectionPackets;
		private ConcurrentQueue<PacketDisconnection> disconnectionPackets;
		private ConcurrentQueue<PacketProtocolPortCorrection> protocolCorrectionPackets;

		private ConcurrentDictionary<IPEndPoint, Connection> connectionsByAdresses;
		private Dictionary<string, Connection> connectionsByName;
		internal HashSet<Connection> Connections { get; private set; }

		private ConnectionPingBroadcaster pingBroadcasters;

		private Thread sortingThread;
		private AutoResetEvent sortingEvent;

		public event NewPlayerHandler NewConnectionEvent;
		public event NewPlayerHandler LostConnectionEvent;

		public ConnectionList( PacketReader reader, Socket udpSocket, ConnectionReceiver conReceiver ) {
			this.reader = reader;
			this.udpSocket = udpSocket;
			this.conReceiver = conReceiver;
			conReceiver.NewConnectionEvent += NewConnection;

			connectionPackets = new ConcurrentQueue<PacketConnection>();
			disconnectionPackets = new ConcurrentQueue<PacketDisconnection>();
			protocolCorrectionPackets = new ConcurrentQueue<PacketProtocolPortCorrection>();

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
				s.Send( new PacketRejection( "Failed to connect, server was unable to add to connectionlist!" ).ByteArray );
			}
		}

		private void ConnectionLost( Connection c ) {
			SendAll( new PacketDisconnection( c.Name ) );
			LostConnectionEvent?.Invoke( c );
		}

		private void SortConnections() {
			while( MemLib.Mem.Threads.Running ) {
				sortingEvent.WaitOne();
				while( connectionPackets.TryDequeue( out PacketConnection p0 ) ) {
					if( connectionsByName.ContainsKey( p0.Username ) ) {
						if( connectionsByAdresses.TryRemove( p0.RemoteEndpoint, out Connection con ) )
							con.Close( new PacketRejection( "Username taken." ) );
						continue;
					}

					if( !connectionsByName.ContainsKey( p0.Username ) ) {
						if( connectionsByAdresses.TryGetValue( p0.RemoteEndpoint, out Connection con ) ) {
							connectionsByName.Add( p0.Username, con );
							Connections.Add( con );
							con.SetName( p0.Username );
							con.CreatePinger( 1000 );
							NewConnectionEvent?.Invoke( con );
							Send( p0.RemoteEndpoint, new PacketPlayerList( new List<string>( connectionsByName.Keys ).ToArray() ) );
							SendAllExcept( p0.RemoteEndpoint, p0 );
						}
					}
				}
				while( disconnectionPackets.TryDequeue( out PacketDisconnection p0 ) ) {
					if( connectionsByAdresses.TryRemove( p0.RemoteEndpoint, out Connection con ) )
						con.Close();
					connectionsByName.Remove( p0.Username );
					Connections.Remove( con );
					SendAll( p0 );
				}
				while( protocolCorrectionPackets.TryDequeue( out PacketProtocolPortCorrection p0 ) ) {
					if( connectionsByAdresses.TryGetValue( p0.RemoteEndpoint, out Connection con ) ) {
						if( p0.Protocol == ProtocolType.Udp ) {
							ConnectionTunnel t = new ConnectionTunnel( udpSocket, new IPEndPoint( p0.RemoteEndpoint.Address, p0.Port ) );
							con.Add( t );
							connectionsByAdresses.TryAdd( t.RemoteEndPoint, con );
						}
					}
				}
			}
		}

		public void HandlePacket( Packet p ) {
			if( p.ID == PacketType.CONNECT.ID ) {
				PacketConnection p0 = p as PacketConnection;
				connectionPackets.Enqueue( p0 );
				sortingEvent.Set();
			} else if( p.ID == PacketType.DISCONNECT.ID ) {
				PacketDisconnection p0 = p as PacketDisconnection;
				disconnectionPackets.Enqueue( p0 );
				sortingEvent.Set();
			} else if( p.ID == PacketType.PROTOPORTCOR.ID ) {
				PacketProtocolPortCorrection p0 = p as PacketProtocolPortCorrection;
				protocolCorrectionPackets.Enqueue( p0 );
				sortingEvent.Set();
			} else if( p.ID == PacketType.PINGTCP.ID ) {
				PacketPingTCP p0 = p as PacketPingTCP;
				if( connectionsByAdresses.TryGetValue( p0.RemoteEndpoint, out Connection con ) ) {
					con.PingTCP = (float) ( (double) ( ( DateTime.Now.Ticks - p0.Time ) * 1000 ) / Stopwatch.Frequency );
				}
			} else if( p.ID == PacketType.PINGUDP.ID ) {
				PacketPingUDP p0 = p as PacketPingUDP;
				if( connectionsByAdresses.TryGetValue( p0.RemoteEndpoint, out Connection con ) ) {
					con.PingUDP = (float) ( (double) ( ( DateTime.Now.Ticks - p0.Time ) * 1000 ) / Stopwatch.Frequency );
				}
			} else if( p.ID == PacketType.CHATMESSAGE.ID ) {
				SendAll( p );
			} else if( p.ID == PacketType.UDPFORCE.ID ) {
				Send( p.RemoteEndpoint, p );
			}
		}

		public bool IsListening( uint iD ) {
			return iD == PacketType.CONNECT.ID || iD == PacketType.DISCONNECT.ID || iD == PacketType.PROTOPORTCOR.ID ||
					iD == PacketType.PINGTCP.ID || iD == PacketType.PINGUDP.ID || iD == PacketType.CHATMESSAGE.ID || 
					iD == PacketType.UDPFORCE.ID;
		}

		public void Send( IPEndPoint address, Packet p ) {
			if( connectionsByAdresses.TryGetValue( address, out Connection c ) ) {
				c.Send( p );
				return;
			}
			MemLib.Mem.Logs.Error.WriteLine( "Unable to find any connection with the address " + address + "!" );
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
