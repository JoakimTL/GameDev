namespace Engine.OpenGL.OOP;

public interface IOglBufferSegment : IDisposable {
	uint OffsetBytes { get; }
	uint LengthBytes { get; }
	event Action<IOglBufferSegment>? OffsetChanged;
	void Write( nint srcPtr, uint dstOffsetBytes, uint lengthBytes );
}

