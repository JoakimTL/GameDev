using Engine.MemLib;
using Engine.Utilities.Data;
using Engine.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Engine.Networking {
	public abstract class Packet {

		public const int HEADER_SIZE = 8;
		public const int HEADER_PACKETSIZE_INDEX = 0;
		public const int HEADER_PACKETID_INDEX = 4;

		public uint PacketID { get; private set; }
		public Type DataType { get; private set; }
		public ProtocolType Protocol { get; private set; }

		private Segmentation.SegmentData[] segments;
		/// <summary>
		/// Contains the start and length of each segments
		/// </summary>
		public IReadOnlyList<Segmentation.SegmentData> Segments => segments;

		protected readonly byte[] bytes;
		public IReadOnlyList<byte> DataBytes => bytes;

		protected Packet( byte[] data ) {
			PacketID = GetID( data );
			(Type, ProtocolType) info = PacketManager.Get( PacketID );
			DataType = info.Item1;
			Protocol = info.Item2;
			bytes = data;
			if( Segmentation.FindSegments( data, out Segmentation.SegmentData[] segs, HEADER_SIZE ) )
				segments = segs;
		}

		public static uint GetID( byte[] data ) {
			return DataTransform.ToUInt32( data, HEADER_PACKETID_INDEX );
		}

		protected static byte[] GeneratePacketData( Type packetType, params byte[][] data ) {
			uint id = PacketManager.GetID( packetType );
			byte[] bytes = Segmentation.Segment( data );
			byte[] headerLengthBytes = DataTransform.GetBytes( (uint) ( bytes.Length + HEADER_SIZE ) );
			byte[] headerIdBytes = DataTransform.GetBytes( id );
			byte[] returnData = new byte[ bytes.Length + HEADER_SIZE ];
			unsafe {
				fixed( byte* packetData = returnData ) {
					packetData[ 0 ] = headerLengthBytes[ 0 ];
					packetData[ 1 ] = headerLengthBytes[ 1 ];
					packetData[ 2 ] = headerLengthBytes[ 2 ];
					packetData[ 3 ] = headerLengthBytes[ 3 ];
					packetData[ 4 ] = headerIdBytes[ 0 ];
					packetData[ 5 ] = headerIdBytes[ 1 ];
					packetData[ 6 ] = headerIdBytes[ 2 ];
					packetData[ 7 ] = headerIdBytes[ 3 ];
					fixed( byte* bodyData = bytes ) {
						for( int i = 8; i < returnData.Length; i++ )
							packetData[ i ] = bodyData[ i - 8 ];
					}
				}
			}
			return returnData;
		}

		public override string ToString() {
			string body = $"Packet[{PacketID}][]";
			for( int i = 0; i < Segments.Count; i++ )
				body += $"[{Segments[ i ].StartIndex},{Segments[ i ].Length}]";
			body += ":";
			for( int i = 0; i < DataBytes.Count; i++ )
				body += $"{DataBytes[ i ]},";
			return body;
		}

	}
}
