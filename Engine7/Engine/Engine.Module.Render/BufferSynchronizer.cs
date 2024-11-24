using Engine.Logging;

namespace Engine.Module.Render;

public sealed class BufferSynchronizer<TSource, TDestination> where TSource : IReadableBuffer<ulong>, IObservableBuffer<ulong> where TDestination : IWritableBuffer<uint> {
	private readonly TSource _sourceBuffer;
	private readonly TDestination _destinationBuffer;
	private readonly Queue<(uint, uint)> _changedSegments;

	public BufferSynchronizer( TSource systemBuffer, TDestination gpuBuffer ) {
		_sourceBuffer = systemBuffer;
		_destinationBuffer = gpuBuffer;
		_changedSegments = [];
		systemBuffer.BufferWrittenTo += OnSystemBufferWrittenTo;
	}

	private void OnSystemBufferWrittenTo( ulong offsetBytes, ulong lengthBytes ) {
		_changedSegments.Enqueue( ((uint) offsetBytes, (uint) lengthBytes) );
	}

	public void Synchronize() {
		while (_changedSegments.TryDequeue(out (uint, uint) segment))
			WriteSegment( segment );
	}

	private void WriteSegment( (uint Offset, uint Length) segment ) {
		Span<byte> data = stackalloc byte[ (int) segment.Length ];
		if (!_sourceBuffer.ReadRange( data, segment.Offset )) {
			this.LogWarning( "Failed to read data from source buffer." );
			return;
		}
		_destinationBuffer.WriteRange( data, segment.Offset );
	}
}
