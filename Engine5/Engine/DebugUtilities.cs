#if DEBUG
using System.Runtime.CompilerServices;

namespace Engine;

public static unsafe class DebugUtilities {

	public static Memory<byte> PointerToMemory( byte* src, uint length ) {
		Memory<byte> data = new byte[ length ];
		using ( var dstPin = data.Pin() )
			Unsafe.CopyBlock( dstPin.Pointer, src, length );
		return data;
	}
}
#endif