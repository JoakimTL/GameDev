using System.Buffers;

namespace Engine.Module.Entities.Container;

internal class SerializationBuffer {

	[ThreadStatic]
	private static SerializationBuffer? _buffer;

	public static SerializationBuffer GetBuffer() {
		_buffer ??= new();
		_buffer._internalBuffer.Clear();
		return _buffer;
	}

	private readonly List<byte> _internalBuffer;

	public SerializationBuffer() {
		_internalBuffer = [];
	}

	public void Add( ReadOnlySpan<byte> bytes ) => _internalBuffer.AddRange( bytes );

	public SerializationResult GetData() {
		IMemoryOwner<byte> memory = MemoryPool<byte>.Shared.Rent( _internalBuffer.Count );
		_internalBuffer.CopyTo( memory.Memory.Span );
		return new( memory, _internalBuffer.Count );
	}
}
