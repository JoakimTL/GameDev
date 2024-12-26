using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketPingTimes : Packet {

		public Dictionary<string, ConnectionTiming> Timings { get; private set; }

		public PacketPingTimes( byte[] data ) : base( data ) {
			SetTimings();
		}

		public PacketPingTimes( HashSet<Connection> cons ) : base( GeneratePacketData( typeof( PacketPingTimes ), GetData(cons) ) ) {
			SetTimings();
		}

		private static byte[][] GetData( HashSet<Connection> cons ) {
			List<byte[]> data = new List<byte[]>();
			foreach( var c in cons ) {
				if( !string.IsNullOrEmpty( c.Name ) ) {
					data.Add( Segmentation.Segment( c.Name, DataTransform.ToString( DataTransform.GetBytes( c.PingTCP ) ), DataTransform.ToString( DataTransform.GetBytes( c.PingUDP ) ) ) );
				}
			}
			return data.ToArray();
		}

		private void SetTimings() {
			Timings = new Dictionary<string, ConnectionTiming>();
			for( int i = 0; i < Segments.Count; i++ ) {
				Segmentation.Parse( bytes, out byte[][] segments, out _, (int) Segments[i].StartIndex );
				Timings[ DataTransform.ToString( segments[ 0 ] ) ] = new ConnectionTiming( DataTransform.ToFloat32( segments[ 1 ], 0 ), DataTransform.ToFloat32( segments[ 2 ], 0 ) );
			}
		}

	}

	public struct ConnectionTiming {

		public float TCP { get; private set; }
		public float UDP { get; private set; }

		public ConnectionTiming( float tcp, float udp ) {
			TCP = tcp;
			UDP = udp;
		}

	}
}
