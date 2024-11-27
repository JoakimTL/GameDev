using System.Numerics;

namespace Engine.Buffers;

public unsafe class DynamicReadableSystemBuffer( ulong initialLengthBytes ) : SystemBufferBase<ulong>( initialLengthBytes ), IReadableBuffer<ulong>, IExtendableBuffer<ulong>, ICopyableBuffer<ulong> {
	public new bool ReadRange<T>( Span<T> destination, ulong sourceOffsetBytes ) where T : unmanaged => base.ReadRange( destination, sourceOffsetBytes );
	public new bool ReadRange( void* dstPtr, ulong dstLengthBytes, ulong sourceOffsetBytes ) => base.ReadRange( dstPtr, dstLengthBytes, sourceOffsetBytes );
	public new void Extend( ulong numBytes ) => base.Extend( numBytes );
	public new bool CopyTo<TRecepientScalar>( IWritableBuffer<TRecepientScalar> recepient, ulong srcOffsetBytes, TRecepientScalar dstOffsetBytes, TRecepientScalar bytesToCopy )
		where TRecepientScalar : unmanaged, IBinaryInteger<TRecepientScalar>, IUnsignedNumber<TRecepientScalar>
		=> base.CopyTo( recepient, srcOffsetBytes, dstOffsetBytes, bytesToCopy );
	public new bool Overwrite<TRecepientScalar>( IWriteResizableBuffer<TRecepientScalar> recepient, ulong srcOffsetBytes, TRecepientScalar bytesToCopy )
		where TRecepientScalar : unmanaged, IBinaryInteger<TRecepientScalar>, IUnsignedNumber<TRecepientScalar>
		=> base.Overwrite( recepient, srcOffsetBytes, bytesToCopy );
}