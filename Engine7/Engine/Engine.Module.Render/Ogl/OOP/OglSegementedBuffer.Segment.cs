//namespace Engine.Module.Render.OpenGL.Ogl.OOP;

//internal sealed class OglBufferSegment : DisposableIdentifiable, IOglBufferSegment {

//	private readonly OglSegementedBuffer _buffer;

//	public uint OffsetBytes { get; private set; }
//	public uint LengthBytes { get; }

//	public event Action<IOglBufferSegment>? OffsetChanged;

//	public OglBufferSegment( OglSegementedBuffer buffer, uint offsetBytes, uint lengthBytes ) {
//		this._buffer = buffer;
//		this.OffsetBytes = offsetBytes;
//		this.LengthBytes = lengthBytes;
//		this.Nickname = $"BUF{_buffer.BufferId} SEG {OffsetBytes} {LengthBytes / 1024d:N2}KiB";
//	}
//	public void Write( nint srcPtr, uint dstOffsetBytes, uint lengthBytes ) {
//		if (Disposed)
//			throw new ObjectDisposedException( FullName );
//		if (dstOffsetBytes + lengthBytes > LengthBytes)
//			throw new OpenGlArgumentException( "Buffer write would exceed segment size", nameof( dstOffsetBytes ), nameof( lengthBytes ) );
//		if (srcPtr == 0)
//			throw new OpenGlArgumentException( "Buffer write source pointer cannot be null", nameof( srcPtr ) );
//		_buffer.SegmentWrite( this, srcPtr, OffsetBytes + dstOffsetBytes, lengthBytes );
//	}

//	internal void SetOffset( uint offsetBytes ) {
//		if (Disposed)
//			throw new ObjectDisposedException( FullName );
//		OffsetBytes = offsetBytes;
//		this.Nickname = $"BUFSEG{_buffer.BufferId} {OffsetBytes} {LengthBytes / 1024d:N2}KiB";
//		OffsetChanged?.Invoke( this );
//	}

//	protected override bool InternalDispose() {
//		_buffer.SegmentDisposed( this );
//		return true;
//	}
//}

