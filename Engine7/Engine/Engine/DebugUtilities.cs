using Engine.Logging;
using System.Runtime.CompilerServices;

namespace Engine;

public static class DebugUtilities {
	public static unsafe Memory<byte> PointerToMemory( byte* src, uint length ) {
		Memory<byte> data = new byte[ length ];
		using (System.Buffers.MemoryHandle dstPin = data.Pin())
			Unsafe.CopyBlock( dstPin.Pointer, src, length );
		return data;
	}

	public static void Breakpoint( this object obj ) {
		if (System.Diagnostics.Debugger.IsAttached) {
			obj.LogWarning( $"Triggering a breakpoint!" );
			System.Diagnostics.Debugger.Break();
			return;
		}
		obj.LogWarning( $"Attempted to trigger a breakpoint, but no debugger is attached." );
	}
}
