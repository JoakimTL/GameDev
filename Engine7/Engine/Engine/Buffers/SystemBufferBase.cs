using Engine.Logging;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Buffers;

/// <summary>
/// Implements all the basic functionality of a buffer as protected methods.
/// </summary>
public abstract unsafe class SystemBufferBase<TScalar>( TScalar initialLengthBytes ) : DisposableIdentifiable, IBuffer<TScalar>, IObservableBuffer<TScalar>, IVariableLengthBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	private byte* _dataPointer = (byte*) NativeMemory.Alloc( nuint.CreateSaturating( initialLengthBytes ) );
	public TScalar LengthBytes { get; private set; } = initialLengthBytes;
	public event Action<IBuffer<TScalar>>? BufferResized;
	public event BufferDataChanged<TScalar>? BufferWrittenTo;

	protected bool ReadRange<T>( Span<T> destination, TScalar sourceOffsetBytes ) where T : unmanaged {
		if (destination.Length == 0)
			return true;
		TScalar bytesToCopy = TScalar.CreateSaturating( (ulong) destination.Length * (uint) sizeof( T ) );
		if (sourceOffsetBytes + bytesToCopy > LengthBytes)
			return false;
		fixed (T* dstPtr = destination)
			DataUtilities.PerformMemCopy( _dataPointer, (byte*) dstPtr, sourceOffsetBytes, 0, bytesToCopy );
		return true;
	}

	protected bool ReadRange( void* dstPtr, TScalar dstLengthBytes, TScalar sourceOffsetBytes ) {
		if (dstLengthBytes == TScalar.Zero)
			return true;
		if (sourceOffsetBytes + dstLengthBytes > LengthBytes)
			return false;
		DataUtilities.PerformMemCopy( _dataPointer, (byte*) dstPtr, sourceOffsetBytes, 0, dstLengthBytes );
		return true;
	}

	protected bool WriteRange<T>( Span<T> source, TScalar destinationOffsetBytes ) where T : unmanaged {
		if (source.Length == 0)
			return true;
		TScalar bytesToCopy = TScalar.CreateSaturating( (ulong) source.Length * (uint) sizeof( T ) );
		if (destinationOffsetBytes + bytesToCopy > LengthBytes)
			return false;
		fixed (T* srcPtr = source)
			DataUtilities.PerformMemCopy( (byte*) srcPtr, _dataPointer, 0, destinationOffsetBytes, bytesToCopy );
		BufferWrittenTo?.Invoke( destinationOffsetBytes, bytesToCopy );
		return true;
	}

	protected bool WriteRange( void* srcPtr, TScalar srcLengthBytes, TScalar destinationOffsetBytes ) {
		if (srcLengthBytes == TScalar.Zero)
			return true;
		if (destinationOffsetBytes + srcLengthBytes > LengthBytes)
			return false;
		DataUtilities.PerformMemCopy( (byte*) srcPtr, _dataPointer, 0, destinationOffsetBytes, srcLengthBytes );
		BufferWrittenTo?.Invoke( destinationOffsetBytes, srcLengthBytes );
		return true;
	}

	protected void InternalMove( TScalar srcOffsetBytes, TScalar dstOffsetBytes, TScalar lengthBytes ) {
		DataUtilities.PerformMemCopy( _dataPointer, _dataPointer, srcOffsetBytes, dstOffsetBytes, lengthBytes );
		BufferWrittenTo?.Invoke( dstOffsetBytes, lengthBytes );
	}

	protected void Extend( TScalar numBytes ) {
		if (numBytes == TScalar.Zero)
			return;
		LengthBytes += numBytes;
		_dataPointer = (byte*) NativeMemory.Realloc( _dataPointer, nuint.CreateSaturating( LengthBytes ) );
		BufferResized?.Invoke( this );
		this.LogLine( $"Extended to {LengthBytes}!", Log.Level.VERBOSE );
	}

	protected override bool InternalDispose() {
		if (_dataPointer != null) {
			NativeMemory.Free( _dataPointer );
			_dataPointer = null;
		}
		return true;
	}
}
