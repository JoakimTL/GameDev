using System.Numerics;

namespace Engine.Standard.Render.Text.Fonts;

public static class FontUtilities {

	public static unsafe T FromBigEndian<T>( this T value ) where T : unmanaged, IBinaryNumber<T> {
		if (!BitConverter.IsLittleEndian)
			return value;
		T returnValue = T.Zero;
		byte* srcPtr = (byte*) &value;
		byte* dstPtr = (byte*) &returnValue;
		for (int i = 0; i < sizeof( T ); i++)
			dstPtr[ sizeof( T ) - i - 1 ] = srcPtr[ i ];
		return *(T*) dstPtr;
	}

	public static unsafe T Read<T>( byte* srcPtr, ref nint offset ) where T : unmanaged {
		T value = *(T*) (srcPtr + offset);
		offset += sizeof( T );
		return value;
	}
}
