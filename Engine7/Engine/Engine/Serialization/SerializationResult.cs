using Engine.Buffers;

namespace Engine.Serialization;

public sealed class SerializationResult( PooledBufferData internalResult ) : IDisposable {

	private readonly PooledBufferData _internalResult = internalResult;

	public ReadOnlyMemory<byte> Payload => _internalResult.Payload;

	public void Dispose() {
		_internalResult.Dispose();
		GC.SuppressFinalize( this );
	}
}