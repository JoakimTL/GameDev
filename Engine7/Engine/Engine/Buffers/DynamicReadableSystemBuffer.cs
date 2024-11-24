namespace Engine.Buffers;

public unsafe class DynamicReadableSystemBuffer( ulong initialLengthBytes ) : SystemBufferBase<ulong>( initialLengthBytes ), IReadableBuffer<ulong>, IExtendableBuffer<ulong> {
	public new bool ReadRange<T>( Span<T> destination, ulong sourceOffsetBytes ) where T : unmanaged => base.ReadRange( destination, sourceOffsetBytes );
	public new bool ReadRange( void* dstPtr, ulong dstLengthBytes, ulong sourceOffsetBytes ) => base.ReadRange( dstPtr, dstLengthBytes, sourceOffsetBytes );
	public new void Extend( ulong numBytes ) => base.Extend( numBytes );
}
