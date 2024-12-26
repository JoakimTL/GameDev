using Engine.MemLib;
using Engine.Networking.Packets;
using Engine.Utilities.IO;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Networking {
	//Used to send packets
	public class ConnectionTunnel {

		const int SIO_UDP_CONNRESET = -1744830452;

		public Socket Socket { get; private set; }
		public PacketReceiver Receiver { get; private set; }

		public bool IsConnecting { get; private set; }
		public event ConnectionFailureHandler ConnectionFailureEvent;

		private IPEndPoint remoteEndPoint;
		public IPEndPoint RemoteEndPoint => GetRemote();

		public readonly bool ShouldCloseWithConnection;
		private AutoResetEvent recClosing;

		public int Timeout { get; private set; }

		//https://stackoverflow.com/questions/47779248/why-is-there-a-remote-closed-connection-exception-for-udp-sockets for udp connection forccibly closed error.
		internal ConnectionTunnel( SocketType socketType, ProtocolType protocol, bool shouldCloseWithConnection ) {
			Socket = new Socket( AddressFamily.InterNetworkV6, socketType, protocol );
			Socket.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0 );
			Socket.Bind( new IPEndPoint( IPAddress.IPv6Any, 0 ) );
			if( Socket.ProtocolType == ProtocolType.Tcp )
				Socket.NoDelay = true;
			if( Socket.ProtocolType == ProtocolType.Udp )
				Socket.IOControl( SIO_UDP_CONNRESET, new byte[] { 0 }, null );
			Timeout = 500;
			ShouldCloseWithConnection = shouldCloseWithConnection;
			recClosing = new AutoResetEvent( true );
		}

		internal ConnectionTunnel( Socket socket, bool shouldCloseWithConnection ) {
			Socket = socket;
			if( Socket.ProtocolType == ProtocolType.Tcp )
				Socket.NoDelay = true;
			if( Socket.ProtocolType == ProtocolType.Udp )
				Socket.IOControl( SIO_UDP_CONNRESET, new byte[] { 0 }, null );
			Timeout = 500;
			ShouldCloseWithConnection = shouldCloseWithConnection;
			recClosing = new AutoResetEvent( true );
		}

		internal ConnectionTunnel( Socket socket, IPEndPoint remote, bool shouldCloseWithConnection ) {
			Socket = socket;
			if( Socket.ProtocolType == ProtocolType.Tcp )
				Socket.NoDelay = true;
			if( Socket.ProtocolType == ProtocolType.Udp )
				Socket.IOControl( SIO_UDP_CONNRESET, new byte[] { 0 }, null );
			Timeout = 500;

			SetRemote( remote );
			ShouldCloseWithConnection = shouldCloseWithConnection;
			recClosing = new AutoResetEvent( true );
		}

		internal void SetRemote( IPEndPoint remote ) {
			remoteEndPoint = remote;
		}

		public IPEndPoint GetRemote() {
			if( remoteEndPoint != null )
				return remoteEndPoint;
			try {
				return Socket.RemoteEndPoint as IPEndPoint;
			} catch( Exception e ) {
				Logging.Warning( e.ToString() );
			}
			return new IPEndPoint( IPAddress.Any, 0 );
		}

		/// <summary>
		/// If necessary, the tunnel can connect to an Address. This is typically used for TCP protocols and similar. UDP does not utilize this function and might produce any error!
		/// </summary>
		/// <param name="address"></param>
		internal bool ConnectTCP( IPEndPoint address ) {
			if( IsConnecting )
				return false;
			IsConnecting = true;
			try {
				Socket.Connect( address );
				IsConnecting = false;
				return true;
			} catch( SocketException e ) {
				Logging.Warning( e.ToString() );
			} catch( Exception e ) {
				Logging.Warning( e.ToString() );
			}
			return false;
		}

		internal void Stop() {
			new Task( delegate () {
				if( recClosing.WaitOne( 0 ) ) {
					if( !( Receiver is null ) ) {
						Receiver.ReceiverClosedEvent -= ReceiverClosedEvent;
						Receiver.ConnectionFailureEvent -= ReceiverConnectionFailureEvent;
						Receiver.Running = false;
					}
					Receiver = null;
					recClosing.Set();
				}
				Socket.Close( Timeout );
			} ).Start();
		}

		internal PacketReceiver CreateReceiver( PacketReader reader ) {
			recClosing.WaitOne();
			if( Receiver != null ) {
				Logging.Warning( "Packet Receiver already exists for " + Socket.ProtocolType + "!" );
				recClosing.Set();
				return null;
			}
			Receiver = new PacketReceiver( Socket, reader );
			Receiver.ConnectionFailureEvent += ReceiverConnectionFailureEvent;
			Receiver.ReceiverClosedEvent += ReceiverClosedEvent;
			recClosing.Set();
			return Receiver;
		}

		private void ReceiverConnectionFailureEvent( Socket s, SocketException e ) {
			ConnectionFailureEvent?.Invoke( s, e );
		}

		private void ReceiverClosedEvent() {
			if( recClosing.WaitOne( 0 ) ) {
				if( !( Receiver is null ) ) {
					Receiver.ConnectionFailureEvent -= ReceiverConnectionFailureEvent;
					Receiver.ReceiverClosedEvent -= ReceiverClosedEvent;
				}
				Receiver = null;
				recClosing.Set();
			}
		}

		public void Send( Packet p ) {
			if( Socket.ProtocolType == ProtocolType.Tcp && !Socket.Connected )
				return;
			try {
				//Console.WriteLine( $"new packet sent: [{RemoteEndPoint},{Socket.ProtocolType}][{p.Protocol},{p.DataType},{p.PacketID}]" );
				Socket.SendTo( p.DataBytes.ToArray(), RemoteEndPoint );
			} catch( SocketException e ) {
				ConnectionFailureEvent?.Invoke( Socket, e );
				Logging.Warning( "Failed send of " + Socket.ProtocolType + " data to " + RemoteEndPoint + ": " + e.ToString() );
			} catch( Exception e ) {
				Logging.Warning( e.ToString() );
			}
		}

	}
}
