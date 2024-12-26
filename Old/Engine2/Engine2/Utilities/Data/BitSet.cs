using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Data {
	public class BitSet {

		private const int BITS_PER_WORD = sizeof( ulong ) * 8;
		private const int BITS = BITS_PER_WORD - 1;
		private ulong[] words;

		public int Size { get; private set; }

		/**
		 * <summary>Max + 1</summary>
		 */
		public int Count => Max + 1;
		/**
		 * <summary>Highest set bit.</summary>
		 */
		public int Max {
			get; private set;
		}
		/**
		 * <summary>Lowest clear bit.</summary>
		 */
		public int Min {
			get; private set;
		}

		public BitSet( int size ) {
			if( size < 0 )
				throw new ArgumentOutOfRangeException( $"size[{size}] cannot be lower than 0" );

			Size = size;

			Max = 0;
			Min = 0;

			words = new ulong[ (int) Math.Ceiling( (float) Size / BITS_PER_WORD ) ];

		}

		private BitSet( BitSet donor ) {
			Size = donor.Size;
			Max = donor.Max;
			Min = donor.Min;
			this.words = new ulong[ donor.words.Length ];

			for( int i = 0; i < words.Length; i++ )
				words[ i ] = donor.words[ i ];
		}

		private int WordIndex( int bitIndex ) {
			return bitIndex / BITS_PER_WORD;
		}

		private int BitInWord( int bitIndex ) {
			return bitIndex & BITS;
		}

		public bool this[ int i ] {
			get {
				return Get( i );
			}
			set {
				if( value )
					Set( i );
				else
					Clear( i );
			}
		}

		public bool Get( int index ) {
			return ( words[ WordIndex( index ) ] & ( 1ul << index & BITS ) ) != 0;
		}

		public void Set( int index ) {
			words[ WordIndex( index ) ] |= ( 1ul << BitInWord( index ) );
			Max = PreviousSet( Size - 1 );
			Min = NextClear( 0 );
		}

		public void Clear( int index ) {
			words[ WordIndex( index ) ] &= ~( 1ul << BitInWord( index ) );
			Max = PreviousSet( Size - 1 );
			Min = NextClear( 0 );
		}

		public void SetIgnore( int index ) {
			words[ WordIndex( index ) ] |= ( 1ul << BitInWord( index ) );
		}

		public void ClearIgnore( int index ) {
			words[ WordIndex( index ) ] &= ~( 1ul << BitInWord( index ) );
		}

		public void Clear() {
			for( int i = 0; i < words.Length; i++ )
				words[ i ] = 0;
			Max = 0;
			Min = 0;
		}

		/**
		 * <summary>Switches all bits. 1s becomes 0, and vica versa.</summary>
		 */
		public void Switch() {
			for( int i = 0; i < words.Length; i++ )
				words[ i ] = ~words[ i ];
			Max = PreviousSet( Size - 1 );
			Min = NextClear( 0 );
		}

		/**
		 * <summary>Sets all bits within the index range to true.</summary>
		 * <param name="from">Inclusive</param>
		 * <param name="to">Exclusive</param>
		 */
		public void SetAll( int from, int to ) {
			for( int i = from; i < to; i++ )
				SetIgnore( i );
			Max = PreviousSet( Size - 1 );
			Min = NextClear( 0 );
		}

		/**
		 * <summary>Sets all bits within the index range to false.</summary>
		 * <param name="from">Inclusive</param>
		 * <param name="to">Exclusive</param>
		 */
		public void ClearAll( int from, int to ) {
			for( int i = from; i < to; i++ )
				ClearIgnore( i );
			Max = PreviousSet( Size - 1 );
			Min = NextClear( 0 );
		}

		/**
		 * <summary>Sets all bits within the index range to true.</summary>
		 * <param name="from">Inclusive</param>
		 * <param name="to">Exclusive</param>
		 */
		public void SetAll( int to ) {
			for( int i = 0; i < to; i++ )
				SetIgnore( i );
			Max = PreviousSet( Size - 1 );
			Min = NextClear( 0 );
		}

		/**
		 * <summary>Sets all bits within the index range to false.</summary>
		 * <param name="from">Inclusive</param>
		 * <param name="to">Exclusive</param>
		 */
		public void ClearAll( int to ) {
			for( int i = 0; i < to; i++ )
				ClearIgnore( i );
			Max = PreviousSet( Size - 1 );
			Min = NextClear( 0 );
		}

		/**
		 * <summary>Sets all bits to true.</summary>
		 */
		public void SetAll() {
			SetAll( Size );
		}

		/**
		 * <summary>Sets all bits to false.</summary>
		 */
		public void ClearAll() {
			ClearAll( Size );
		}


		public int NextSet( int i ) {
			int wIndex = WordIndex( i );
			ulong word = words[ wIndex ] & ( ~0UL << i );

			while( true ) {
				if( word != 0 )
					return ( wIndex * BITS_PER_WORD ) + NumberOfTrailingZeros( word );
				if( ++wIndex >= words.Length )
					return -1;
				word = words[ wIndex ];
			}
		}

		public int PreviousSet( int i ) {
			int wIndex = WordIndex( i );
			ulong word = words[ wIndex ] & ( ~0UL >> -( i + 1 ) );

			while( true ) {
				if( word != 0 )
					return ( wIndex + 1 ) * BITS_PER_WORD - 1 - NumberOfLeadingZeros( word );
				if( --wIndex == -1 )
					return -1;
				word = words[ wIndex ];
			}
		}

		public int NextClear( int i ) {
			int wIndex = WordIndex( i );
			ulong word = ~words[ wIndex ] & ( ~0UL << i );

			while( true ) {
				if( word != 0 )
					return ( wIndex * BITS_PER_WORD ) + NumberOfTrailingZeros( word );
				if( ++wIndex >= words.Length )
					return -1;
				word = ~words[ wIndex ];
			}
		}

		public int PreviousClear( int i ) {
			int wIndex = WordIndex( i );
			ulong word = ~words[ wIndex ] & ( ~0UL >> -( i + 1 ) );

			while( true ) {
				if( word != 0 )
					return ( wIndex + 1 ) * BITS_PER_WORD - 1 - NumberOfLeadingZeros( word );
				if( --wIndex == -1 )
					return -1;
				word = ~words[ wIndex ];
			}
		}

		public int CountSet() {
			int n = 0;
			int i = -1;
			while( ( i = NextSet( i + 1 ) ) != -1 ) {//rework
				n++;
			}
			return n;
		}

		public void Resize( int newSize ) {
			if( newSize <= Size )
				return;
			lock( words ) {
				ulong[] prevWords = words;
				Size = newSize;
				words = new ulong[ (int) System.Math.Ceiling( (float) Size / BITS_PER_WORD ) ];
				for( int i = 0; i < prevWords.Length; i++ ) {
					words[ i ] = prevWords[ i ];
				}
				Max = PreviousSet( Size - 1 );
				Min = NextClear( 0 );
			}
		}

		public int BytesUsed() {
			return words.Length * sizeof( ulong );
		}

		public BitSet Clone() {
			return new BitSet( this );
		}

		public static int NumberOfTrailingZeros( ulong i ) {
			uint x, y;
			if( i == 0 )
				return 64;
			int n = 63;
			y = (uint) i;
			if( y != 0 ) {
				n = n - 32;
				x = y;
			} else
				x = (uint) ( i >> 32 );
			y = x << 16;
			if( y != 0 ) {
				n = n - 16;
				x = y;
			}
			y = x << 8;
			if( y != 0 ) {
				n = n - 8;
				x = y;
			}
			y = x << 4;
			if( y != 0 ) {
				n = n - 4;
				x = y;
			}
			y = x << 2;
			if( y != 0 ) {
				n = n - 2;
				x = y;
			}
			return (int) ( n - ( ( x << 1 ) >> 31 ) );
		}

		public static int NumberOfLeadingZeros( ulong i ) {
			if( i == 0 )
				return 64;
			int n = 1;
			uint x = (uint) ( i >> 32 );
			if( x == 0 ) {
				n += 32;
				x = (uint) i;
			}
			if( x >> 16 == 0 ) {
				n += 16;
				x <<= 16;
			}
			if( x >> 24 == 0 ) {
				n += 8;
				x <<= 8;
			}
			if( x >> 28 == 0 ) {
				n += 4;
				x <<= 4;
			}
			if( x >> 30 == 0 ) {
				n += 2;
				x <<= 2;
			}
			n -= (int) ( x >> 31 );
			return n;
		}

	}
}
