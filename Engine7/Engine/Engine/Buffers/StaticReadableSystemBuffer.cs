namespace Engine.Buffers;

public unsafe class StaticReadableSystemBuffer( ulong initialLengthBytes ) : SystemBufferBase<ulong>( initialLengthBytes ), IReadableBuffer<ulong> {
	public new bool ReadRange<T>( Span<T> destination, ulong sourceOffsetBytes ) where T : unmanaged => base.ReadRange( destination, sourceOffsetBytes );
	public new bool ReadRange( void* dstPtr, ulong dstLengthBytes, ulong sourceOffsetBytes ) => base.ReadRange( dstPtr, dstLengthBytes, sourceOffsetBytes );
}
