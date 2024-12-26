using Engine.MemLib;
using Engine.Utilities.Data;
using Engine.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Net;

namespace Engine.Networking {
	public abstract class Packet {

		public const int HEADER_SIZE = 8;
		public const int HEADER_COUNT_LOC = 0;
		public const int HEADER_ID_LOC = 4;

		public IPEndPoint RemoteEndpoint { get; private set; }
		public uint ID => Type.ID;
		public PacketType Type { get; private set; }
		//public abstract int NetworkProtocol { get; }

		private string[] content;
		public IReadOnlyList<string> Content => content;
		public int NumSegments => content.Length;

		private int[] segStart; //does not correspond with shit
		public IReadOnlyList<int> SegmentStart => segStart;

		public IReadOnlyList<byte> DataBytes => data.AsReadOnly();
		private List<byte> data;
		public byte[] ByteArray => data.ToArray();

		protected Packet( params IReadOnlyList<byte>[] content ) {
			this.RemoteEndpoint = null;
			this.Type = PacketType.GetPacketType( this.GetType() );
			this.content = new string[ content.Length ];
			for( int i = 0; i < this.content.Length; i++ )
				this.content[ i ] = DataTransform.ToString( content[ i ] );
			data = new List<byte>();
			data.AddRange( Segmentation.Segment( out segStart, this.Content ) );
			data.InsertRange( HEADER_COUNT_LOC, DataTransform.GetBytes( data.Count + HEADER_SIZE ) );
			data.InsertRange( HEADER_ID_LOC, DataTransform.GetBytes( ID ) );
		}

		protected Packet( params string[] content ) {
			this.RemoteEndpoint = null;
			this.Type = PacketType.GetPacketType( this.GetType() );
			this.content = content;
			data = new List<byte>();
			data.AddRange( Segmentation.Segment( out segStart, this.Content ) );
			data.InsertRange( HEADER_COUNT_LOC, DataTransform.GetBytes( data.Count + HEADER_SIZE ) );
			data.InsertRange( HEADER_ID_LOC, DataTransform.GetBytes( ID ) );
		}

		/// <summary>
		/// Creates a packet from a byte List. This process is mostly used when receiving a packet from another address.
		/// </summary>
		/// <param name="inData"></param>
		protected Packet( IPEndPoint remoteEndpoint, IReadOnlyList<byte> inData ) {
			this.RemoteEndpoint = remoteEndpoint;
			data = new List<byte>( inData );
			Segmentation.Parse( inData, out content, out segStart, HEADER_SIZE );
			this.Type = PacketType.GetPacketType( DataTransform.ToUInt32( data.GetRange( HEADER_ID_LOC , 4).ToArray(), HEADER_ID_LOC ) );
		}

		public override string ToString() {
			string body = $"Packet[{Type.ID}]";
			for( int i = 0; i < content.Length; i++ ) {
				body += $"[{Content[ i ]}]";
			}
			return body;
		}

	}
}
