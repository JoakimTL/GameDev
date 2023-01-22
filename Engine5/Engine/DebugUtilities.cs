#if DEBUG
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Engine;

public static unsafe class DebugUtilities {

	public static Memory<byte> PointerToMemory( byte* src, uint length ) {
		Memory<byte> data = new byte[ length ];
		using ( var dstPin = data.Pin() )
			Unsafe.CopyBlock( dstPin.Pointer, src, length );
		return data;
	}
	public static void Breakpoint( this object? breakingObject ) {
		Log.Line( $"{breakingObject?.ToString() ?? "NULL"} caused a break!", Log.Level.NORMAL );
		Debugger.Break();
	}
}
#endif