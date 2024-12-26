using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Utilities.Data {
	public class DataArray<T> {
		public IReadOnlyList<T> DataReadonly => Data;
		public uint Size => (uint) Data.Length;

		internal T[] Data { get; private set; }
		private BitSet dataBitset;
		private List<DataSegment> segments;
		public delegate void DataChangeHandler( DataChangeSegment seg );
		public bool Resize { get; set; }
		public event DataChangeHandler DataChange;
		public event Action DataReset;
		public event Action Resized;

		public DataArray( int size, bool resize = true ) {
			Resize = resize;
			Data = new T[ size ];
			dataBitset = new BitSet( size );
			segments = new List<DataSegment>();
		}

		public DataSegment CreateSegment( int size ) {
			if( dataBitset.Min < dataBitset.Max )
				Defragment();
			int numClear = dataBitset.Size - dataBitset.Min;
			if( numClear < size || dataBitset.Min == -1 )
				if( Resize ) {
					Expand( Data.Length << 1 );
				} else
					return null;
			DataSegment seg = new DataSegment( this, size, dataBitset.Min );
			dataBitset.SetAll( dataBitset.Min, dataBitset.Min + size );
			segments.Add( seg );

			return seg;
		}

		internal T this[ int index ] {
			get => Data[ index ];
			set { lock( Data ) { Data[ index ] = value; DataChange?.Invoke( new DataChangeSegment( index, 1 ) ); } }
		}

		internal void SetRange( int offset, T[] data ) {
			int end = offset + data.Length;
			if( offset < 0 || end >= this.Data.Length )
				return;
			lock( data ) {
				for( int i = offset; i < end; i++ )
					this.Data[ i ] = data[ i - offset ];
				DataChange?.Invoke( new DataChangeSegment( offset, data.Length ) );
			}
		}

		public void Defragment() {
			int index = 0;
			int newFrom = 0;
			T[] newArray = new T[ Data.Length ];
			lock( Data ) {
				lock( segments ) {
					for( int i = 0; i < segments.Count; i++ ) {
						int startIndex = index;
						int segOffset = segments[ i ].Offset;
						dataBitset.SetAll( index, index + segments[ i ].Length );
						for( int j = 0; j < segments[ i ].Length; j++ ) {
							newArray[ index++ ] = segments[ i ][ j ];
						}
						segments[ i ].SetOffset( startIndex );
						if( segOffset != startIndex && newFrom == 0 )
							newFrom = startIndex;

					}
				}
				DataReset?.Invoke();
				DataChange?.Invoke( new DataChangeSegment( newFrom, index ) );
				dataBitset.ClearAll( index, dataBitset.Size );
				Data = newArray;
			}
		}

		private void Expand( int newSize ) {
			lock( Data ) {
				T[] oldArray = Data;
				Data = new T[ newSize ];
				Array.Copy( oldArray, 0, Data, 0, oldArray.Length );
				dataBitset.Resize( newSize );
				Resized?.Invoke();
			}
		}

		private void Default( int offset, int length ) {
			for( int i = offset; i < offset + length; i++ ) {
				Data[ i ] = default;
			}
		}

		private void SegmentDisposed( DataSegment s ) {
			lock( segments ) {
				dataBitset.ClearAll( s.Offset, s.Offset + s.Length );
				segments.Remove( s );
			}
		}

		public override string ToString() {
			string @out = "[";
			for( int i = 0; i < Data.Length; i++ ) {
				@out += Data[ i ].ToString();
				if( i < Data.Length - 1 )
					@out += " ";
			}
			@out += "]";
			return @out;
		}

		public int GetBytesUsed( int bytesPerElement ) {
			return Data.Length * bytesPerElement + dataBitset.BytesUsed();
		}

		public class DataSegment {

			private DataArray<T> array;
			private bool disposed, @readonly;
			public int Offset { get; private set; }
			public readonly int Length;

			public event Action OffsetChanged;

			internal DataSegment( DataArray<T> array, int length, int offset ) {
				this.array = array;
				Length = length;
				Offset = offset;
				disposed = false;
				@readonly = false;
				array.Default( Offset, Length );
			}

			internal void SetOffset( int off ) {
				if( off == Offset )
					return;
				Offset = off;
				OffsetChanged?.Invoke();
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
						throw new ArgumentOutOfRangeException( $"Index {index} is out of range." );
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

			public SubSegment CreateSubSegment( int offset ) {
				if( offset < 0 || offset >= Length )
					return null;
				return new SubSegment( this, offset );
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

			public class SubSegment {
				public readonly DataSegment Segment;
				public readonly int Offset;

				internal SubSegment( DataSegment segment, int offset ) {
					Segment = segment;
					Offset = offset;
				}


				public T this[ int index ] {
					get => Segment[ index + Offset ];
					set {
						Segment[ Offset + index ] = value;
					}
				}

				public void SetRange( int offset, T[] data ) {
					Segment.SetRange( Offset + offset, data );
				}
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
