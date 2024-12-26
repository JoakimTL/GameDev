using Engine.MemLib;
using Engine.Utilities.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Engine.Networking {
	public class PacketReader {

		private ConcurrentQueue<Packet> packets;

		private Thread readingThread;
		private AutoResetEvent packetReceiveLock;

		private volatile Dictionary<uint, List<IPacketListener>> listeners;

		public PacketReader() {
			packetReceiveLock = new AutoResetEvent( false );
			packets = new ConcurrentQueue<Packet>();
			listeners = new Dictionary<uint, List<IPacketListener>>();
			readingThread = MemLib.Mem.Threads.StartNew( this.ReadPackets, $"Packet Reader" );
		}

		private void ReadPackets() {
			while( MemLib.Mem.Threads.Running ) {
				packetReceiveLock.WaitOne();
				while( packets.TryDequeue( out Packet p ) ) {
					//if( !Silenced( p.ID ) )
					//	Cache.Logs.Routine.WriteLine( "Received new Packet: " + p );
					lock( listeners ) {
						if( listeners.TryGetValue( p.ID, out List<IPacketListener> listenerList ) ) {
							for( int i = 0; i < listenerList.Count; i++ ) {
								listenerList[ i ].HandlePacket( p );
							}
						}
					}
				}
			}
		}

		private bool Silenced( uint iD ) {
			return iD == PacketType.PINGTCP.ID || iD == PacketType.PINGUDP.ID || iD == PacketType.PINGTIMES.ID;
		}

		public void NewPacket( Packet p ) {
			packets.Enqueue( p );
			packetReceiveLock.Set();
		}

		public void Add( IPacketListener listener ) {
			lock( listeners ) {
				foreach( uint id in PacketType.ListIDs ) {
					if( listener.IsListening( id ) ) {
						if( !listeners.TryGetValue( id, out List<IPacketListener> listenerList ) )
							listeners.Add( id, listenerList = new List<IPacketListener>() );
						listenerList.Add( listener );
					}
				}
			}
		}

		public void Remove( IPacketListener listener ) {
			lock( listeners ) {
				foreach( uint id in PacketType.ListIDs ) {
					if( listener.IsListening( id ) ) {
						if( listeners.TryGetValue( id, out List<IPacketListener> listenerList ) )
							listenerList.Remove( listener );
					}
				}
			}
		}
	}
}
