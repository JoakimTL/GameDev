namespace Engine.Data.Buffers;
public class SegmentedDataBuffer : DataBuffer {

	public readonly uint ByteAlignment;
	private bool _fragmented;
	private ulong _allocatedBytes;
	private readonly ulong _expansion;
	private readonly List<Segment> _segments;
	private readonly AutoResetEvent _allocation;

	public SegmentedDataBuffer( string name, ulong sizeBytes, uint segmentByteAlignment = sizeof( byte ), ulong expansion = 0 ) : base( name, sizeBytes ) {
		if ( segmentByteAlignment == 0 )
			throw new ArgumentOutOfRangeException( nameof( segmentByteAlignment ), "Must be greater than zero!" );
		this._allocatedBytes = 0;
		this._expansion = expansion;
		this._segments = new List<Segment>();
		this.ByteAlignment = segmentByteAlignment;
		this._allocation = new AutoResetEvent( true );
	}

	public IDataSegment AllocateSynchronized( uint sizeBytes ) {
		if ( sizeBytes % this.ByteAlignment != 0 ) {
			uint newSizeBytes = ( ( sizeBytes / this.ByteAlignment ) + 1 ) * this.ByteAlignment;
			this.LogWarning( $"Attempted to allocate segment outside alignment. Adjusting from {sizeBytes}B to {newSizeBytes}B!" );
			sizeBytes = newSizeBytes;
		}
		this._allocation.WaitOne();
		while ( this._allocatedBytes + sizeBytes > this.SizeBytes ) {
			if ( this._fragmented ) {
				DefragmentSynchronized();
			} else
				ResizeSynchronized( this._expansion );
		}

		Segment segment = new( this, this._allocatedBytes, sizeBytes );
		this.LogLine( $"Allocated {sizeBytes}B", Log.Level.VERBOSE );
		this._segments.Add( segment );
		this._allocatedBytes += segment.SizeBytes;
		this._allocation.Set();
		return segment;
	}

	private void DefragmentSynchronized() {
		this._allocatedBytes = 0;
		StartReadWrite();
		for ( int i = 0; i < this._segments.Count; i++ ) {
			Segment segment = this._segments[ i ];
			if ( segment.OffsetBytes != this._allocatedBytes ) {
				Move( segment.OffsetBytes, this._allocatedBytes, segment.SizeBytes );
				segment.SetOffset( this._allocatedBytes );
			}
			this._allocatedBytes += segment.SizeBytes;
		}
		EndReadWrite();
		this._fragmented = false;
		this.LogLine( $"Defragmented!", Log.Level.NORMAL );
	}

	private void Free( Segment segment ) {
		this._segments.Remove( segment );
		this.LogLine( $"Segment {segment} removed!", Log.Level.LOW );
		this._fragmented = true;
	}

	public class Segment : DisposableIdentifiable, IDataSegment {

		private readonly SegmentedDataBuffer _dataBuffer;
		public ulong OffsetBytes { get; private set; }
		public uint SizeBytes { get; private set; }
		public event Action<ulong>? OffsetChanged;

		protected override string UniqueNameTag => $"{this.OffsetBytes}->{this.SizeBytes / 1024d}KiB";

		internal Segment( SegmentedDataBuffer dataBuffer, ulong offsetBytes, uint sizeBytes ) {
			this._dataBuffer = dataBuffer;
			this.SizeBytes = sizeBytes;
			this.OffsetBytes = offsetBytes;
		}

		internal void SetOffset( ulong offset ) {
			this.OffsetBytes = offset;
			OffsetChanged?.Invoke( offset );
		}

		public T Read<T>( ulong offsetBytes ) where T : unmanaged {
			unsafe {
				if ( offsetBytes + (uint) sizeof( T ) > this.SizeBytes ) {
					this.LogWarning( $"{nameof( Read )}{typeof( T ).Name}: Attempted to access area outside segment!" );
					return default;
				}
				return this._dataBuffer.ReadUnrestrictedUnalignedSynchronized<T>( this.OffsetBytes + offsetBytes );
			}
		}

		/// <summary>
		/// <b>CAUSES MEMORY ALLOCATION</b>
		/// </summary>
		public Memory<T> Read<T>( ulong offsetBytes, uint elementCount ) where T : unmanaged {
			unsafe {
				if ( offsetBytes + (uint) ( elementCount * sizeof( T ) ) > this.SizeBytes ) {
					this.LogWarning( $"{nameof( Read )}{typeof( T ).Name}: Attempted to access area outside segment!" );
					return default;
				}
				T[] returnData = new T[ elementCount ];
				this._dataBuffer.ReadUnrestrictedUnalignedSynchronized( returnData, (uint) ( elementCount * sizeof( T ) ), this.OffsetBytes + offsetBytes );
				return new Memory<T>( returnData );
			}
		}

		public void Write<T>( ulong offsetBytes, T data ) where T : unmanaged {
			unsafe {
				if ( offsetBytes + (uint) sizeof( T ) > this.SizeBytes ) {
					this.LogWarning( $"{nameof( Write )}{typeof( T ).Name}: Attempted to access area outside segment!" );
					return;
				}
				this._dataBuffer.WriteUnrestrictedUnalignedSynchronized( data, this.OffsetBytes + offsetBytes );
			}
		}

		public unsafe void Write<T>( ulong offsetBytes, T* dataPtr, uint elementCount ) where T : unmanaged {
			unsafe {
				if ( offsetBytes + (uint) ( elementCount * sizeof( T ) ) > this.SizeBytes ) {
					this.LogWarning( $"{nameof( Write )}{typeof( T ).Name}: Attempted to access area outside segment!" );
					return;
				}
				this._dataBuffer.WriteUnrestrictedUnalignedSynchronized( (byte*) dataPtr, (uint) ( elementCount * sizeof( T ) ), this.OffsetBytes + offsetBytes );
			}
		}

		public void Write<T>( ulong offsetBytes, Span<T> data ) where T : unmanaged {
			unsafe {
				if ( offsetBytes + (uint) ( data.Length * sizeof( T ) ) > this.SizeBytes ) {
					this.LogWarning( $"{nameof( Write )}{typeof( T ).Name}: Attempted to access area outside segment!" );
					return;
				}
				fixed ( T* dataPtr = data ) {
					this._dataBuffer.WriteUnrestrictedUnalignedSynchronized( (byte*) dataPtr, (uint) ( data.Length * sizeof( T ) ), this.OffsetBytes + offsetBytes );
				}
			}
		}

		public void Write<T>( ulong offsetBytes, ReadOnlySpan<T> data ) where T : unmanaged {
			unsafe {
				if ( offsetBytes + (uint) ( data.Length * sizeof( T ) ) > this.SizeBytes ) {
					this.LogWarning( $"{nameof( Write )}{typeof( T ).Name}: Attempted to access area outside segment!" );
					return;
				}
				fixed ( T* dataPtr = data ) {
					this._dataBuffer.WriteUnrestrictedUnalignedSynchronized( (byte*) dataPtr, (uint) ( data.Length * sizeof( T ) ), this.OffsetBytes + offsetBytes );
				}
			}
		}

		public void Write<T>( ulong offsetBytes, Memory<T> data ) where T : unmanaged {
			unsafe {
				if ( offsetBytes + (uint) ( data.Length * sizeof( T ) ) > this.SizeBytes ) {
					this.LogWarning( $"{nameof( Write )}{typeof( T ).Name}: Attempted to access area outside segment!" );
					return;
				}
				using ( System.Buffers.MemoryHandle pin = data.Pin() ) {
					T* dataPtr = (T*) pin.Pointer;
					this._dataBuffer.WriteUnrestrictedUnalignedSynchronized( (byte*) dataPtr, (uint) ( data.Length * sizeof( T ) ), this.OffsetBytes + offsetBytes );
				}
			}
		}

		public void Write<T>( ulong offsetBytes, ReadOnlyMemory<T> data ) where T : unmanaged {
			unsafe {
				if ( offsetBytes + (uint) ( data.Length * sizeof( T ) ) > this.SizeBytes ) {
					this.LogWarning( $"{nameof( Write )}{typeof( T ).Name}: Attempted to access area outside segment!" );
					return;
				}
				using ( System.Buffers.MemoryHandle pin = data.Pin() ) {
					T* dataPtr = (T*) pin.Pointer;
					this._dataBuffer.WriteUnrestrictedUnalignedSynchronized( (byte*) dataPtr, (uint) ( data.Length * sizeof( T ) ), this.OffsetBytes + offsetBytes );
				}
			}
		}

		protected override bool OnDispose() {
			this._dataBuffer.Free( this );
			this.SizeBytes = 0;
			return true;
		}
	}
}
