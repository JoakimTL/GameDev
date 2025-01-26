using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

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
		byte* offsetSrcPtr = srcPtr + srcOffsetBytesAsIntPtr;
		byte* offsetDstPtr = dstPtr + dstOffsetBytesAsIntPtr;
		Buffer.MemoryCopy( offsetSrcPtr, offsetDstPtr, bytesToCopyAsIntPtr, bytesToCopyAsIntPtr );
	}
	public static unsafe string ToStringNullStop( this nint pointer, Encoding encoding, int maxLen = 1024 ) {
		sbyte* ptr = (sbyte*) pointer.ToPointer();
		int len = 0;
		while (ptr[ len ] != 0 && len < maxLen)
			len++;

		return new string( ptr, 0, len, encoding );
	}

	public static unsafe int ToIntCode( this string code ) => unchecked(((byte) code[ 0 ] << 24) | ((byte) code[ 1 ] << 16) | ((byte) code[ 2 ] << 8) | (byte) code[ 3 ]);
}