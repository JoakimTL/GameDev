using System.Numerics;

namespace Engine;

public interface IBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	TScalar LengthBytes { get; }
}

public interface IObservableBuffer<TScalar> : IBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	event BufferDataChanged<TScalar>? OnBufferWrittenTo;
}

public interface IBufferSegment<TScalar> : IBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	TScalar OffsetBytes { get; }
}

public interface IRelocatingBufferSegment<TScalar> : IBufferSegment<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	event Action<IBufferSegment<TScalar>>? OffsetChanged;
}

public interface IExtendableBuffer<TScalar> : IVariableLengthBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	/// <summary>
	/// Extends the buffer by the specified number of bytes.
	/// </summary>
	void Extend( TScalar numBytes );
}

public interface IReadableBuffer<TScalar> : IBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	/// <summary>
	/// Reads a range of data into the destination span.
	/// </summary>
	/// <returns>True if the data was copied into the span, false if the operation failed.</returns>
	bool ReadRange<T>( Span<T> destination, TScalar sourceOffsetBytes ) where T : unmanaged;
	/// <summary>
	/// Reads a range of data into the destination span.
	/// </summary>
	/// <param name="dstPtr">Pointer to the destination.</param>
	/// <param name="dstLengthBytes">The number of bytes to copy to the destination.</param>
	/// <param name="sourceOffsetBytes">The offset in bytes from the start of the buffer to copy from.</param>
	/// <returns>True if the data was copied into the span, false if the operation failed.</returns>
	unsafe bool ReadRange( void* dstPtr, TScalar dstLengthBytes, TScalar sourceOffsetBytes );
}

public interface IVariableLengthBuffer<TScalar> : IBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	/// <summary>
	/// Called when the buffer has been resized.
	/// </summary>
	event Action<IBuffer<TScalar>>? OnBufferResized;
}

public interface IWritableBuffer<TScalar> : IBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	/// <summary>
	/// Writes a range of data from the source span.
	/// </summary>
	/// <returns>True if the data was copied from the span, false if the operation failed.</returns>
	bool WriteRange<T>( Span<T> source, TScalar destinationOffsetBytes ) where T : unmanaged;
	/// <summary>
	/// Writes a range of data from the source span.
	/// </summary>
	/// <returns>True if the data was copied from the span, false if the operation failed.</returns>
	unsafe bool WriteRange( void* srcPtr, TScalar srcLengthBytes, TScalar destinationOffsetBytes );
}

public interface ICopyableBuffer<TScalar> {
	bool CopyTo<TRecepientScalar>( IWritableBuffer<TRecepientScalar> recepient, TScalar srcOffsetBytes, TRecepientScalar dstOffsetBytes, TRecepientScalar bytesToCopy ) 
		where TRecepientScalar : unmanaged, IBinaryInteger<TRecepientScalar>, IUnsignedNumber<TRecepientScalar>;
	bool Overwrite<TRecepientScalar>( IWriteResizableBuffer<TRecepientScalar> recepient, TScalar srcOffsetBytes, TRecepientScalar bytesToCopy )
		where TRecepientScalar : unmanaged, IBinaryInteger<TRecepientScalar>, IUnsignedNumber<TRecepientScalar>;
}

public interface IWriteResizableBuffer<TScalar> where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> {
	bool ResizeWrite( nint srcPtr, TScalar srcLengthBytes );
}