using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Buffers;
public abstract class OglBufferBase : DisposableIdentifiable, IBuffer<uint> {

	public readonly BufferUsage Usage;
	public readonly uint BufferId;
	public uint LengthBytes { get; private set; }

	protected OglBufferBase( BufferUsage usage, uint lengthBytes ) {
		this.Usage = usage;
		this.BufferId = Gl.CreateBuffer();
		SetSize( lengthBytes );
	}

	protected void SetSize( uint size ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		if (size == 0)
			throw new OpenGlArgumentException( "Buffer cannot be size zero", nameof( size ) );
		this.LengthBytes = size;
		Gl.NamedBufferData( this.BufferId, size, nint.Zero, this.Usage );
		this.Nickname = $"BUF{this.BufferId} {this.LengthBytes / 1024d:N2}KiB";
	}

	protected void ResizeWrite( nint srcPtr, uint lengthBytes ) {
		if (this.Disposed)
			throw new ObjectDisposedException( this.FullName );
		if (lengthBytes == 0)
			throw new OpenGlArgumentException( "Buffer cannot be size zero", nameof( lengthBytes ) );
		if (srcPtr == 0)
			throw new OpenGlArgumentException( "Buffer write source pointer cannot be null", nameof( srcPtr ) );
		this.LengthBytes = lengthBytes;
		Gl.NamedBufferData( this.BufferId, lengthBytes, srcPtr, this.Usage );
		this.Nickname = $"BUF{this.BufferId} {this.LengthBytes / 1024d:N2}KiB";
	}

	protected void Write( nint srcPtr, uint dstOffsetBytes, uint lengthBytes ) {
		if (this.Disposed)
			throw new ObjectDisposedException( this.FullName );
		if (dstOffsetBytes + lengthBytes > this.LengthBytes)
			throw new OpenGlArgumentException( "Buffer write would exceed buffer size", nameof( dstOffsetBytes ), nameof( lengthBytes ) );
		if (srcPtr == 0)
			throw new OpenGlArgumentException( "Buffer write source pointer cannot be null", nameof( srcPtr ) );
		Gl.NamedBufferSubData( this.BufferId, (nint) dstOffsetBytes, lengthBytes, srcPtr );
	}

	protected override bool InternalDispose() {
		Gl.DeleteBuffers( this.BufferId );
		return true;
	}

	/// <summary>
	/// Resizes the buffer while retaining data.
	/// </summary>
	public static void InternalMove( OglBufferBase buffer, uint srcOffsetBytes, uint dstOffsetBytes, uint bytesToMove ) {
		ObjectDisposedException.ThrowIf( buffer.Disposed, buffer );
		if (srcOffsetBytes + bytesToMove > buffer.LengthBytes)
			throw new OpenGlArgumentException( "Source offset and length exceed buffer size", nameof( srcOffsetBytes ), nameof( bytesToMove ) );
		if (dstOffsetBytes + bytesToMove > buffer.LengthBytes)
			throw new OpenGlArgumentException( "Destination offset and length exceed buffer size", nameof( dstOffsetBytes ), nameof( bytesToMove ) );

		OglStaticBuffer _tempBuffer = new( BufferUsage.DynamicDraw, bytesToMove );
		Gl.CopyNamedBufferSubData( buffer.BufferId, _tempBuffer.BufferId, (nint) srcOffsetBytes, nint.Zero, _tempBuffer.LengthBytes );
		Gl.CopyNamedBufferSubData( _tempBuffer.BufferId, buffer.BufferId, nint.Zero, (nint) dstOffsetBytes, _tempBuffer.LengthBytes );
		_tempBuffer.Dispose();
	}

	/// <summary>
	/// Resizes the buffer while retaining data.
	/// </summary>
	public static void Resize( OglBufferBase buffer, uint newSizeBytes ) {
		ObjectDisposedException.ThrowIf( buffer.Disposed, buffer );
		OglStaticBuffer _tempBuffer = new( BufferUsage.DynamicDraw, Math.Min( buffer.LengthBytes, newSizeBytes ) );
		Gl.CopyNamedBufferSubData( buffer.BufferId, _tempBuffer.BufferId, nint.Zero, nint.Zero, _tempBuffer.LengthBytes );
		buffer.SetSize( newSizeBytes );
		Gl.CopyNamedBufferSubData( _tempBuffer.BufferId, buffer.BufferId, nint.Zero, nint.Zero, _tempBuffer.LengthBytes );
		_tempBuffer.Dispose();
	}
}
