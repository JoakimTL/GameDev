using OpenGL;
using System.Diagnostics;

namespace Engine.Rendering.Objects;

public class VertexBufferObject : Identifiable, IDisposable {

	private bool _disposed;
	public readonly BufferUsage Usage;
	public uint BufferId { get; private set; }
	public uint SizeBytes { get; private set; }

	protected override string UniqueNameTag => $"{this.BufferId}:{Usage}, {this.SizeBytes / 1048576d:N4}MiB";

	public VertexBufferObject( string name, uint sizeBytes, BufferUsage usage ) : base( name ) {
		_disposed = false;
		this.Usage = usage;
		this.BufferId = Gl.CreateBuffer();
		SetSize( sizeBytes );
	}

#if DEBUG
	~VertexBufferObject() {
		Debug.Fail( "VertexBufferObject was not disposed!" );
	}
#endif

	public bool Write( IntPtr dataPtr, uint dstOffsetBytes, uint srcOffsetBytes, uint lengthBytes ) {
		if ( this._disposed )
			return false;
		unsafe {
			Gl.NamedBufferSubData( this.BufferId, (IntPtr) dstOffsetBytes, lengthBytes, new IntPtr( (byte*) dataPtr.ToPointer() + srcOffsetBytes ) );
		}
		return true;
	}

	public bool ResizeWrite( IntPtr dataPtr, uint lengthBytes ) {
		if ( this._disposed )
			return false;
		this.LogLine( $"Resizing to {lengthBytes / 1024d:N0}KiB", Log.Level.NORMAL, color: ConsoleColor.Green );
		Gl.NamedBufferData( this.BufferId, lengthBytes, dataPtr, this.Usage );
		this.SizeBytes = lengthBytes;
		return true;
	}

	internal void SetSize( uint bytes ) {
		if ( this._disposed )
			return;
		this.LogLine( $"Sized to {bytes / 1024d:N0}KiB", Log.Level.NORMAL, color: ConsoleColor.Green );
		Gl.NamedBufferData( this.BufferId, bytes, IntPtr.Zero, this.Usage );
		this.SizeBytes = bytes;
	}

	public void Dispose() {
		Gl.DeleteBuffers( this.BufferId );
		_disposed = true;
		GC.SuppressFinalize( this );
	}
}
