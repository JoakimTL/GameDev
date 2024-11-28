using Engine.Logging;
using Engine.Module.Render.Domain;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Buffers;

public unsafe class OglDynamicBuffer( BufferUsage usage, uint lengthBytes ) : OglStaticBuffer( usage, lengthBytes ), IWriteResizableBuffer<uint> {
	public new bool ResizeWrite( nint srcPtr, uint srcLengthBytes ) {
		if (this.Disposed)
			return this.LogWarningThenReturn( "Already disposed!", false );
		if (srcLengthBytes == 0)
			return this.LogWarningThenReturn( "Cannot write 0 bytes!", false );
		base.ResizeWrite( srcPtr, srcLengthBytes );
		return true;
	}
}