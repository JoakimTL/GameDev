using System;
using System.Collections.Generic;
using System.Text;

namespace PacketTesting {
	abstract class Packet {

		public Packet(byte[] data) {

			Console.WriteLine( "p:" + GetType() );
		}

	}
}
