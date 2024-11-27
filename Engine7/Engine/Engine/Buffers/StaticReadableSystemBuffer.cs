using System.Numerics;

namespace Engine.Buffers;

public unsafe class StaticReadableSystemBuffer( ulong initialLengthBytes ) : SystemBufferBase<ulong>( initialLengthBytes ), IReadableBuffer<ulong>, ICopyableBuffer<ulong> {
	public new bool ReadRange<T>( Span<T> destination, ulong sourceOffsetBytes ) where T : unmanaged => base.ReadRange( destination, sourceOffsetBytes );
	public new bool ReadRange( void* dstPtr, ulong dstLengthBytes, ulong sourceOffsetBytes ) => base.ReadRange( dstPtr, dstLengthBytes, sourceOffsetBytes );
	public new bool CopyTo<TRecepientScalar>( IWritableBuffer<TRecepientScalar> recepient, ulong srcOffsetBytes, TRecepientScalar dstOffsetBytes, TRecepientScalar bytesToCopy )
		where TRecepientScalar : unmanaged, IBinaryInteger<TRecepientScalar>, IUnsignedNumber<TRecepientScalar>
		=> base.CopyTo( recepient, srcOffsetBytes, dstOffsetBytes, bytesToCopy );
	public new bool Overwrite<TRecepientScalar>( IWriteResizableBuffer<TRecepientScalar> recepient, ulong srcOffsetBytes, TRecepientScalar bytesToCopy )
		where TRecepientScalar : unmanaged, IBinaryInteger<TRecepientScalar>, IUnsignedNumber<TRecepientScalar>
		=> base.Overwrite( recepient, srcOffsetBytes, bytesToCopy );
}
