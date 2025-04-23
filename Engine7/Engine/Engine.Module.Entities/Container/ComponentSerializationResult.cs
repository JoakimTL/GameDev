using Engine.Buffers;

namespace Engine.Module.Entities.Container;

public sealed class ComponentSerializationResult( PooledBufferData internalResult, Type componentType ) : IDisposable {

	private readonly PooledBufferData _internalResult = internalResult;

	public ReadOnlyMemory<byte> Payload => _internalResult.Payload;
	public Type ComponentType { get; } = componentType;

	public void Dispose() {
		_internalResult.Dispose();
		GC.SuppressFinalize( this );
	}
}
