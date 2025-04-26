using System.Buffers;

namespace Engine.Buffers;
public class ThreadedByteBuffer {
	[ThreadStatic]
	private static Dictionary<string, ThreadedByteBuffer>? _buffers;

	public static ThreadedByteBuffer GetBuffer( string bufferIdentity ) {
		_buffers ??= [];
		if (!_buffers.TryGetValue( bufferIdentity, out ThreadedByteBuffer? buffer ))
			_buffers[ bufferIdentity ] = buffer = new( bufferIdentity );
		buffer._internalBuffer.Clear();
		return buffer;
	}

	private readonly List<byte> _internalBuffer;

	public string Identity { get; }

	public int Count => _internalBuffer.Count;

	public ThreadedByteBuffer( string identity ) {
		_internalBuffer = [];
		this.Identity = identity;
	}

	public void Add( ReadOnlySpan<byte> bytes ) => _internalBuffer.AddRange( bytes );

	/// <summary>
	/// Remember to dispose of the data after use!
	/// </summary>
	/// <returns></returns>
	public PooledBufferData GetData( bool reset = true ) {
		PooledBufferData output = new( _internalBuffer );
		if (reset)
			_internalBuffer.Clear();
		return output;
	}
}
