using Engine.MemLib;
using Engine.Networking.Packets;
using Engine.Utilities.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Engine.Networking {
	public class PacketReader {

		private ConcurrentQueue<PacketMessage> packets;

		private Thread readingThread;
		private AutoResetEvent packetReceiveLock;

		private volatile Dictionary<Type, List<IPacketListener>> listeners;

		public PacketReader() {
			packetReceiveLock = new AutoResetEvent( false );
			packets = new ConcurrentQueue<PacketMessage>();
			listeners = new Dictionary<Type, List<IPacketListener>>();
			readingThread = Mem.Threads.StartNew( ReadPackets, $"Packet Reader" );
		}

		private void ReadPackets() {
			while( Mem.Threads.Running ) {
				packetReceiveLock.WaitOne();
				while( packets.TryDequeue( out PacketMessage p ) ) {
					lock( listeners ) {
						if( listeners.TryGetValue( p.Packet.DataType, out List<IPacketListener> listenerList ) ) {
							for( int i = 0; i < listenerList.Count; i++ ) {
								listenerList[ i ].HandlePacket( p );
							}
						}
					}
				}
			}
		}

		/*
		private bool Silenced( Type type ) {
			return type == PacketType.PINGTCP.ID || type == PacketType.PINGUDP.ID || type == PacketType.PINGTIMES.ID;
		}
		*/

		internal void NewPacket( PacketMessage p ) {
			packets.Enqueue( p );
			packetReceiveLock.Set();
		}

		public void Add( IPacketListener listener ) {
			lock( listeners ) {
				foreach( Type type in listener.ListeningPacketTypes ) {
					if( !listeners.TryGetValue( type, out List<IPacketListener> listenerList ) )
						listeners.Add( type, listenerList = new List<IPacketListener>() );
					listenerList.Add( listener );
				}
			}
		}

		public void Remove( IPacketListener listener ) {
			lock( listeners ) {
				foreach( Type type in listener.ListeningPacketTypes ) {
					if( listeners.TryGetValue( type, out List<IPacketListener> listenerList ) )
						listenerList.Remove( listener );
				}
			}
		}
	}
}
