using OpenGL;

namespace Engine.Rendering;

public class VertexBufferObject : DisposableIdentifiable {

	public readonly BufferUsage Usage;
	public uint BufferId { get; private set; }
	public uint SizeBytes { get; private set; }

	public event Action? BufferIdSet;

	protected override string UniqueNameTag => $"{this.BufferId}, {this.SizeBytes / 1048576d:N4}MiB";

	public VertexBufferObject( string name, uint sizeBytes, BufferUsage usage ) : base( name ) {
		this.Usage = usage;
		this.BufferId = Gl.CreateBuffer();
		DirectSetSize( sizeBytes );
		BufferIdSet?.Invoke();
	}

	public bool DirectWrite( IntPtr dataPtr, uint dstOffsetBytes, uint srcOffsetBytes, uint lengthBytes ) {
		if ( this.Disposed )
			return false;
		unsafe {
			Gl.NamedBufferSubData( this.BufferId, (IntPtr) dstOffsetBytes, lengthBytes, new IntPtr( (byte*) dataPtr.ToPointer() + srcOffsetBytes ) );
		}
		return true;
	}

	public bool DirectResizeWrite( IntPtr dataPtr, uint lengthBytes ) {
		if ( this.Disposed )
			return false;
		this.LogLine( $"Resizing to {lengthBytes / 1024d:N0}KiB", Log.Level.NORMAL, color: ConsoleColor.Green );
		Gl.NamedBufferData( this.BufferId, lengthBytes, dataPtr, this.Usage );
		this.SizeBytes = lengthBytes;
		return true;
	}

	internal void DirectSetSize( uint bytes ) {
		if ( this.Disposed )
			return;
		this.LogLine( $"Sized to {bytes / 1024d:N0}KiB", Log.Level.NORMAL, color: ConsoleColor.Green );
		Gl.NamedBufferData( this.BufferId, bytes, IntPtr.Zero, this.Usage );
		this.SizeBytes = bytes;
	}

	protected override bool OnDispose() {
		Gl.DeleteBuffers( this.BufferId );
		return true;
	}
}
