using Engine.MemLib;
using Engine.Utilities.Data;
using Engine.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Engine.Networking {
	/// <summary>
	/// Receives incoming packets on any protocol. USE RECEIVEFROM AND SORT PROPERLY.
	/// </summary>
	public class PacketReceiver {
		//Packet listeners, receiving events when packets with certain IDs pop up.

		public bool Running;

		private Socket socket;
		private Thread recThread;
		private PacketReader reader;

		private Dictionary<IPEndPoint, DataContainer> data;

		public event ConnectionFailureHandler ConnectionFailureEvent;
		public event Action ReceiverClosedEvent;

		public PacketReceiver( Socket socket, PacketReader reader ) {
			this.socket = socket;
			this.reader = reader;
			data = new Dictionary<IPEndPoint, DataContainer>();
			recThread = MemLib.Mem.Threads.StartNew( this.Receive, "Packet Receiver " + socket.ProtocolType );
		}

		private void Receive() {
			try {
				byte[] inData = new byte[ 1024 ];
				List<byte> activeData = new List<byte>();
				Logging.Routine( "Listening for " + socket.ProtocolType + " messages!", ConsoleColor.Cyan );
				Running = true;
				while( Mem.Threads.Running && Running ) {

					//Receives the first 4 bytes to read the total length of this packet.
					EndPoint remote = new IPEndPoint( IPAddress.Any, 0 );
					int read = socket.ReceiveFrom( inData, 0, inData.Length, SocketFlags.None, ref remote );
					//sort udp packets?
					activeData.Clear();

					if( read == inData.Length ) {
						activeData.AddRange( inData );
					} else
						for( int i = 0; i < read; i++ )
							activeData.Add( inData[ i ] );//???

					IPEndPoint address = remote as IPEndPoint;
					if( address.Address == IPAddress.Any && address.Port == 0 ) {
						address = socket.RemoteEndPoint as IPEndPoint;
					}

					if( !data.TryGetValue( address, out DataContainer readData ) )
						data.Add( address, readData = new DataContainer() );

					if( readData.IntendedLength == 0 )
						readData.Reset( (int) DataTransform.ToUInt32( inData, 0 ) );

					while( readData.Data.Count < readData.IntendedLength && activeData.Count > 0 ) {
						read -= readData.Add( activeData, Math.Min( read, readData.IntendedLength - readData.Data.Count ) );

						if( readData.Data.Count == readData.IntendedLength && readData.Data.Count > 0 ) {
							//Packet is done!
							Packet p = PacketManager.CreatePacket( readData.Data.ToArray() );
							readData.Reset( 0 );

							if( p != null ) {
								//Store the packet in the container for future access.
								reader.NewPacket( new PacketMessage( p, address ) );
							}
							if( activeData.Count > 0 )
								readData.Reset( (int) DataTransform.ToUInt32( activeData.ToArray(), 0 ) );
						}
					}
				}
			} catch( SocketException e ) {
				Logging.Warning( "Rec:" + socket.ProtocolType + ": " + e.ToString() );
				ConnectionFailureEvent?.Invoke( socket, e );
			} catch( Exception e ) {
				Logging.Error( e.ToString() );
			}
			ReceiverClosedEvent?.Invoke();
			Logging.Routine( "Stopped listening for " + socket.ProtocolType + " messages!", ConsoleColor.Cyan );
		}

		class DataContainer {

			public List<byte> Data { get; private set; }
			public int IntendedLength { get; private set; }

			public DataContainer() {
				IntendedLength = 0;
				Data = new List<byte>();
			}

			public void Reset( int intendedLength ) {
				Data.Clear();
				IntendedLength = intendedLength;
			}

			public int Add( List<byte> data, int l ) {
				if( this.Data.Count + l > IntendedLength ) {
					MemLib.Mem.Logs.Error.WriteLine( "Packet length is longer than anticipated. Discarding packet!" );
					Reset( 0 );
					return 0;
				}
				if( l == data.Count ) {
					this.Data.AddRange( data );
					data.Clear();
				} else {
					for( int i = 0; i < l; i++ ) {
						this.Data.Add( data[ i ] );
					}
					data.RemoveRange( 0, l );
				}
				return l;
			}

		}
	}
}
