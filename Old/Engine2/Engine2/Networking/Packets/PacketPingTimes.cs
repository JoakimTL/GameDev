using Engine.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Net;

namespace Engine.Networking.Packets {
	public class PacketPingTimes : Packet {

		public Dictionary<string, ConnectionTiming> Timings { get; private set; }

		public PacketPingTimes( HashSet<Connection> cons ) : base( GetData( cons ) ) {
			SetTimings();
		}

		private static IReadOnlyList<byte>[] GetData( HashSet<Connection> cons ) {
			List<List<byte>> data = new List<List<byte>>();
			foreach( var c in cons ) {
				if( !string.IsNullOrEmpty( c.Name ) ) {
					List<byte> lb = new List<byte>();
					lb.AddRange( Segmentation.Segment( c.Name, DataTransform.ToString( DataTransform.GetBytes( c.PingTCP ) ), DataTransform.ToString( DataTransform.GetBytes( c.PingUDP ) ) ) );
					data.Add( lb );
				}
			}
			return data.ToArray();
		}

		private void SetTimings() {
			Timings = new Dictionary<string, ConnectionTiming>();
			for( int i = 0; i < Content.Count; i++ ) {
				Segmentation.Parse( DataBytes, out byte[][] segments, out _, SegmentStart[ i ] );
				Timings[ DataTransform.ToString( segments[ 0 ] ) ] = new ConnectionTiming( DataTransform.ToFloat32( segments[ 1 ], 0 ), DataTransform.ToFloat32( segments[ 2 ], 0 ) );
			}
		}

		public PacketPingTimes( IPEndPoint remoteEndpoint, IReadOnlyList<byte> inData ) : base( remoteEndpoint, inData ) {
			SetTimings();
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
