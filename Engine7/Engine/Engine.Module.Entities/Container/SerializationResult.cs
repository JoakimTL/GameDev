using System.Buffers;

namespace Engine.Module.Entities.Container;

public sealed class SerializationResult : IDisposable {
	private IMemoryOwner<byte>? _memoryOwner;
	private readonly int _lengthBytes;

	public SerializationResult( IMemoryOwner<byte> memoryOwner, int lengthBytes ) {
		this._memoryOwner = memoryOwner;
		this._lengthBytes = lengthBytes;
		this.Payload = memoryOwner.Memory[ .._lengthBytes ];
	}

	public ReadOnlyMemory<byte> Payload { get; }

	~SerializationResult() {
		System.Diagnostics.Debug.Fail( "Failed to dispose of serialization result." );
	}

	public void Dispose() {
		if (_memoryOwner is null)
			return;
		_memoryOwner.Dispose();
		_memoryOwner = null;
		GC.SuppressFinalize( this );
	}
}
