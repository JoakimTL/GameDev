using System;
using System.Collections.Generic;
using System.Text;

namespace PacketTesting {
	class PacketCOn : Packet {
		public PacketCOn( byte[] data ) : base( data ) {
			Console.WriteLine( "pc:" + GetType() );
		}

		class aaa : Packet {
			public aaa( byte[] data ) : base( data ) {

			}
		}

	}
}
