namespace Engine.Modules.Rendering.Ogl.OOP;

internal sealed class OglBufferSegment( OglSegementedBuffer buffer, uint offsetBytes, uint lengthBytes ) : DisposableIdentifiable, IOglBufferSegment {

	private readonly OglSegementedBuffer _buffer = buffer;

	public uint OffsetBytes { get; private set; } = offsetBytes;
	public uint LengthBytes { get; } = lengthBytes;

	protected override string ExtraInformation => $"BUFSEG{_buffer.BufferId} {OffsetBytes} {LengthBytes / 1024d:N2}KiB";

	public event Action<IOglBufferSegment>? OffsetChanged;

	public void Write( nint srcPtr, uint dstOffsetBytes, uint lengthBytes ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		if (dstOffsetBytes + lengthBytes > LengthBytes)
			throw new OpenGlArgumentException( "Buffer write would exceed segment size", nameof( dstOffsetBytes ), nameof( lengthBytes ) );
		if (srcPtr == 0)
			throw new OpenGlArgumentException( "Buffer write source pointer cannot be null", nameof( srcPtr ) );
		_buffer.SegmentWrite( this, srcPtr, OffsetBytes + dstOffsetBytes, lengthBytes );
	}

	internal void SetOffset( uint offsetBytes ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		OffsetBytes = offsetBytes;
		OffsetChanged?.Invoke( this );
	}

	protected override bool InternalDispose() {
		_buffer.SegmentDisposed( this );
		return true;
	}
}

