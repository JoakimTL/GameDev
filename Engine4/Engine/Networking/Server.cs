using System.Net;
using System.Net.Sockets;

namespace Engine.Networking;
public abstract class ConnectionBase {

	protected Socket Socket { get; private set; }
	public bool IsConnectionLess { get; private set; }
	private readonly DataReceiver _dataReceiver;

	protected ConnectionBase( ProtocolType protocol ) {
		this.Socket = CreateSocket( protocol, false, out bool connectionless );
		this.IsConnectionLess = connectionless;
		this._dataReceiver = new();
	}

	private static Socket CreateSocket( ProtocolType protocol, bool ipv6, out bool connectionless ) {
		switch ( protocol ) {
			case ProtocolType.Tcp:
				connectionless = false;
				return new Socket( ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Stream, protocol );
			case ProtocolType.Udp:
				connectionless = true;
				return new Socket( ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Dgram, protocol );
			default:
				throw new ArgumentException( $"Protocol {protocol} is not supported!" );
		}
	}

	public void Send( ReadOnlySpan<byte> data, IPEndPoint? endpoint ) {
		if ( endpoint is null ) {
			if ( this.IsConnectionLess )
				throw new InvalidOperationException( "Can't use connected sends on a connectionless socket!" );
			try {
				this.Socket.Send( data );
			} catch ( SocketException e ) {
				HandleSocketException( e );
			}
		} else {
			if ( !this.IsConnectionLess )
				throw new InvalidOperationException( "Can't use connectionless sends on a connected socket!" );
			try {
				this.Socket.SendTo( data, endpoint );
			} catch ( SocketException e ) {
				HandleSocketException( e );
			}
		}
	}

	/*public void Receive( ref byte[]? buffer, out IPEndPoint? sender ) {
		if ( buffer is null )
			buffer = new byte[ 1024 ];
		EndPoint endpoint = new IPEndPoint( IPAddress.Any, 0 );
		int l = 0;
		try {
			if ( this.IsConnectionLess ) {
				l = this.Socket.ReceiveFrom( buffer, buffer.Length, SocketFlags.None, ref endpoint );
			} else {
				l = this.Socket.Receive( buffer, buffer.Length, SocketFlags.None );
				endpoint = this.Socket.RemoteEndPoint ?? throw new InvalidOperationException( "Remote for bound connection is null." );
			}
		} catch ( SocketException e ) {
			HandleSocketException( e );
		}

		this._dataReceiver.HandleData( endpoint, buffer, l );
	}*/

	private void HandleSocketException( SocketException e ) {
		throw e;
	}
}

public class Server : ConnectionBase {

	public int bytesSent = 0;
	public int bytesReceived = 0;
	public int received = 0;

	public Server() : base( ProtocolType.Udp ) {
		this.Socket.Bind( new IPEndPoint( IPAddress.Any, 12345 ) );
		//this.Socket.Listen( 1 );
		Task.Run( () =>
		{
			EndPoint e = new IPEndPoint( IPAddress.Any, 0 );
			Thread.Sleep( 1000 );
			{
				byte[] buf = new byte[ 512 ];
				int i = 0;
				while ( true ) {
					int l = this.Socket.ReceiveFrom( buf, ref e );
					Console.WriteLine( $"server rec[{l}]: " + string.Join( ",", buf[ ..l ].Select( p => p.ToString( "X2" ) ) ) );
					this.received++;
					this.bytesReceived += l;
					if ( l == 32 && buf.Take(l).All( p => p == 0 ) ) {
						Console.WriteLine( $"server: No more data. Received {this.received} packets." );
						break;
					}
				}
			}
			{
				Random rnd = new();
				for ( int i = 0; i < 64; i++ ) {
					byte[] data = new byte[ rnd.Next( 64 ) + 1 ];
					for ( int j = 0; j < data.Length; j++ ) {
						data[ j ] = (byte) ( i + j );
					}
					this.Socket.SendTo( data, e );
					this.bytesSent += data.Length;
					Console.WriteLine( $"server sent[{data.Length}]: {string.Join( ",", data.Select( p => p.ToString( "X2" ) ) )}" );
				}
				{
					byte[] data = new byte[ 32 ];
					for ( int j = 0; j < data.Length; j++ ) {
						data[ j ] = 0;
					}
					this.Socket.SendTo( data, e );
					Console.WriteLine( $"server sent[{data.Length}]: {string.Join( ",", data.Select( p => p.ToString( "X2" ) ) )}" );
				}
			}
		} );
	}
}

public class Client : ConnectionBase {

	public int bytesSent = 0;
	public int bytesReceived = 0;
	public int received = 0;

	public Client() : base( ProtocolType.Udp ) {
		//this.Socket.Connect( new IPEndPoint( IPAddress.Loopback, 12345 ) );
		Task.Run( () =>
		{
			{
				Random rnd = new();
				for ( int i = 0; i < 64; i++ ) {
					byte[] data = new byte[ rnd.Next( 64 ) + 1 ];
					for ( int j = 0; j < data.Length; j++ ) {
						data[ j ] = (byte) ( i + j );
					}
					this.Socket.SendTo( data, new IPEndPoint( IPAddress.Loopback, 12345 ) );
					this.bytesSent += data.Length;
					Console.WriteLine( $"client sent[{data.Length}]: {string.Join( ",", data.Select( p => p.ToString( "X2" ) ) )}" );
				}
				{
					byte[] data = new byte[ 32 ];
					for ( int j = 0; j < data.Length; j++ ) {
						data[ j ] = 0;
					}
					this.Socket.SendTo( data, new IPEndPoint( IPAddress.Loopback, 12345 ) );
					Console.WriteLine( $"client sent[{data.Length}]: {string.Join( ",", data.Select( p => p.ToString( "X2" ) ) )}" );
				}
			}
			Thread.Sleep( 1500 );
			{
				byte[] buf = new byte[ 512 ];
				Thread.Sleep( 1000 );
				EndPoint e = new IPEndPoint( IPAddress.Any, 0 );
				int i = 0;
				while ( true ) {
					int l = this.Socket.ReceiveFrom( buf, ref e );
					Console.WriteLine( $"client rec[{l}]: " + string.Join( ",", buf[ ..l ].Select( p => p.ToString( "X2" ) ) ) );
					this.received++;
					this.bytesReceived += l;
					if ( l == 32 && buf.Take( l ).All( p => p == 0 ) ) {
						Console.WriteLine( $"client: No more data. Received {this.received} packets." );
						break;
					}
				}
			}
		} );
	}
}
