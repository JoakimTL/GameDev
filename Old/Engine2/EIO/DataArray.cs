using System;
using System.Collections.Generic;
using System.Text;

namespace ED {
	public class DataArray<T> {
		public IReadOnlyList<T> Data => data;
		public uint Size => (uint) data.Length;

		private T[] data;
		private BitSet dataBitset;
		private List<DataSegment> segments;
		public delegate void DataChangeHandler( DataChangeSegment seg);
		public event DataChangeHandler DataChange;
		public event Action DataReset;
		public event Action Resized;

		public DataArray( int size ) {
			data = new T[ size ];
			dataBitset = new BitSet( size );
			segments = new List<DataSegment>();
		}

		public DataSegment CreateSegment( int size ) {
			Console.WriteLine( dataBitset.Min + ", " + dataBitset.Max );
			if( dataBitset.Min < dataBitset.Max )
				Defragment();
			int numClear = dataBitset.Size - dataBitset.Min;
			if( numClear < size || dataBitset.Min == -1 )
				Expand( data.Length << 1 );
			DataSegment seg = new DataSegment( this, size, dataBitset.Min );
			dataBitset.SetAll( dataBitset.Min, dataBitset.Min + size );
			segments.Add( seg );

			return seg;
		}

		internal T this[ int index ] {
			get => data[ index ];
			set { lock( data ) { data[ index ] = value; DataChange?.Invoke( new DataChangeSegment( index, 1 ) ); } }
		}

		internal void SetRange( int offset, T[] data ) {
			int end = offset + data.Length;
			if( offset < 0 || end >= this.data.Length )
				return;
			lock( data ) {
				for( int i = offset; i < end; i++ )
					this.data[ i ] = data[ i ];
				DataChange?.Invoke( new DataChangeSegment( offset, data.Length ) );
			}
		}

		public void Defragment() {
			int index = 0;
			T[] newArray = new T[ data.Length ];
			lock( data ) {
				for( int i = 0; i < segments.Count; i++ ) {
					int startIndex = index;
					dataBitset.SetAll( index, index + segments[ i ].Length );
					for( int j = 0; j < segments[ i ].Length; j++ ) {
						newArray[ index++ ] = segments[ i ][ j ];
					}
					segments[ i ].SetOffset( startIndex );
				}
				DataReset?.Invoke();
				DataChange?.Invoke( new DataChangeSegment( 0, index ) );
				dataBitset.ClearAll( index, dataBitset.Size );
				data = newArray;
			}
		}

		public IntPtr GetData( DataArray<byte>.DataChangeSegment seg ) {

		}

		private void Expand( int newSize ) {
			lock( data ) {
				T[] oldArray = data;
				data = new T[ newSize ];
				Array.Copy( oldArray, 0, data, 0, oldArray.Length );
				dataBitset.Resize( newSize );
				Resized?.Invoke();
			}
		}

		private void SegmentDisposed( DataSegment s ) {
			dataBitset.ClearAll( s.Offset, s.Offset + s.Length );
			segments.Remove( s );
		}

		public override string ToString() {
			string @out = "[";
			for( int i = 0; i < data.Length; i++ ) {
				@out += data[ i ].ToString();
				if( i < data.Length - 1 )
					@out += " ";
			}
			@out += "]";
			return @out;
		}

		public int GetBytesUsed( int bytesPerElement ) {
			return data.Length * bytesPerElement + dataBitset.BytesUsed();
		}

		public class DataSegment {

			private DataArray<T> array;
			private bool disposed, @readonly;
			public int Offset { get; private set; }
			public readonly int Length;

			public DataSegment( DataArray<T> array, int length, int offset ) {
				this.array = array;
				Length = length;
				Offset = offset;
				disposed = false;
				@readonly = false;
				for( int i = 0; i < length; i++ ) {
					array[ i + Offset ] = default;
				}
			}

			internal void SetOffset( int off ) {
				Offset = off;
			}

			public void Lock() {
				@readonly = true;
			}

			public T this[ int index ] {
				get => array[ index + Offset ];
				set {
					if( disposed || @readonly )
						return;
					int ind = Offset + index;
					if( ind < Offset || ind >= Offset + Length )
						throw new ArgumentOutOfRangeException($"Index {index} is out of range.");
					array[ ind ] = value;
				}
			}
			public void SetRange( int offset, T[] data ) {
				int ind = Offset + offset;
				if( ind < Offset || ind >= Offset + Length )
					throw new ArgumentOutOfRangeException( $"Offset {offset} is out of range." );
				if( data.Length > Length || data.Length + ind > Offset + Length )
					throw new ArgumentOutOfRangeException( $"Data length {data.Length} is out of range." );
				array.SetRange( ind, data );
			}

			public void Dispose() {
				disposed = true;
				array.SegmentDisposed( this );
			}

			public override string ToString() {
				string @out = $"[{Offset}][";
				for( int i = 0; i < Length; i++ ) {
					@out += this[ i ].ToString();
					if( i < Length - 1 )
						@out += " ";
				}
				@out += $"][{( disposed ? "DISPOSED" : "VALID" )}]";
				return @out;
			}

		}

		public struct DataChangeSegment {
			public readonly int Offset;
			public readonly int Length;

			public DataChangeSegment( int offset, int length ) {
				Length = length;
				Offset = offset;
			}
		}
	}
}
