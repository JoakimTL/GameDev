using Engine.Networking;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDev {
	public class PacketTest : Packet {
		public PacketTest( byte[] data ) : base( data ) {
		}
	}
}
