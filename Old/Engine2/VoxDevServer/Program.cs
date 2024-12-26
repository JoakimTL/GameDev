using Engine.MemLib;
using Engine.Networking;
using Engine.Networking.Packets;
using Engine.QuickstartKit;
using Engine.Utilities.Data;
using Engine.Utilities.Time;
using System;
using System.Linq;

namespace VoxDevServer {
	class Program {
		static void Main( string[] args ) {
			int port = 12345;
			//password?
			//server name?

			//Mem.Initialize();
			//PacketManager.ScanPacketTypes();
			//PacketConnection p = new PacketConnection( "test" );
			//PacketConnection p0 = new PacketConnection( p.DataBytes.ToArray() );

			//Console.WriteLine( "p" );
			//for( int i = 0; i < p.DataBytes.Count; i++ ) {
			//	Console.Write( p.DataBytes[ i ] + ", " );
			//}
			//Console.WriteLine();
			//Console.WriteLine( Packet.GetID( p.DataBytes.ToArray() ) );
			//Console.WriteLine( p.Username );
			//for( int i = 0; i < p.Username.Length; i++ ) {
			//	Console.Write( (int) p.Username[ i ] + ", " );
			//}
			//Console.WriteLine();

			//Console.WriteLine("p0");
			//for( int i = 0; i < p0.DataBytes.Count; i++ ) {
			//	Console.Write( p0.DataBytes[ i ] + ", " );
			//}
			//Console.WriteLine();
			//Console.WriteLine( Packet.GetID( p0.DataBytes.ToArray() ) );
			//Console.WriteLine( p0.Username );
			//for( int i = 0; i < p0.Username.Length; i++ ) {
			//	Console.Write( (int) p0.Username[i] + ", " );
			//}
			//Console.WriteLine();

			//Console.WriteLine("test");
			//byte[] d = DataTransform.GetBytes( (uint) 2 );
			//for( int i = 0; i < d.Length; i++ ) {
			//	Console.Write( d[ i ] + ", " );
			//}
			//Console.WriteLine();

			//Console.WriteLine( DataTransform.ToUInt32( d ) );
			Quickstart.Start( new ServerStart( port ) );
		}
	}
}
