using Engine.MemLib;
using Engine.Networking.Packets;
using Engine.Utilities.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Engine.Networking {
	//Used to send packets
	public class ConnectionTunnel {

		private string name;

		public Socket Socket { get; private set; }
		public PacketReceiver Receiver { get; private set; }

		public bool IsConnecting { get; private set; }
		public event ConnectionFailureHandler ConnectionFailureEvent;

		private IPEndPoint remoteEndPoint;
		public IPEndPoint RemoteEndPoint => GetRemote();

		public int Timeout { get; private set; }

		public ConnectionTunnel( SocketType socketType, ProtocolType protocol ) {
			Socket = new Socket( AddressFamily.InterNetworkV6, socketType, protocol );
			Socket.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0 );
			Socket.Bind( new IPEndPoint( IPAddress.IPv6Any, 0 ) );
			if( Socket.ProtocolType == ProtocolType.Tcp )
				Socket.NoDelay = true;
			Timeout = 500;
		}

		public ConnectionTunnel( Socket socket ) {
			Socket = socket;
			if( Socket.ProtocolType == ProtocolType.Tcp )
				Socket.NoDelay = true;
			Timeout = 500;
		}

		public ConnectionTunnel( Socket socket, IPEndPoint remote ) {
			Socket = socket;
			if( Socket.ProtocolType == ProtocolType.Tcp )
				Socket.NoDelay = true;
			Timeout = 500;

			SetRemote( remote );
		}

		public void SetRemote( IPEndPoint remote ) {
			remoteEndPoint = remote;
		}

		public IPEndPoint GetRemote() {
			if( remoteEndPoint != null )
				return remoteEndPoint;
			try {
				return Socket.RemoteEndPoint as IPEndPoint;
			} catch( Exception e ) {
				MemLib.Mem.Logs.Routine.WriteLine( e.ToString() );
			}
			return new IPEndPoint( IPAddress.Any, 0 );
		}

		/// <summary>
		/// If necessary, the tunnel can connect to an Address. This is typically used for TCP protocols and similar. UDP does not utilize this function and might produce any error!
		/// </summary>
		/// <param name="address"></param>
		public bool ConnectTCP( IPEndPoint address, string name ) {
			if( IsConnecting )
				return false;
			IsConnecting = true;
			try {
				Socket.Connect( address );
				this.name = name;
				Send( new PacketConnection( name ) );
				IsConnecting = false;
				return true;
			} catch( SocketException e ) {
				MemLib.Mem.Logs.Routine.WriteLine( e.ToString() );
			} catch( Exception e ) {
				MemLib.Mem.Logs.Routine.WriteLine( e.ToString() );
			}
			return false;
		}

		public void Stop() {
			new Task( delegate () {
				if( !( Receiver is null ) ) {
					Receiver.Running = false;
					Receiver.ConnectionFailureEvent -= ReceiverConnectionFailureEvent;
					Receiver.ReceiverClosedEvent -= ReceiverClosedEvent;
				}
				Receiver = null;
				Send( new PacketDisconnection( name ) );
				Socket.Close( Timeout );
			} ).Start();
		}

		public PacketReceiver CreateReceiver( PacketReader reader ) {
			if( Receiver != null ) {
				MemLib.Mem.Logs.Error.WriteLine( "Packet Receiver already exists for " + Socket.ProtocolType + "!" );
				return null;
			}
			Receiver = new PacketReceiver( Socket, reader );
			Receiver.ConnectionFailureEvent += ReceiverConnectionFailureEvent;
			Receiver.ReceiverClosedEvent += ReceiverClosedEvent;
			return Receiver;
		}

		private void ReceiverConnectionFailureEvent( Socket s, SocketException e ) {
			ConnectionFailureEvent?.Invoke( s, e );
		}

		private void ReceiverClosedEvent() {
			if( !( Receiver is null ) ) {
				Receiver.ConnectionFailureEvent -= ReceiverConnectionFailureEvent;
				Receiver.ReceiverClosedEvent -= ReceiverClosedEvent;
			}
			Receiver = null;
		}

		public void Send( Packet p ) {
			try {
				Socket.SendTo( p.ByteArray, RemoteEndPoint );
			} catch( SocketException e ) {
				ConnectionFailureEvent?.Invoke( Socket, e );
				MemLib.Mem.Logs.Routine.WriteLine( e.ToString() );
			} catch( Exception e ) {
				MemLib.Mem.Logs.Routine.WriteLine( e.ToString() );
			}
		}

	}
}
