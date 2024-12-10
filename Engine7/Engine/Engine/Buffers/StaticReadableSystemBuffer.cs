using System.Numerics;

namespace Engine.Buffers;

public unsafe class StaticReadableSystemBuffer( ulong initialLengthBytes ) : SystemBufferBase<ulong>( initialLengthBytes ), IReadableBuffer<ulong>, ICopyableBuffer<ulong> {
	public new bool ReadRange<T>( Span<T> destination, ulong sourceOffsetBytes ) where T : unmanaged => base.ReadRange( destination, sourceOffsetBytes );
	public new bool ReadRange( void* dstPtr, ulong dstLengthBytes, ulong sourceOffsetBytes ) => base.ReadRange( dstPtr, dstLengthBytes, sourceOffsetBytes );
	public new bool CopyTo<TRecipientsScalar>( IWritableBuffer<TRecipientsScalar> recipient, ulong srcOffsetBytes, TRecipientsScalar dstOffsetBytes, TRecipientsScalar bytesToCopy )
		where TRecipientsScalar : unmanaged, IBinaryInteger<TRecipientsScalar>, IUnsignedNumber<TRecipientsScalar>
		=> base.CopyTo( recipient, srcOffsetBytes, dstOffsetBytes, bytesToCopy );
	public new bool Overwrite<TRecipientScalar>( IWriteResizableBuffer<TRecipientScalar> recipient, ulong srcOffsetBytes, TRecipientScalar bytesToCopy )
		where TRecipientScalar : unmanaged, IBinaryInteger<TRecipientScalar>, IUnsignedNumber<TRecipientScalar>
		=> base.Overwrite( recipient, srcOffsetBytes, bytesToCopy );
}
