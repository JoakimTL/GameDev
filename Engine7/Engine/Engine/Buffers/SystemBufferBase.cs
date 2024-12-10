using Engine.Logging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Buffers;

/// <summary>
/// Implements all the basic functionality of a buffer as protected methods.
/// </summary>
public abstract unsafe class SystemBufferBase<TScalar>( TScalar initialLengthBytes ) : DisposableIdentifiable, IBuffer<TScalar>, IObservableBuffer<TScalar>, IVariableLengthBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	private byte* _dataPointer = (byte*) NativeMemory.Alloc( nuint.CreateSaturating( initialLengthBytes ) );
	public TScalar LengthBytes { get; private set; } = initialLengthBytes;
	public event Action<IBuffer<TScalar>>? OnBufferResized;
	public event BufferDataChanged<TScalar>? OnBufferWrittenTo;

	protected bool ReadRange<T>( Span<T> destination, TScalar sourceOffsetBytes ) where T : unmanaged {
		ObjectDisposedException.ThrowIf( Disposed, this );
		if (destination.Length == 0)
			return true;
		TScalar bytesToCopy = TScalar.CreateSaturating( (ulong) destination.Length * (uint) sizeof( T ) );
		if (sourceOffsetBytes + bytesToCopy > this.LengthBytes)
			return false;
		fixed (T* dstPtr = destination)
			DataUtilities.PerformMemCopy( this._dataPointer, (byte*) dstPtr, sourceOffsetBytes, 0, bytesToCopy );
		return true;
	}

	protected bool ReadRange( void* dstPtr, TScalar dstLengthBytes, TScalar sourceOffsetBytes ) {
		ObjectDisposedException.ThrowIf( Disposed, this );
		if (dstLengthBytes == TScalar.Zero)
			return true;
		if (sourceOffsetBytes + dstLengthBytes > this.LengthBytes)
			return false;
		DataUtilities.PerformMemCopy( this._dataPointer, (byte*) dstPtr, sourceOffsetBytes, 0, dstLengthBytes );
		return true;
	}

	protected bool WriteRange<T>( Span<T> source, TScalar destinationOffsetBytes ) where T : unmanaged {
		ObjectDisposedException.ThrowIf( Disposed, this );
		if (source.Length == 0)
			return true;
		TScalar bytesToCopy = TScalar.CreateSaturating( (ulong) source.Length * (uint) sizeof( T ) );
		if (destinationOffsetBytes + bytesToCopy > this.LengthBytes)
			return false;
		fixed (T* srcPtr = source)
			DataUtilities.PerformMemCopy( (byte*) srcPtr, this._dataPointer, 0, destinationOffsetBytes, bytesToCopy );
		OnBufferWrittenTo?.Invoke( destinationOffsetBytes, bytesToCopy );
		return true;
	}

	protected bool WriteRange( void* srcPtr, TScalar srcLengthBytes, TScalar destinationOffsetBytes ) {
		ObjectDisposedException.ThrowIf( Disposed, this );
		if (srcLengthBytes == TScalar.Zero)
			return true;
		if (destinationOffsetBytes + srcLengthBytes > this.LengthBytes)
			return false;
		DataUtilities.PerformMemCopy( (byte*) srcPtr, this._dataPointer, 0, destinationOffsetBytes, srcLengthBytes );
		OnBufferWrittenTo?.Invoke( destinationOffsetBytes, srcLengthBytes );
		return true;
	}

	protected void InternalMove( TScalar srcOffsetBytes, TScalar dstOffsetBytes, TScalar lengthBytes ) {
		DataUtilities.PerformMemCopy( this._dataPointer, this._dataPointer, srcOffsetBytes, dstOffsetBytes, lengthBytes );
		OnBufferWrittenTo?.Invoke( dstOffsetBytes, lengthBytes );
	}

	protected bool CopyTo<TRecepientScalar>( IWritableBuffer<TRecepientScalar> recipient, TScalar srcOffsetBytes, TRecepientScalar dstOffsetBytes, TRecepientScalar bytesToCopy )
		where TRecepientScalar : unmanaged, IBinaryInteger<TRecepientScalar>, IUnsignedNumber<TRecepientScalar> {
		ObjectDisposedException.ThrowIf( Disposed, this );
		if (srcOffsetBytes + TScalar.CreateSaturating( bytesToCopy ) > this.LengthBytes)
			return false;
		return recipient.WriteRange( this._dataPointer + nint.CreateSaturating( srcOffsetBytes ), bytesToCopy, dstOffsetBytes );
	}

	protected bool Overwrite<TRecepientScalar>( IWriteResizableBuffer<TRecepientScalar> recipient, TScalar srcOffsetBytes, TRecepientScalar bytesToCopy )
		where TRecepientScalar : unmanaged, IBinaryInteger<TRecepientScalar>, IUnsignedNumber<TRecepientScalar> {
		ObjectDisposedException.ThrowIf( Disposed, this );
		if (srcOffsetBytes + TScalar.CreateSaturating( bytesToCopy ) > this.LengthBytes)
			return false;
		return recipient.ResizeWrite( (nint) (this._dataPointer + nint.CreateSaturating( srcOffsetBytes )), bytesToCopy );
	}

	protected void Extend( TScalar numBytes ) {
		ObjectDisposedException.ThrowIf( Disposed, this );
		if (numBytes == TScalar.Zero)
			return;
		this.LengthBytes += numBytes;
		this._dataPointer = (byte*) NativeMemory.Realloc( this._dataPointer, nuint.CreateSaturating( this.LengthBytes ) );
		OnBufferResized?.Invoke( this );
		this.LogLine( $"Extended to {this.LengthBytes}!", Log.Level.VERBOSE );
	}

	protected override bool InternalDispose() {
		if (this._dataPointer != null) {
			NativeMemory.Free( this._dataPointer );
			this._dataPointer = null;
		}
		return true;
	}

#if DEBUG
	protected Memory<byte> GetDebugSlice( nint startOffsetBytes, nint lengthBytes ) => DebugUtilities.PointerToMemory( this._dataPointer + startOffsetBytes, (uint) lengthBytes );
#endif
}
