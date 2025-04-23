using Engine.Buffers;

namespace Engine.Module.Entities.Container;

public sealed class EntitySerializationResult( PooledBufferData internalResult ) : IDisposable {

	private readonly PooledBufferData _internalResult = internalResult;

	public ReadOnlyMemory<byte> Payload => _internalResult.Payload;

	public void Dispose() {
		_internalResult.Dispose();
		GC.SuppressFinalize( this );
	}
}