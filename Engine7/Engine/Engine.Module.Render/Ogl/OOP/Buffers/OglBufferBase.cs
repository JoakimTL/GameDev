using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Buffers;
public abstract class OglBufferBase : DisposableIdentifiable, IBuffer<uint> {

	public readonly BufferUsage Usage;
	public readonly uint BufferId;
	public uint LengthBytes { get; private set; }

	protected OglBufferBase( BufferUsage usage, uint lengthBytes ) {
		this.Usage = usage;
		BufferId = Gl.CreateBuffer();
		SetSize( lengthBytes );
	}

	protected void SetSize( uint size ) {
		ObjectDisposedException.ThrowIf( Disposed, this );
		if (size == 0)
			throw new OpenGlArgumentException( "Buffer cannot be size zero", nameof( size ) );
		LengthBytes = size;
		Gl.NamedBufferData( BufferId, size, nint.Zero, Usage );
		Nickname = $"BUF{BufferId} {LengthBytes / 1024d:N2}KiB";
	}

	protected void ResizeWrite( nint srcPtr, uint lengthBytes ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		if (lengthBytes == 0)
			throw new OpenGlArgumentException( "Buffer cannot be size zero", nameof( lengthBytes ) );
		if (srcPtr == 0)
			throw new OpenGlArgumentException( "Buffer write source pointer cannot be null", nameof( srcPtr ) );
		LengthBytes = lengthBytes;
		Gl.NamedBufferData( BufferId, lengthBytes, srcPtr, Usage );
		Nickname = $"BUF{BufferId} {LengthBytes / 1024d:N2}KiB";
	}

	protected void Write( nint srcPtr, uint dstOffsetBytes, uint lengthBytes ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		if (dstOffsetBytes + lengthBytes > LengthBytes)
			throw new OpenGlArgumentException( "Buffer write would exceed buffer size", nameof( dstOffsetBytes ), nameof( lengthBytes ) );
		if (srcPtr == 0)
			throw new OpenGlArgumentException( "Buffer write source pointer cannot be null", nameof( srcPtr ) );
		Gl.NamedBufferSubData( BufferId, (nint) dstOffsetBytes, lengthBytes, srcPtr );
	}

	protected override bool InternalDispose() {
		Gl.DeleteBuffers( BufferId );
		return true;
	}
}
