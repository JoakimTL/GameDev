using System.Buffers;

namespace Engine.Buffers;

public sealed class PooledBufferData : IDisposable {
	private IMemoryOwner<byte>? _memoryOwner;
	private readonly int _lengthBytes;
	private readonly object? _tag;

	public PooledBufferData( List<byte> data, object? tag ) {
		_memoryOwner = MemoryPool<byte>.Shared.Rent( data.Count );
		_lengthBytes = data.Count;
		data.CopyTo( _memoryOwner.Memory.Span );
		this.Payload = _memoryOwner.Memory[ .._lengthBytes ];
		_tag = tag;
	}

	public PooledBufferData( int lengthBytes, object? tag ) {
		_memoryOwner = MemoryPool<byte>.Shared.Rent( lengthBytes );
		_lengthBytes = lengthBytes;
		this.Payload = _memoryOwner.Memory[ .._lengthBytes ];
		_tag = tag;
	}

	public Memory<byte> Payload { get; }

	~PooledBufferData() {
		System.Diagnostics.Debug.Fail( "Failed to dispose of pooled buffer data." );
	}

	public void Dispose() {
		if (_memoryOwner is null)
			return;
		_memoryOwner.Dispose();
		_memoryOwner = null;
		GC.SuppressFinalize( this );
	}
}