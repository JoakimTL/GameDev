using System;
using System.Collections.Generic;

namespace ED {
	public static class DataTransform {

		#region GetBytes
		public static byte[] GetBytes( string s ) {
			char[] c = s.ToCharArray();
			byte[] r = new byte[ s.Length ];
			for( int i = 0; i < c.Length; i++ )
				r[ i ] = (byte) c[ i ];
			return r;
		}
		public static byte[] GetBytesLiteral( string s ) {
			char[] c = s.ToCharArray();
			byte[] r = new byte[ s.Length * 2 ];
			for( int i = 0; i < c.Length; i += 2 ) {
				r[ i ] = (byte) c[ i ];
				r[ i + 1 ] = (byte) ( c[ i + 1 ] >> 8 & 0xff );
			}
			return r;
		}
		#region Values
		public static byte[] GetBytes( char v ) => GetBytes( (short) v );

		public static byte[] GetBytes( short v ) {
			return BitConverter.GetBytes( v );
		}

		public static byte[] GetBytes( ushort v ) {
			return BitConverter.GetBytes( v );
		}

		public static byte[] GetBytes( int v ) {
			return BitConverter.GetBytes( v );
		}

		public static byte[] GetBytes( uint v ) {
			return BitConverter.GetBytes( v );
		}

		public static byte[] GetBytes( long v ) {
			return BitConverter.GetBytes( v );
		}

		public static unsafe byte[] GetBytes( ulong v ) {
			return BitConverter.GetBytes( v );
		}

		public static unsafe byte[] GetBytes( float v ) {
			return BitConverter.GetBytes( v );
		}

		public static unsafe byte[] GetBytes( double v ) {
			return BitConverter.GetBytes( v );
		}
		#endregion
		#region Arrays
		public static byte[] GetBytes( params float[] values ) {
			byte[] data = new byte[ values.Length * sizeof( float ) ];
			for( int i = 0; i < values.Length; i++ ) {
				byte[] bytes = BitConverter.GetBytes( values[ i ] );
				for( int j = 0; j < bytes.Length; j++ )
					data[ i * sizeof( float ) + j ] = bytes[ j ];
			}
			return data;
		}

		public static byte[] GetBytes( params double[] values ) {
			byte[] data = new byte[ values.Length * sizeof( double ) ];
			for( int i = 0; i < values.Length; i++ ) {
				byte[] bytes = BitConverter.GetBytes( values[ i ] );
				for( int j = 0; j < bytes.Length; j++ )
					data[ i * sizeof( double ) + j ] = bytes[ j ];
			}
			return data;
		}

		public static byte[] GetBytes( params short[] values ) {
			byte[] data = new byte[ values.Length * sizeof( short ) ];
			for( int i = 0; i < values.Length; i++ ) {
				byte[] bytes = BitConverter.GetBytes( values[ i ] );
				for( int j = 0; j < bytes.Length; j++ )
					data[ i * sizeof( short ) + j ] = bytes[ j ];
			}
			return data;
		}

		public static byte[] GetBytes( params ushort[] values ) {
			byte[] data = new byte[ values.Length * sizeof( ushort ) ];
			for( int i = 0; i < values.Length; i++ ) {
				byte[] bytes = BitConverter.GetBytes( values[ i ] );
				for( int j = 0; j < bytes.Length; j++ )
					data[ i * sizeof( ushort ) + j ] = bytes[ j ];
			}
			return data;
		}


		public static byte[] GetBytes( params int[] values ) {
			byte[] data = new byte[ values.Length * sizeof( int ) ];
			for( int i = 0; i < values.Length; i++ ) {
				byte[] bytes = BitConverter.GetBytes( values[ i ] );
				for( int j = 0; j < bytes.Length; j++ )
					data[ i * sizeof( int ) + j ] = bytes[ j ];
			}
			return data;
		}

		public static byte[] GetBytes( params uint[] values ) {
			byte[] data = new byte[ values.Length * sizeof( uint ) ];
			for( int i = 0; i < values.Length; i++ ) {
				byte[] bytes = BitConverter.GetBytes( values[ i ] );
				for( int j = 0; j < bytes.Length; j++ )
					data[ i * sizeof( uint ) + j ] = bytes[ j ];
			}
			return data;
		}

		public static byte[] GetBytes( params long[] values ) {
			byte[] data = new byte[ values.Length * sizeof( long ) ];
			for( int i = 0; i < values.Length; i++ ) {
				byte[] bytes = BitConverter.GetBytes( values[ i ] );
				for( int j = 0; j < bytes.Length; j++ )
					data[ i * sizeof( long ) + j ] = bytes[ j ];
			}
			return data;
		}

		public static byte[] GetBytes( params ulong[] values ) {
			byte[] data = new byte[ values.Length * sizeof( ulong ) ];
			for( int i = 0; i < values.Length; i++ ) {
				byte[] bytes = BitConverter.GetBytes( values[ i ] );
				for( int j = 0; j < bytes.Length; j++ )
					data[ i * sizeof( ulong ) + j ] = bytes[ j ];
			}
			return data;
		}
		#endregion
		#endregion

		#region ToValue
		#region Values
		#region Bytes
		public static char ToChar( byte[] arr, int index = 0 ) {
			return BitConverter.ToChar( arr, index );
		}
		public static short ToInt16( byte[] arr, int index = 0 ) {
			return BitConverter.ToInt16( arr, index );
		}
		public static ushort ToUInt16( byte[] arr, int index = 0 ) {
			return BitConverter.ToUInt16( arr, index );
		}
		public static int ToInt32( byte[] arr, int index = 0 ) {
			return BitConverter.ToInt32( arr, index );
		}
		public static uint ToUInt32( byte[] arr, int index = 0 ) {
			return BitConverter.ToUInt32( arr, index );
		}
		public static long ToInt64( byte[] arr, int index = 0 ) {
			return BitConverter.ToInt64( arr, index );
		}
		public static unsafe ulong ToUInt64( byte[] arr, int index = 0 ) {
			return BitConverter.ToUInt64( arr, index );
		}
		public static unsafe float ToFloat32( byte[] arr, int index = 0 ) {
			return BitConverter.ToSingle( arr, index );
		}
		public static unsafe double ToFloat64( byte[] arr, int index = 0 ) {
			return BitConverter.ToDouble( arr, index );
		}
		#endregion
		#region Strings
		#region Literal
		/// <summary>Treats the chars are 2 byte values</summary>
		public static char ToCharLiteral( string arr, int index = 0 ) {
			return arr[ 0 ];
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static short ToInt16Literal( string arr, int index = 0 ) {
			return (short) arr[ 0 ];
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static ushort ToUInt16Literal( string arr, int index = 0 ) {
			return arr[ 0 ];
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static int ToInt32Literal( string arr, int index = 0 ) {
			return BitConverter.ToInt32( GetBytesLiteral( arr ), index );
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static uint ToUInt32Literal( string arr, int index = 0 ) {
			return BitConverter.ToUInt32( GetBytesLiteral( arr ), index );
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static long ToInt64Literal( string arr, int index = 0 ) {
			return BitConverter.ToInt64( GetBytesLiteral( arr ), index );
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static ulong ToUInt64Literal( string arr, int index = 0 ) {
			return BitConverter.ToUInt64( GetBytesLiteral( arr ), index );
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static unsafe float ToFloat32Literal( string arr, int index = 0 ) {
			return BitConverter.ToSingle( GetBytesLiteral( arr ), index );
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static unsafe double ToFloat64Literal( string arr, int index = 0 ) {
			return BitConverter.ToDouble( GetBytesLiteral( arr ), index );
		}
		#endregion
		#region As Bytes
		public static char ToChar( string arr, int index = 0 ) {
			return BitConverter.ToChar( GetBytes( arr ), index );
		}
		public static short ToInt16( string arr, int index = 0 ) {
			return BitConverter.ToInt16( GetBytes( arr ), index );
		}
		public static ushort ToUInt16( string arr, int index = 0 ) {
			return BitConverter.ToUInt16( GetBytes( arr ), index );
		}
		public static int ToInt32( string arr, int index = 0 ) {
			return BitConverter.ToInt32( GetBytes( arr ), index );
		}
		public static uint ToUInt32( string arr, int index = 0 ) {
			return BitConverter.ToUInt32( GetBytes( arr ), index );
		}
		public static long ToInt64( string arr, int index = 0 ) {
			return BitConverter.ToInt64( GetBytes( arr ), index );
		}
		public static ulong ToUInt64( string arr, int index = 0 ) {
			return BitConverter.ToUInt64( GetBytes( arr ), index );
		}
		public static unsafe float ToFloat32( string arr, int index = 0 ) {
			return BitConverter.ToSingle( GetBytes( arr ), index );
		}
		public static unsafe double ToFloat64( string arr, int index = 0 ) {
			return BitConverter.ToDouble( GetBytes( arr ), index );
		}
		#endregion
		#endregion
		#endregion
		#region Arrays
		public static float[] ToFloat32Array( byte[] data ) {
			float[] array = new float[ data.Length / sizeof( float ) ];
			for( int i = 0; i < array.Length; i++ )
				array[ i ] = BitConverter.ToSingle( data, i * sizeof( float ) );
			return array;
		}
		public static double[] ToFloat64Array( byte[] data ) {
			double[] array = new double[ data.Length / sizeof( double ) ];
			for( int i = 0; i < array.Length; i++ )
				array[ i ] = BitConverter.ToDouble( data, i * sizeof( double ) );
			return array;
		}
		public static short[] ToInt16Array( byte[] data ) {
			short[] array = new short[ data.Length / sizeof( short ) ];
			for( int i = 0; i < array.Length; i++ )
				array[ i ] = BitConverter.ToInt16( data, i * sizeof( short ) );
			return array;
		}
		public static ushort[] ToUInt16Array( byte[] data ) {
			ushort[] array = new ushort[ data.Length / sizeof( ushort ) ];
			for( int i = 0; i < array.Length; i++ )
				array[ i ] = BitConverter.ToUInt16( data, i * sizeof( ushort ) );
			return array;
		}
		public static int[] ToInt32Array( byte[] data ) {
			int[] array = new int[ data.Length / sizeof( int ) ];
			for( int i = 0; i < array.Length; i++ )
				array[ i ] = BitConverter.ToInt32( data, i * sizeof( int ) );
			return array;
		}
		public static uint[] ToUInt32Array( byte[] data ) {
			uint[] array = new uint[ data.Length / sizeof( uint ) ];
			for( int i = 0; i < array.Length; i++ )
				array[ i ] = BitConverter.ToUInt32( data, i * sizeof( uint ) );
			return array;
		}
		public static long[] ToInt64Array( byte[] data ) {
			long[] array = new long[ data.Length / sizeof( long ) ];
			for( int i = 0; i < array.Length; i++ )
				array[ i ] = BitConverter.ToInt64( data, i * sizeof( long ) );
			return array;
		}
		public static ulong[] ToUInt64Array( byte[] data ) {
			ulong[] array = new ulong[ data.Length / sizeof( ulong ) ];
			for( int i = 0; i < array.Length; i++ )
				array[ i ] = BitConverter.ToUInt64( data, i * sizeof( ulong ) );
			return array;
		}
		#endregion
		#region ToString
		public static string ToString( byte[] b, int len ) {
			char[] c = new char[ len ];
			int l = Math.Min( b.Length, len );
			int i = 0;
			for( ; i < l; i++ )
				c[ i ] = (char) b[ i ];
			for( ; i < len; i++ )
				c[ i ] = (char) 0;
			return new string( c );
		}
		public static string ToString( IReadOnlyList<byte> b, int len ) {
			char[] c = new char[ len ];
			int l = Math.Min( b.Count, len );
			int i = 0;
			for( ; i < l; i++ )
				c[ i ] = (char) b[ i ];
			for( ; i < len; i++ )
				c[ i ] = (char) 0;
			return new string( c );
		}
		public static string ToString( byte[] b ) {
			char[] c = new char[ b.Length ];
			for( int i = 0; i < c.Length; i++ )
				c[ i ] = (char) b[ i ];
			return new string( c );
		}
		public static string ToString( IReadOnlyList<byte> b ) {
			char[] c = new char[ b.Count ];
			for( int i = 0; i < c.Length; i++ )
				c[ i ] = (char) b[ i ];
			return new string( c );
		}
		#endregion
		#endregion

	}
}
