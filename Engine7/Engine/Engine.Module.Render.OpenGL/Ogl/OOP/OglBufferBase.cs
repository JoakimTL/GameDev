using OpenGL;

namespace Engine.Module.Render.OpenGL.Ogl.OOP;
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
		ObjectDisposedException.ThrowIf( Disposed, this );
		if (lengthBytes == 0)
			throw new OpenGlArgumentException( "Buffer cannot be size zero", nameof( lengthBytes ) );
		if (srcPtr == 0)
			throw new OpenGlArgumentException( "Buffer write source pointer cannot be null", nameof( srcPtr ) );
		LengthBytes = lengthBytes;
		Gl.NamedBufferData( BufferId, lengthBytes, srcPtr, Usage );
		Nickname = $"BUF{BufferId} {LengthBytes / 1024d:N2}KiB";
	}

	protected void Write( nint srcPtr, uint dstOffsetBytes, uint lengthBytes ) {
		ObjectDisposedException.ThrowIf( Disposed, this );
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

public unsafe class StaticOglBuffer : OglBufferBase, IWritableBuffer<uint> {
	public StaticOglBuffer( BufferUsage usage, uint lengthBytes ) : base( usage, lengthBytes ) { }

	public bool WriteRange<T>( Span<T> source, uint destinationOffsetBytes ) where T : unmanaged {
		if (source.Length == 0)
			return false;
		uint bytesToCopy = (uint) (source.Length * sizeof( T ));
		if (destinationOffsetBytes + bytesToCopy > LengthBytes)
			return false;
		fixed (T* srcPtr = source)
			Write( new nint( srcPtr ), destinationOffsetBytes, bytesToCopy );
		return true;
	}

	public unsafe bool WriteRange<T>( T* srcPtr, uint srcLengthBytes, uint destinationOffsetBytes ) where T : unmanaged {
		if (source.Length == 0)
			return false;
		uint bytesToCopy = (uint) (source.Length * sizeof( T ));
		if (destinationOffsetBytes + bytesToCopy > LengthBytes)
			return false;
		fixed (T* srcPtr = source)
			Write( new nint( srcPtr ), destinationOffsetBytes, bytesToCopy );
		return true;
	}
}