using OpenGL;

namespace Engine.Rendering.Utilities;
public static class BufferResizing {

	private static VertexBufferObject? _tempBuffer;

	public static void DirectResize( this VertexBufferObject buffer, uint newSizeBytes ) {
		if ( !Resources.Render.InThread )
			return;
		_tempBuffer = new VertexBufferObject( "temp", buffer.SizeBytes, BufferUsage.DynamicDraw );
		Gl.CopyNamedBufferSubData( buffer.BufferId, _tempBuffer.BufferId, IntPtr.Zero, IntPtr.Zero, _tempBuffer.SizeBytes );
		buffer.DirectSetSize( newSizeBytes );
		Gl.CopyNamedBufferSubData( _tempBuffer.BufferId, buffer.BufferId, IntPtr.Zero, IntPtr.Zero, _tempBuffer.SizeBytes );
		_tempBuffer.Dispose();
		_tempBuffer = null;
	}
}
