using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Data.Buffers;
public abstract unsafe class DataBuffer : DisposableIdentifiable {

	public delegate void SectionUpdateHandler( in SectionUpdate update );
	public ulong SizeBytes { get; private set; }
	protected byte* _allocPtr;

	private int _readWriters;
	private readonly ManualResetEvent _readWriteWait;
	private readonly ManualResetEvent _resizeWait;
	private readonly AutoResetEvent _resizeInternalWait;

	protected override string UniqueNameTag => $"{this.SizeBytes / 1024d:N3}KiB/{(ulong) this._allocPtr:X2}*";

	public event Action<ulong>? Resized;
	public event SectionUpdateHandler? SegmentUpdated;

	public DataBuffer( string name, ulong sizeBytes ) : base( name ) {
		this.SizeBytes = sizeBytes;
		this._allocPtr = (byte*) NativeMemory.AllocZeroed( (nuint) this.SizeBytes );
		this._readWriters = 0;
		this._readWriteWait = new ManualResetEvent( true );
		this._resizeWait = new ManualResetEvent( true );
		this._resizeInternalWait = new AutoResetEvent( true );
	}

	public void UpdateVBO( Rendering.VertexBufferObject vbo, DataBufferChangeRegistry changes, ref bool[]? changeSet ) {
		DataBufferChangeRegistry.UpdateState state = changes.ConsumeUpdates( ref changeSet );
		if ( state == DataBufferChangeRegistry.UpdateState.DISPOSED ) {
			this.LogError( "Disposed!" );
			return;
		}
		if ( vbo.BufferId == 0 || state == DataBufferChangeRegistry.UpdateState.NONE ) {
			return;
		}
		StartReadWrite();
		if ( state == DataBufferChangeRegistry.UpdateState.RESIZED ) {
			if ( this.SizeBytes > uint.MaxValue )
				this.LogWarning( "Size of buffer exceeds maximum size of VBO." );
			this.LogLine( $"Updating {vbo} with resize!", Log.Level.LOW );
			vbo.DirectResizeWrite( new IntPtr( this._allocPtr ), (uint) this.SizeBytes );
			EndReadWrite();
			return;
		}
		if ( changeSet is null ) {
			this.LogError( "Changes are not set." );
			EndReadWrite();
			return;
		}
		this.LogLine( $"Writing to {vbo}!", Log.Level.VERBOSE );
		uint startByte = 0;
		bool tallying = false;
		for ( uint i = 0; i < changeSet.Length; i++ ) {
			if ( changeSet[ i ] ) {
				if ( !tallying ) {
					tallying = true;
					startByte = i * changes.SectionSizeBytes;
				}
			} else {
				if ( tallying ) {
					tallying = false;
					this.LogLine( $"Wrote {( i * changes.SectionSizeBytes ) - startByte}B to {vbo}!", Log.Level.LOW );
					vbo.DirectWrite( new IntPtr( this._allocPtr ), startByte, startByte, ( i * changes.SectionSizeBytes ) - startByte );
				}
			}
			changeSet[ i ] = false;
		}
		if ( tallying ) {
			this.LogLine( $"Wrote {vbo.SizeBytes - startByte}B to {vbo}!", Log.Level.LOW );
			vbo.DirectWrite( new IntPtr( this._allocPtr ), startByte, startByte, vbo.SizeBytes - startByte );
		}
		EndReadWrite();
	}

	protected void Move( ulong srcOffset, ulong dstOffset, uint length ) {
		unsafe {
			StartReadWrite();
			Unsafe.CopyBlockUnaligned( this._allocPtr + dstOffset, this._allocPtr + srcOffset, length );
			EndReadWrite();
			SegmentUpdated?.Invoke( new SectionUpdate( dstOffset, length ) );
		}
	}

	protected unsafe void WriteUnrestrictedUnalignedSynchronized<T>( T data, ulong dstOffsetBytes ) where T : unmanaged {
#if DEBUG
		if ( this.Disposed )
			this.LogWarning( "Undefined behaviour, setting data in disposed databuffer!" );
#endif
		StartReadWrite();
		( (T*) ( this._allocPtr + dstOffsetBytes ) )[ 0 ] = data;
		EndReadWrite();
		SegmentUpdated?.Invoke( new SectionUpdate( dstOffsetBytes, (ulong) sizeof( T ) ) );
	}

	protected unsafe void WriteUnrestrictedUnalignedSynchronized( void* data, uint numBytes, ulong dstOffsetBytes ) {
#if DEBUG
		if ( this.Disposed )
			this.LogWarning( "Undefined behaviour, setting data in disposed databuffer!" );
#endif
		StartReadWrite();
		Unsafe.CopyBlockUnaligned( this._allocPtr + dstOffsetBytes, data, numBytes );
		EndReadWrite();
		SegmentUpdated?.Invoke( new SectionUpdate( dstOffsetBytes, numBytes ) );
	}

	protected unsafe T ReadUnrestrictedUnalignedSynchronized<T>( ulong offsetBytes ) where T : unmanaged {
#if DEBUG
		if ( this.Disposed )
			this.LogWarning( "Undefined behaviour, reading from disposed databuffer!" );
#endif
		StartReadWrite();
		T ret = ( (T*) ( this._allocPtr + offsetBytes ) )[ 0 ];
		EndReadWrite();
		return ret;
	}

	protected unsafe void ReadUnrestrictedUnalignedSynchronized<T>( T[] dataContainer, uint sizeBytes, ulong offsetBytes ) where T : unmanaged {
#if DEBUG
		if ( this.Disposed )
			this.LogWarning( "Undefined behaviour, reading from disposed databuffer!" );
#endif
		StartReadWrite();
		fixed ( T* dstPtr = dataContainer )
			Unsafe.CopyBlockUnaligned( dstPtr, this._allocPtr + offsetBytes, sizeBytes );
		EndReadWrite();
	}

	protected void ResizeSynchronized( ulong expansion = 0 ) {
		if ( expansion == 0 )
			expansion = this.SizeBytes;
		this._resizeWait.WaitOne();
		this._readWriteWait.Reset();
		this._resizeInternalWait.WaitOne();
		this.SizeBytes += expansion;
		this._allocPtr = (byte*) NativeMemory.Realloc( this._allocPtr, (nuint) this.SizeBytes );
		this.LogLine( $"Resized to {this.SizeBytes}B!", Log.Level.NORMAL );
		this._resizeInternalWait.Set();
		this._readWriteWait.Set();
		Resized?.Invoke( this.SizeBytes );
	}

	protected void StartReadWrite() {
		this._resizeWait.Reset();
		this._readWriteWait.WaitOne();
		Interlocked.Increment( ref this._readWriters );
	}
	protected void EndReadWrite() {
		Interlocked.Decrement( ref this._readWriters );
		if ( this._readWriters == 0 )
			this._resizeWait.Set();
	}

	protected override bool OnDispose() {
		NativeMemory.Free( this._allocPtr );
		return true;
	}

	public struct SectionUpdate {
		public readonly ulong Offset;
		public readonly ulong Size;

		public SectionUpdate( ulong offset, ulong size ) {
			this.Offset = offset;
			this.Size = size;
		}
	}
}
