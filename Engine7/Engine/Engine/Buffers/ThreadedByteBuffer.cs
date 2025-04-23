using System.Buffers;

namespace Engine.Buffers;
public class ThreadedByteBuffer {
	[ThreadStatic]
	private static Dictionary<string, ThreadedByteBuffer>? _buffers;

	public static ThreadedByteBuffer GetBuffer(string bufferIdentity) {
		_buffers ??= [];
		if (!_buffers.TryGetValue( bufferIdentity, out ThreadedByteBuffer? buffer ))
			_buffers[ bufferIdentity ] = buffer = new();
		buffer._internalBuffer.Clear();
		return buffer;
	}

	private readonly List<byte> _internalBuffer;

	public ThreadedByteBuffer() {
		_internalBuffer = [];
	}

	public void Add( ReadOnlySpan<byte> bytes ) => _internalBuffer.AddRange( bytes );

	/// <summary>
	/// Remember to dispose of the data after use!
	/// </summary>
	/// <returns></returns>
	public PooledBufferData GetData(int endPadding = 0) {
		for (int i = 0; i < endPadding; i++)
			_internalBuffer.Add( 0 );
		return new( _internalBuffer );
	}
}
