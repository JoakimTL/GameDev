using System;
using System.Collections.Generic;
using System.Text;

namespace PacketTesting {
	class PacketTest :Packet {
		public PacketTest( byte[] data ) : base( data ) {
			Console.WriteLine( "pt:" + GetType() );
		}

		public PacketTest( int lol ) : base( new byte[] { (byte) lol } ) {
			Console.WriteLine( "pt:" + GetType() );
		}

	}
}
