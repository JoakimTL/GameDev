namespace Engine.Buffers;

public unsafe class DynamicWritableSystemBuffer( ulong initialLengthBytes ) : DynamicReadableSystemBuffer( initialLengthBytes ), IWritableBuffer<ulong> {
	public new bool WriteRange<T>( Span<T> source, ulong destinationOffsetBytes ) where T : unmanaged => base.WriteRange( source, destinationOffsetBytes );
	public new bool WriteRange( void* srcPtr, ulong srcLengthBytes, ulong destinationOffsetBytes ) => base.WriteRange( srcPtr, srcLengthBytes, destinationOffsetBytes );
}
