using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Utilities.Data {
	public static class Segmentation {

		/// <summary>
		/// Index of the byte signifying the number of bytes used to represent the amount of segments in the datapackage.
		/// </summary>
		private const int I_NUMSEG = 0;
		/// <summary>
		/// Index of the byte signifying the number of bytes used to represent the size of the segments.
		/// </summary>
		private const int I_LENSEG = 1;
		private const int LAST_INDEX = I_LENSEG;

		#region Parsing
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">The data to parse.</param>
		/// <param name="segments">The segments found in the data. The tuple has two values, the first being the start index of the segment and the second being the length of the segment.</param>
		/// <param name="startIndex">Where to start searching through the data for segments.</param>
		/// <returns></returns>
		public static bool FindSegments( byte[] data, out SegmentData[] segments, int startIndex = 0 ) {
			segments = null;
			if( data.Length <= LAST_INDEX + startIndex )
				return false;

			int btr_Num = data[ I_NUMSEG + startIndex ];
			int segLen = data[ I_LENSEG + startIndex ];

			int index = LAST_INDEX + startIndex;

			int numSegments = (int) GetBytesToUInt( data, btr_Num, ++index );
			segments = new SegmentData[ numSegments ];
			if( numSegments > 0 ) {
				index += btr_Num;

				segments[ 0 ] = new SegmentData( (uint) ( index + numSegments * segLen ), GetBytesToUInt( data, segLen, index ) );
				index += segLen;
				for( int i = 1; i < numSegments; i++ ) {
					SegmentData lastSeg = segments[ i - 1 ];
					segments[ i ] = new SegmentData( lastSeg.StartIndex + lastSeg.Length, GetBytesToUInt( data, segLen, index ) );
					index += segLen;
				}
			}
			return true;
		}
		public static bool Parse( byte[] data, out byte[][] segments, out int[] segStarts, int startIndex = 0 ) {
			segments = null;
			segStarts = null;
			uint[] segLengths;

			if( data.Length <= LAST_INDEX + startIndex )
				return false;

			int btr_Num = data[ I_NUMSEG + startIndex ];
			int segLen = data[ I_LENSEG + startIndex ];

			int index = LAST_INDEX + startIndex;

			int numSegments = (int) GetBytesToUInt( data, btr_Num, ++index );
			index += btr_Num;

			segments = new byte[ numSegments ][];
			segStarts = new int[ numSegments ];
			segLengths = new uint[ numSegments ];

			for( int i = 0; i < numSegments; i++ ) {
				segLengths[ i ] = GetBytesToUInt( data, segLen, index );
				index += segLen;
			}

			for( int i = 0; i < numSegments; i++ ) {
				segments[ i ] = new ArraySegment<byte>( data, index, (int) segLengths[ i ] ).ToArray();
				segStarts[ i ] = index;
				index += (int) segLengths[ i ];
			}

			return true;
		}

		public static bool Parse( IReadOnlyList<byte> data, out byte[][] segments, out int[] segStarts, int startIndex = 0 ) => Parse( data.ToArray(), out segments, out segStarts, startIndex );
		public static bool Parse( IReadOnlyList<byte> data, out IReadOnlyList<byte>[] segments, out int[] segStarts, int startIndex = 0 ) => Parse( data.ToArray(), out segments, out segStarts, startIndex );

		public static bool Parse( byte[] data, out string[] segments, out int[] segStarts, int startIndex ) {
			bool v = Parse( data, out byte[][] segByte, out segStarts, startIndex );
			segments = new string[ segByte.Length ];
			for( int i = 0; i < segments.Length; i++ )
				segments[ i ] = DataTransform.ToString( segByte[ i ] );
			return v;
		}

		public static bool Parse( IReadOnlyList<byte> data, out string[] segments, out int[] segStarts, int startIndex ) {
			bool v = Parse( data, out byte[][] segByte, out segStarts, startIndex );
			segments = new string[ segByte.Length ];
			for( int i = 0; i < segments.Length; i++ )
				segments[ i ] = DataTransform.ToString( segByte[ i ] );
			return v;
		}
		#endregion

		#region Segmentation
		public static byte[] Segment( out int[] segmentStarts, params IReadOnlyList<byte>[] data ) {
			List<byte> bytes = new List<byte>();

			int btr_Num = BytesToRepresent( data.Length );
			bytes.Insert( I_NUMSEG, (byte) btr_Num );

			int longestSegment = 0;
			for( int i = 0; i < data.Length; i++ )
				if( data[ i ].Count > longestSegment )
					longestSegment = data[ i ].Count;

			int btr_Len = BytesToRepresent( longestSegment );

			bytes.Insert( I_LENSEG, (byte) btr_Len );

			bytes.AddRange( GetUIntBytes( (uint) data.Length, btr_Num ) );

			for( int i = 0; i < data.Length; i++ )
				bytes.AddRange( GetUIntBytes( (uint) data[ i ].Count, btr_Len ) );

			segmentStarts = new int[ data.Length ];
			for( int i = 0; i < data.Length; i++ ) {
				segmentStarts[ i ] = bytes.Count;
				bytes.AddRange( data[ i ] );
			}

			return bytes.ToArray();
		}

		public static byte[] Segment( out int[] segmentStarts, params byte[][] data ) {
			List<byte> bytes = new List<byte>();

			int btr_Num = BytesToRepresent( data.Length );
			bytes.Insert( I_NUMSEG, (byte) btr_Num );

			int longestSegment = 0;
			for( int i = 0; i < data.Length; i++ )
				if( data[ i ].Length > longestSegment )
					longestSegment = data[ i ].Length;

			int btr_Len = BytesToRepresent( longestSegment );

			bytes.Insert( I_LENSEG, (byte) btr_Len );

			bytes.AddRange( GetUIntBytes( (uint) data.Length, btr_Num ) );

			for( int i = 0; i < data.Length; i++ )
				bytes.AddRange( GetUIntBytes( (uint) data[ i ].Length, btr_Len ) );

			segmentStarts = new int[ data.Length ];
			for( int i = 0; i < data.Length; i++ ) {
				segmentStarts[ i ] = bytes.Count;
				bytes.AddRange( data[ i ] );
			}

			return bytes.ToArray();
		}

		public static byte[] Segment( params byte[][] data ) => Segment( out _, data );

		public static byte[] SegmentObjects( out int[] segmentStarts, params object[] data ) {
			byte[][] bytedata = new byte[ data.Length ][];
			for( int i = 0; i < data.Length; i++ )
				bytedata[ i ] = DataTransform.GetBytes( data[ i ].ToString() );
			return Segment( out segmentStarts, bytedata );
		}

		public static byte[] Segment( params object[] data ) => SegmentObjects( out _, data );
		#endregion

		#region Helper Methods
		public static int BytesToRepresent( int num ) {
			int n = 1;
			while( ( num >>= 8 ) != 0 ) {
				n++;
			}
			return n;
		}

		public static byte[] GetUIntBytes( uint num, int numBytes ) {
			byte[] d = new byte[ numBytes ];
			for( int i = 1; i <= numBytes; i++ )
				d[ i - 1 ] = (byte) ( ( num % ( 1u << ( 8 * ( numBytes - i + 1 ) ) ) ) / ( 1u << ( 8 * ( numBytes - i ) ) ) );
			return d;
		}

		public static uint GetBytesToUInt( byte[] d, int numBytes, int offset ) {
			uint val = 0;
			for( int i = 1; i <= numBytes; i++ )
				val += d[ offset + ( i - 1 ) ] * ( 1u << ( 8 * ( numBytes - i ) ) );
			return val;
		}
		#endregion

		public struct SegmentData {
			public readonly uint StartIndex;
			public readonly uint Length;

			public SegmentData( uint start, uint len ) {
				StartIndex = start;
				Length = len;
			}
		}
	}
}
