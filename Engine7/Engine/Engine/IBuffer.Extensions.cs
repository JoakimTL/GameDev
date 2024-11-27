using System.Numerics;

namespace Engine;

public static class BufferExtensions {
	/// <summary>
	/// Copies data from the source buffer to this buffer.
	/// </summary>
	/// <param name="sourceBuffer">The source to copy from.</param>
	/// <param name="destinationBuffer">The destination buffer which will be copied to.</param>
	/// <param name="sourceOffsetBytes">Offset from the source buffer</param>
	/// <param name="destinationOffsetBytes">Offset at the destination buffer</param>
	/// <param name="bytesToCopy">Number of bytes to copy from the source to the destination.</param>
	/// <returns>Returns false if bytesToCopy isn't a natural number, or if either the <see cref="IReadableBuffer.ReadRange{T}(Span{T}, ulong)"/> or <see cref="IWritableBuffer.WriteRange{T}(Span{T}, ulong)"/> operation fails.</returns>
	public static unsafe bool CopyTo<TScalarSource, TScalarDestination>( this IReadableBuffer<TScalarSource> sourceBuffer, IWritableBuffer<TScalarDestination> destinationBuffer, TScalarSource sourceOffsetBytes, TScalarDestination destinationOffsetBytes, int bytesToCopy ) where TScalarSource : unmanaged, IBinaryInteger<TScalarSource>, IUnsignedNumber<TScalarSource> where TScalarDestination : unmanaged, IBinaryInteger<TScalarDestination>, IUnsignedNumber<TScalarDestination> {
		if (bytesToCopy <= 0)
			return false;
		if (bytesToCopy <= 131072) {
			byte* spanIntermediaryPtr = stackalloc byte[ bytesToCopy ];
			return sourceBuffer.ReadRange( spanIntermediaryPtr, TScalarSource.CreateSaturating( bytesToCopy ), sourceOffsetBytes ) && destinationBuffer.WriteRange( spanIntermediaryPtr, TScalarDestination.CreateSaturating( bytesToCopy ), destinationOffsetBytes );
		}
		byte* heapIntermediaryPtr = (byte*) System.Runtime.InteropServices.NativeMemory.Alloc( nuint.CreateSaturating( bytesToCopy ) );
		bool result = sourceBuffer.ReadRange( heapIntermediaryPtr, TScalarSource.CreateSaturating( bytesToCopy ), sourceOffsetBytes ) && destinationBuffer.WriteRange( heapIntermediaryPtr, TScalarDestination.CreateSaturating( bytesToCopy ), destinationOffsetBytes );
		System.Runtime.InteropServices.NativeMemory.Free( heapIntermediaryPtr );
		return result;
	}

	public static unsafe bool Read<TScalar, T>( this IReadableBuffer<TScalar> buffer, TScalar sourceOffsetBytes, out T value ) where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> where T : unmanaged {
		value = default;
		Span<T> dst = stackalloc T[ 1 ];
		if (!buffer.ReadRange( dst, sourceOffsetBytes ))
			return false;
		value = dst[ 0 ];
		return true;
	}

	public static unsafe bool Write<TScalar, T>( this IWritableBuffer<TScalar> buffer, TScalar destinationOffsetBytes, T value ) where TScalar : unmanaged, IBinaryInteger<TScalar>, IUnsignedNumber<TScalar> where T : unmanaged
		=> buffer.WriteRange( [ value ], destinationOffsetBytes );
}
