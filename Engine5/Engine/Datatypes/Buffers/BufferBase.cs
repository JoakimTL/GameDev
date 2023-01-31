using Engine.Rendering.Objects;
using Engine.Structure.Interfaces.Buffers;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Engine.Datatypes.Buffers;
public abstract unsafe class BufferBase : Identifiable, IBuffer, IReadableBuffer, IReadableBufferIndexable, IListenableWriteableBuffer, IListenableResizeableBuffer {
	public ulong SizeBytes { get; protected set; }
	private void* _bufferPointer;
	protected readonly AutoResetEvent? _multithreadingLock;

	protected void* Pointer => _bufferPointer;

	public event IListenableWriteableBuffer.BufferWrittenEvent? Written;
	public event IListenableResizeableBuffer.BufferResizeEvent? Resized;

	/// <param name="initialSizeBytes">The initial size of the buffer in bytes</param>
	/// <param name="safeguardMultiThreading">If true the buffer will take precautions and undergo locking for each operation to ensure only one thread has access at a time. This only affects writing and modifications to the buffer.</param>
	protected BufferBase( string name, ulong initialSizeBytes, bool safeguardMultiThreading ) : base( name ) {
		SizeBytes = initialSizeBytes;
		this._multithreadingLock = safeguardMultiThreading ? new( true ) : null;
		_bufferPointer = NativeMemory.Alloc( (nuint) SizeBytes.ToUlong() );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	protected unsafe bool Write( ulong offsetBytes, void* data, ulong sizeBytes, int timeout, bool performLocking ) {
		if ( performLocking )
			if ( !Lock( timeout ) ) {
				this.LogWarning( "Buffer write timed out." );
				return false;
			}
		if ( offsetBytes + sizeBytes > SizeBytes ) {
			this.LogWarning( "Tried to write data outside buffer." );
			return false;
		}
		Buffer.MemoryCopy( data, (byte*) _bufferPointer + offsetBytes.ToUlong(), ( SizeBytes - offsetBytes ).ToUlong(), sizeBytes.ToUlong() );
		Written?.Invoke( offsetBytes, sizeBytes );
		if ( performLocking )
			Unlock();
		return true;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	protected bool Move( ulong srcOffset, ulong dstOffset, ulong length, int timeout, bool performLocking )
		=> Write( dstOffset, (byte*) _bufferPointer + srcOffset.ToUlong(), length, timeout, performLocking );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	protected bool Resize( ulong newSizeBytes, int timeout, bool performLocking ) {
		if ( performLocking )
			if ( !Lock( timeout ) ) {
				this.LogWarning( "Buffer write timed out." );
				return false;
			}
		NativeMemory.Realloc( _bufferPointer, (nuint) newSizeBytes.ToUlong() );
		Resized?.Invoke( newSizeBytes );
		if ( performLocking )
			Unlock();
		return true;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	protected bool Lock( int timeout ) {
		if ( _multithreadingLock is not null )
			return _multithreadingLock.WaitOne( timeout );
		return true;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	protected void Unlock() {
		if ( _multithreadingLock is not null )
			_multithreadingLock.Set();
	}

	public TData ReadOne<TData>( ulong offsetBytes ) where TData : unmanaged {
		ulong offset = offsetBytes.ToUlong();
		uint length = (uint) sizeof( TData );
		if ( offset + length > SizeBytes.ToUlong() ) {
			this.LogWarning( "Tried to access data outside buffer." );
			return default;
		}
		return ( (TData*) ( ( (byte*) _bufferPointer ) + offsetBytes.ToUint() ) )[ 0 ];
	}

	public IIndexableReadOnlyBufferSegment<T> Read<T>( ulong offsetBytes, ulong lengthElements ) where T : unmanaged {
		ulong offset = offsetBytes.ToUlong();
		ulong length = lengthElements.ToUlong() * (uint) sizeof( T );
		if ( offset + length > SizeBytes.ToUlong() ) {
			this.LogWarning( "Tried to access data outside buffer." );
			return IndexableReadOnlyBufferSegment<T>.Empty;
		}
		return new IndexableReadOnlyBufferSegment<T>( this, length, offsetBytes );
	}

	public T[] Snapshot<T>( ulong offsetBytes, ulong lengthElements ) where T : unmanaged {
		ulong offset = offsetBytes.ToUlong();
		ulong length = lengthElements.ToUlong() * (uint) sizeof( T );
		if ( offset + length > SizeBytes.ToUlong() ) {
			this.LogWarning( "Tried to access data outside buffer." );
			return Array.Empty<T>();
		}
		return new ReadOnlySpan<T>( ( (byte*) _bufferPointer ) + offset, (int) length ).ToArray();
	}

}
