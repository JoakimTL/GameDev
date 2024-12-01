using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine;

public static class DataUtilities {
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static unsafe void PerformMemCopy<TScalarSource, TScalarDestination, TScalarLength>( byte* srcPtr, byte* dstPtr, TScalarSource srcOffsetBytes, TScalarDestination dstOffsetBytes, TScalarLength lengthBytes )
		where TScalarSource : unmanaged, IBinaryInteger<TScalarSource>
		where TScalarDestination : unmanaged, IBinaryInteger<TScalarDestination>
		where TScalarLength : unmanaged, IBinaryInteger<TScalarLength> {
		nuint srcOffsetBytesAsIntPtr = nuint.CreateSaturating( srcOffsetBytes );
		nuint dstOffsetBytesAsIntPtr = nuint.CreateSaturating( dstOffsetBytes );
		nuint bytesToCopyAsIntPtr = nuint.CreateSaturating( lengthBytes );
		Buffer.MemoryCopy( srcPtr + srcOffsetBytesAsIntPtr, dstPtr + dstOffsetBytesAsIntPtr, bytesToCopyAsIntPtr, bytesToCopyAsIntPtr );
	}
}