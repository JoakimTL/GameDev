using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;

namespace Engine.Utilities.Data {
	public static class DataTransform {

		#region GetBytes
		#region Strings
		public static byte[] GetBytes( string s ) {
			if( string.IsNullOrEmpty(s) )
				return new byte[ 0 ];
			byte[] r = new byte[ s.Length * sizeof( char ) ];
			unsafe {
				fixed( char* valuePointer = s ) {
					byte* valueDataPointer = (byte*) valuePointer;
					for( int i = 0; i < r.Length; i++ )
						r[ i ] = valueDataPointer[ i ];
				}
			}
			return r;
		}
		public static byte[][] GetByteArray( params string[] s ) {
			byte[][] r = new byte[ s.Length ][];
			for( int j = 0; j < s.Length; j++ ) {
				r[ j ] = new byte[ s[ j ].Length * sizeof( char ) ];
				unsafe {
					fixed( char* valuePointer = s[ j ] ) {
						byte* valueDataPointer = (byte*) valuePointer;
						for( int i = 0; i < r.Length; i++ )
							r[ j ][ i ] = valueDataPointer[ i ];
					}
				}
			}
			return r;
		}
		#endregion
		#region Single Values
		public static byte[] GetBytes( char v ) {
			unsafe {
				byte* valueDataPointer = (byte*) &v;
				byte[] r = new byte[ sizeof( char ) ];
				for( int i = 0; i < r.Length; i++ )
					r[ i ] = valueDataPointer[ i ];
				return r;
			}
		}
		public static byte[] GetBytes( short v ) {
			unsafe {
				byte* valueDataPointer = (byte*) &v;
				byte[] r = new byte[ sizeof( short ) ];
				for( int i = 0; i < r.Length; i++ )
					r[ i ] = valueDataPointer[ i ];
				return r;
			}
		}
		public static byte[] GetBytes( ushort v ) {
			unsafe {
				byte* valueDataPointer = (byte*) &v;
				byte[] r = new byte[ sizeof( ushort ) ];
				for( int i = 0; i < r.Length; i++ )
					r[ i ] = valueDataPointer[ i ];
				return r;
			}
		}
		public static byte[] GetBytes( int v ) {
			unsafe {
				byte* valueDataPointer = (byte*) &v;
				byte[] r = new byte[ sizeof( int ) ];
				for( int i = 0; i < r.Length; i++ )
					r[ i ] = valueDataPointer[ i ];
				return r;
			}
		}
		public static byte[] GetBytes( uint v ) {
			unsafe {
				byte* valueDataPointer = (byte*) &v;
				byte[] r = new byte[ sizeof( uint ) ];
				for( int i = 0; i < r.Length; i++ )
					r[ i ] = valueDataPointer[ i ];
				return r;
			}
		}
		public static byte[] GetBytes( long v ) {
			unsafe {
				byte* valueDataPointer = (byte*) &v;
				byte[] r = new byte[ sizeof( long ) ];
				for( int i = 0; i < r.Length; i++ )
					r[ i ] = valueDataPointer[ i ];
				return r;
			}
		}
		public static byte[] GetBytes( ulong v ) {
			unsafe {
				byte* valueDataPointer = (byte*) &v;
				byte[] r = new byte[ sizeof( ulong ) ];
				for( int i = 0; i < r.Length; i++ )
					r[ i ] = valueDataPointer[ i ];
				return r;
			}
		}
		public static byte[] GetBytes( float v ) {
			unsafe {
				byte* valueDataPointer = (byte*) &v;
				byte[] r = new byte[ sizeof( float ) ];
				for( int i = 0; i < r.Length; i++ )
					r[ i ] = valueDataPointer[ i ];
				return r;
			}
		}
		public static byte[] GetBytes( double v ) {
			unsafe {
				byte* valueDataPointer = (byte*) &v;
				byte[] r = new byte[ sizeof( double ) ];
				for( int i = 0; i < r.Length; i++ )
					r[ i ] = valueDataPointer[ i ];
				return r;
			}
		}
		#endregion
		#region Value Arrays
		public static bool SetBytes( byte[] array, params float[] values ) {
			if( array.Length * 4 < values.Length )
				return false;
			unsafe {
				fixed( float* valuesPointer = values ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < array.Length; i++ )
						array[ i ] = toBytePointer[ i ];
				}
			}
			return true;
		}
		public static byte[] GetBytes( params float[] values ) {
			byte[] data = new byte[ values.Length * sizeof( float ) ];
			unsafe {
				fixed( float* valuesPointer = values ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < data.Length; i++ )
						data[ i ] = toBytePointer[ i ];
				}
			}
			return data;
		}
		public static byte[] GetBytes( IReadOnlyList<float> values ) {
			byte[] data = new byte[ values.Count * sizeof( float ) ];
			unsafe {
				fixed( float* valuesPointer = values.ToArray() ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < data.Length; i++ )
						data[ i ] = toBytePointer[ i ];
				}
			}
			return data;
		}
		public static byte[] GetBytes( params double[] values ) {
			byte[] data = new byte[ values.Length * sizeof( double ) ];
			unsafe {
				fixed( double* valuesPointer = values ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < data.Length; i++ )
						data[ i ] = toBytePointer[ i ];
				}
			}
			return data;
		}
		public static byte[] GetBytes( params short[] values ) {
			byte[] data = new byte[ values.Length * sizeof( short ) ];
			unsafe {
				fixed( short* valuesPointer = values ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < data.Length; i++ )
						data[ i ] = toBytePointer[ i ];
				}
			}
			return data;
		}
		public static byte[] GetBytes( params ushort[] values ) {
			byte[] data = new byte[ values.Length * sizeof( ushort ) ];
			unsafe {
				fixed( ushort* valuesPointer = values ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < data.Length; i++ )
						data[ i ] = toBytePointer[ i ];
				}
			}
			return data;
		}
		public static byte[] GetBytes( params int[] values ) {
			byte[] data = new byte[ values.Length * sizeof( int ) ];
			unsafe {
				fixed( int* valuesPointer = values ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < data.Length; i++ )
						data[ i ] = toBytePointer[ i ];
				}
			}
			return data;
		}
		public static byte[] GetBytes( IReadOnlyList<int> values ) {
			byte[] data = new byte[ values.Count * sizeof( int ) ];
			unsafe {
				fixed( int* valuesPointer = values.ToArray() ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < data.Length; i++ )
						data[ i ] = toBytePointer[ i ];
				}
			}
			return data;
		}
		public static byte[] GetBytes( params uint[] values ) {
			byte[] data = new byte[ values.Length * sizeof( uint ) ];
			unsafe {
				fixed( uint* valuesPointer = values ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < data.Length; i++ )
						data[ i ] = toBytePointer[ i ];
				}
			}
			return data;
		}
		public static byte[] GetBytes( params long[] values ) {
			byte[] data = new byte[ values.Length * sizeof( long ) ];
			unsafe {
				fixed( long* valuesPointer = values ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < data.Length; i++ )
						data[ i ] = toBytePointer[ i ];
				}
			}
			return data;
		}
		public static byte[] GetBytes( params ulong[] values ) {
			byte[] data = new byte[ values.Length * sizeof( ulong ) ];
			unsafe {
				fixed( ulong* valuesPointer = values ) {
					byte* toBytePointer = (byte*) valuesPointer;
					for( int i = 0; i < data.Length; i++ )
						data[ i ] = toBytePointer[ i ];
				}
			}
			return data;
		}
		#endregion
		#endregion
		#region ToValue
		#region Values
		#region Bytes
		public static char ToChar( byte[] arr, int index = 0 ) {
			unsafe {
				fixed( byte* valuesPointer = arr )
					return ( (char*) &valuesPointer[ index ] )[ 0 ];
			}
		}
		public static short ToInt16( byte[] arr, int index = 0 ) {
			unsafe {
				fixed( byte* valuesPointer = arr )
					return ( (short*) &valuesPointer[ index ] )[ 0 ];
			}
		}
		public static ushort ToUInt16( byte[] arr, int index = 0 ) {
			unsafe {
				fixed( byte* valuesPointer = arr )
					return ( (ushort*) &valuesPointer[ index ] )[ 0 ];
			}
		}
		public static int ToInt32( byte[] arr, int index = 0 ) {
			unsafe {
				fixed( byte* valuesPointer = arr )
					return ( (int*) &valuesPointer[ index ] )[ 0 ];
			}
		}
		public static uint ToUInt32( byte[] arr, int index = 0 ) {
			unsafe {
				fixed( byte* valuesPointer = arr )
					return ( (uint*) &valuesPointer[ index ] )[ 0 ];
			}
		}
		public static long ToInt64( byte[] arr, int index = 0 ) {
			unsafe {
				fixed( byte* valuesPointer = arr )
					return ( (long*) &valuesPointer[ index ] )[ 0 ];
			}
		}
		public static ulong ToUInt64( byte[] arr, int index = 0 ) {
			unsafe {
				fixed( byte* valuesPointer = arr )
					return ( (ulong*) &valuesPointer[ index ] )[ 0 ];
			}
		}
		public static float ToFloat32( byte[] arr, int index = 0 ) {
			unsafe {
				fixed( byte* valuesPointer = arr )
					return ( (float*) &valuesPointer[ index ] )[ 0 ];
			}
		}
		public static double ToFloat64( byte[] arr, int index = 0 ) {
			unsafe {
				fixed( byte* valuesPointer = arr )
					return ( (double*) &valuesPointer[ index ] )[ 0 ];
			}
		}
		#endregion
		#region Strings
		/// <summary>Treats the chars as 2 byte values</summary>
		public static char ToCharLiteral( string arr, int index = 0 ) {
			return arr[ index ];
		}
		/// <summary>Treats the chars as 2 byte values</summary>
		public static short ToInt16Literal( string arr, int index = 0 ) {
			return (short) arr[ index ];
		}
		/// <summary>Treats the chars as 2 byte values</summary>
		public static ushort ToUInt16Literal( string arr, int index = 0 ) {
			return arr[ index ];
		}
		/// <summary>Treats the chars as 2 byte values</summary>
		public static int ToInt32Literal( string arr, int index = 0 ) {
			unsafe {
				fixed( char* valuesPointer = arr ) {
					return ( (int*) valuesPointer )[ index ];
				}
			}
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static uint ToUInt32Literal( string arr, int index = 0 ) {
			unsafe {
				fixed( char* valuesPointer = arr ) {
					return ( (uint*) valuesPointer )[ index ];
				}
			}
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static long ToInt64Literal( string arr, int index = 0 ) {
			unsafe {
				fixed( char* valuesPointer = arr ) {
					return ( (long*) valuesPointer )[ index ];
				}
			}
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static ulong ToUInt64Literal( string arr, int index = 0 ) {
			unsafe {
				fixed( char* valuesPointer = arr ) {
					return ( (ulong*) valuesPointer )[ index ];
				}
			}
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static unsafe float ToFloat32Literal( string arr, int index = 0 ) {
			unsafe {
				fixed( char* valuesPointer = arr ) {
					return ( (float*) valuesPointer )[ index ];
				}
			}
		}
		/// <summary>Treats the chars are 2 byte values</summary>
		public static unsafe double ToFloat64Literal( string arr, int index = 0 ) {
			unsafe {
				fixed( char* valuesPointer = arr ) {
					return ( (double*) valuesPointer )[ index ];
				}
			}
		}
		#endregion
		#endregion
		#region Arrays
		public static float[] ToFloat32Array( byte[] data ) {
			float[] array = new float[ data.Length / sizeof( float ) ];
			unsafe {
				fixed( byte* valuePointer = data ) {
					float* valueDataPointer = (float*) valuePointer;
					for( int i = 0; i < array.Length; i++ )
						array[ i ] = valueDataPointer[ i ];
				}
			}
			return array;
		}
		public static double[] ToFloat64Array( byte[] data ) {
			double[] array = new double[ data.Length / sizeof( double ) ];
			unsafe {
				fixed( byte* valuePointer = data ) {
					double* valueDataPointer = (double*) valuePointer;
					for( int i = 0; i < array.Length; i++ )
						array[ i ] = valueDataPointer[ i ];
				}
			}
			return array;
		}
		public static short[] ToInt16Array( byte[] data ) {
			short[] array = new short[ data.Length / sizeof( short ) ];
			unsafe {
				fixed( byte* valuePointer = data ) {
					short* valueDataPointer = (short*) valuePointer;
					for( int i = 0; i < array.Length; i++ )
						array[ i ] = valueDataPointer[ i ];
				}
			}
			return array;
		}
		public static ushort[] ToUInt16Array( byte[] data ) {
			ushort[] array = new ushort[ data.Length / sizeof( ushort ) ];
			unsafe {
				fixed( byte* valuePointer = data ) {
					ushort* valueDataPointer = (ushort*) valuePointer;
					for( int i = 0; i < array.Length; i++ )
						array[ i ] = valueDataPointer[ i ];
				}
			}
			return array;
		}
		public static int[] ToInt32Array( byte[] data ) {
			int[] array = new int[ data.Length / sizeof( int ) ];
			unsafe {
				fixed( byte* valuePointer = data ) {
					int* valueDataPointer = (int*) valuePointer;
					for( int i = 0; i < array.Length; i++ )
						array[ i ] = valueDataPointer[ i ];
				}
			}
			return array;
		}
		public static uint[] ToUInt32Array( byte[] data ) {
			uint[] array = new uint[ data.Length / sizeof( uint ) ];
			unsafe {
				fixed( byte* valuePointer = data ) {
					uint* valueDataPointer = (uint*) valuePointer;
					for( int i = 0; i < array.Length; i++ )
						array[ i ] = valueDataPointer[ i ];
				}
			}
			return array;
		}
		public static long[] ToInt64Array( byte[] data ) {
			long[] array = new long[ data.Length / sizeof( long ) ];
			unsafe {
				fixed( byte* valuePointer = data ) {
					long* valueDataPointer = (long*) valuePointer;
					for( int i = 0; i < array.Length; i++ )
						array[ i ] = valueDataPointer[ i ];
				}
			}
			return array;
		}
		public static ulong[] ToUInt64Array( byte[] data ) {
			ulong[] array = new ulong[ data.Length / sizeof( ulong ) ];
			unsafe {
				fixed( byte* valuePointer = data ) {
					ulong* valueDataPointer = (ulong*) valuePointer;
					for( int i = 0; i < array.Length; i++ )
						array[ i ] = valueDataPointer[ i ];
				}
			}
			return array;
		}
		#endregion
		#region Strings
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytes">The data to read the string from.</param>
		/// <param name="lengthChars">The length searched, in chars. Chars use 2 bytes per char.</param>
		/// <param name="startByteIndex">The start index in bytes.</param>
		/// <returns></returns>
		public static string ToString( byte[] bytes, int lengthChars, int startByteIndex = 0 ) {
			if( lengthChars <= 0 ) {
				Logging.Warning( "Tried to get string from byte array with a length of 0 or lower" );
				return "";
			}
			if( startByteIndex < 0 ) {
				Logging.Warning( "Tried to get string from byte array string at an index lower than 0" );
				return "";
			}
			int byteIndex = startByteIndex;
			int endByteIndex = Math.Min( startByteIndex + lengthChars * sizeof( char ), bytes.Length );
			char[] c = new char[ ( endByteIndex - startByteIndex ) / sizeof( char ) ];
			unsafe {
				fixed( byte* valuePointer = bytes ) {
					for( int i = 0; byteIndex < endByteIndex; byteIndex += sizeof( char ), i++ ) {
						c[ i ] = ( (char*) &valuePointer[ byteIndex ] )[ 0 ];
					}
				}
			}
			return new string( c );
		}
		public static string ToString( IReadOnlyList<byte> b, int len, int startByteIndex = 0 ) {
			return ToString( b.ToArray(), len, startByteIndex );
		}
		public static string ToString( byte[] b ) {
			char[] c = new char[ b.Length / sizeof( char ) ];
			unsafe {
				fixed( byte* valuePointer = b.ToArray() ) {
					char* valueDataPointer = (char*) valuePointer;
					for( int i = 0; i < c.Length; i++ )
						c[ i ] = valueDataPointer[ i ];
				}
			}
			return new string( c );
		}
		public static string ToString( IReadOnlyList<byte> b ) {
			char[] c = new char[ b.Count / sizeof( char ) ];
			unsafe {
				fixed( byte* valuePointer = b.ToArray() ) {
					char* valueDataPointer = (char*) valuePointer;
					for( int i = 0; i < c.Length; i++ )
						c[ i ] = valueDataPointer[ i ];
				}
			}
			return new string( c );
		}
		#endregion
		#endregion

	}
}
