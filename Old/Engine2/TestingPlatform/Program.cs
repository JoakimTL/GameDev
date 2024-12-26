using Engine.MemLib;
using Engine.Networking;
using Engine.Networking.Server;
using Engine.QuickstartKit;
using Engine.Utilities.Graphics.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TestingPlatform {
	class Program {
		static void Main( string[] args ) {
			/*DataArray<byte> bytes = new DataArray<byte>( 32 );

			DataArray<byte>.DataSegment segment1 = bytes.CreateSegment( 8 );
			DataArray<byte>.DataSegment segment2 = bytes.CreateSegment( 8 );

			Console.WriteLine( segment1 );
			Console.WriteLine( segment2 );
			Console.WriteLine( bytes );

			segment1[ 7 ] = 5;
			segment2[ 3 ] = 4;
			Console.WriteLine( segment1 );
			Console.WriteLine( segment2 );
			Console.WriteLine( bytes );

			segment1[ 7 ] = 3;
			segment2[ 3 ] = 6;
			Console.WriteLine( segment1 );
			Console.WriteLine( segment2 );
			Console.WriteLine( bytes );

			DataArray<byte>.DataSegment segment3 = bytes.CreateSegment( 16 );

			Console.WriteLine( segment1 );
			Console.WriteLine( segment2 );
			Console.WriteLine( segment3 );
			Console.WriteLine( bytes );

			for( int i = 6; i < 16; i++ ) {
				segment3[ i ] = (byte) ( i % 10 );
			}

			Console.WriteLine( segment1 );
			Console.WriteLine( segment2 );
			Console.WriteLine( segment3 );
			Console.WriteLine( bytes );

			DataArray<byte>.DataSegment segment4 = bytes.CreateSegment( 16 );

			Console.WriteLine( segment1 );
			Console.WriteLine( segment2 );
			Console.WriteLine( segment3 );
			Console.WriteLine( segment4 );
			Console.WriteLine( bytes );

			for( int i = 5; i < 13; i += 2 ) {
				segment4[ i ] = (byte) ( i % 10 );
			}

			Console.WriteLine( segment1 );
			Console.WriteLine( segment2 );
			Console.WriteLine( segment3 );
			Console.WriteLine( segment4 );
			Console.WriteLine( bytes );

			segment2.Dispose();

			Console.WriteLine( segment1 );
			Console.WriteLine( segment2 );
			Console.WriteLine( segment3 );
			Console.WriteLine( segment4 );
			Console.WriteLine( bytes );

			DataArray<byte>.DataSegment segment5 = bytes.CreateSegment( 4 );

			Console.WriteLine( segment1 );
			Console.WriteLine( segment2 );
			Console.WriteLine( segment3 );
			Console.WriteLine( segment4 );
			Console.WriteLine( segment5 );
			Console.WriteLine( bytes );

			Console.WriteLine( bytes.GetBytesUsed( 1 ) );*/

			/*Socket sock = new Socket( AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp );
			sock.SetSocketOption( SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0 );
			sock.Bind( new IPEndPoint( IPAddress.IPv6Any, 25565 ) );
			sock.Listen( 4 );
			new Task( () => {
				Socket client = sock.Accept();
				Console.WriteLine( "Client has connected successfully with the server" );
			} ).Start();
			new Task( () => {
				Socket sock2 = new Socket( AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp );
				sock2.Connect( new IPEndPoint( IPAddress.Parse( "2001:4646:8ab:0:15e9:eb2a:e302:a977"), 25565 ) );
			} );
			new Task( () => {
				Socket sock2 = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
				sock2.Connect( new IPEndPoint( IPAddress.Parse( "85.164.99.107" ), 25565 ) );
			} ).Start();*/

			/*for( int j = 0; j < 3; j++ ) {

				long time1 = Stopwatch.GetTimestamp();
				for( int i = 0; i < 100_000_000; i++ ) {
					DataTransform.ToUInt64( DataTransform.GetBytes( ulong.MaxValue ) );
				}
				long time2 = Stopwatch.GetTimestamp();
				for( int i = 0; i < 100_000_000; i++ ) {
					BitConverter.ToUInt64( BitConverter.GetBytes( ulong.MaxValue ), 0 );
				}
				long time3 = Stopwatch.GetTimestamp();
				Console.WriteLine( ( time2 - time1 ) / TimeSpan.TicksPerMillisecond );
				Console.WriteLine( ( time3 - time2 ) / TimeSpan.TicksPerMillisecond );
			}

			Console.WriteLine( DataTransform.ToInt64( DataTransform.ToString( DataTransform.GetBytes( 2000341 ), 8 ) ) );*/

			/*Ezmem.Initialize();
			ServerAdmin server = new ServerAdmin( 25565 );
			PacketReader packetReader = new PacketReader();
			Connection con = new Connection( packetReader );
			con.ConnectAsClient( "test", new IPEndPoint( IPAddress.Loopback, 25565 ) );*/

			/*{

				List<int> a = new List<int>();
				Dictionary<int, int> b = new Dictionary<int, int>();

				bool aa = false;

				long t1 = Stopwatch.GetTimestamp();
				if( aa ) {
					for( int i = 0; i < 1_000; i++ ) {
						a.Add( i );
					}
				}
				long t2 = Stopwatch.GetTimestamp();
				if( !aa ) {
					for( int i = 0; i < 1_000; i++ ) {
						b[ i ] = i;
					}
				}
				long t3 = Stopwatch.GetTimestamp();
				if( aa ) {
					int j = 0;
					for( int i = 0; i < 1_000; i++ ) {
						j += a[ i ] * ( ( i % 2 ) * 2 - 1 );
					}
					Console.WriteLine( "r:" + j );
				}
				long t4 = Stopwatch.GetTimestamp();
				if( !aa ) {
					int j = 0;
					for( int i = 0; i < 1_000; i++ ) {
						j += b[ i ] * ( ( i % 2 ) * 2 - 1 );
					}
					Console.WriteLine( "r:" + j );
				}
				long t5 = Stopwatch.GetTimestamp();

				Console.WriteLine( "a: " + (double) ( t2 - t1 ) / TimeSpan.TicksPerMillisecond );
				Console.WriteLine( "b: " + (double) ( t3 - t2 ) / TimeSpan.TicksPerMillisecond );

				Console.WriteLine( "a: " + (double) ( t4 - t3 ) / TimeSpan.TicksPerMillisecond );
				Console.WriteLine( "b: " + (double) ( t5 - t4 ) / TimeSpan.TicksPerMillisecond );
			}*/

			/*{
				Engine.LinearAlgebra.Vector4[] vs1 = new Engine.LinearAlgebra.Vector4[ 1_000_000 ];
				System.Numerics.Vector4[] vs2 = new System.Numerics.Vector4[ 1_000_000 ];
				float[] fs1 = new float[ 1_000_000 ];
				float[] fs2 = new float[ 1_000_000 ];
				long t1 = Stopwatch.GetTimestamp();
				{
					Engine.LinearAlgebra.Vector4 v1 = (20, 30, 40, 50);
					for( int i = 0; i < 1_000_000; i++ ) {
						Engine.LinearAlgebra.Vector4 v2 = (i, i, 20, i);
						vs1[ i ] = v1 + v2;
					}
				}
				long t2 = Stopwatch.GetTimestamp();

				{
					System.Numerics.Vector4 v1 = new System.Numerics.Vector4(20, 30, 40, 50);
					for( int i = 0; i < 1_000_000; i++ ) {
						System.Numerics.Vector4 v2 = new System.Numerics.Vector4(i, i, 20, i);
						vs2[ i ] = v1 + v2;
					}
				}
				long t3 = Stopwatch.GetTimestamp();

				Console.WriteLine( (double) ( t2 - t1 ) / TimeSpan.TicksPerMillisecond );
				Console.WriteLine( (double) ( t3 - t2 ) / TimeSpan.TicksPerMillisecond );
			}
			
			Console.ReadLine();*/
			Quickstart.Start( new Client() );
		}
	}
}
