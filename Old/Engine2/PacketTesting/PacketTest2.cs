using System;
using System.Collections.Generic;
using System.Text;

namespace PacketTesting {
	class PacketTest2 : PacketTest {
		/*
		public PacketTest2( byte[] data ) : base(data) {

		}
		*/

		public PacketTest2( int lol ) : base( new byte[] { (byte) lol } ) {
			Console.WriteLine( "pt:" + GetType() );
		}

	}
}
