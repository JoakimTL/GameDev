using Engine.Logging;

namespace Engine.Module.Render;

public sealed class BufferSynchronizer<TSource, TDestination> where TSource : IReadableBuffer<ulong>, IObservableBuffer<ulong> where TDestination : IWritableBuffer<uint> {
	private readonly TSource _sourceBuffer;
	private readonly TDestination _destinationBuffer;
	private readonly Queue<(uint, uint)> _changedSegments;

	public BufferSynchronizer( TSource systemBuffer, TDestination gpuBuffer ) {
		this._sourceBuffer = systemBuffer;
		this._destinationBuffer = gpuBuffer;
		this._changedSegments = [];
		systemBuffer.OnBufferWrittenTo += OnSystemBufferWrittenTo;
	}

	private void OnSystemBufferWrittenTo( ulong offsetBytes, ulong lengthBytes ) {
		this._changedSegments.Enqueue( ((uint) offsetBytes, (uint) lengthBytes) );
	}

	public void Synchronize() {
		while (this._changedSegments.TryDequeue(out (uint, uint) segment))
			WriteSegment( segment );
	}

	private void WriteSegment( (uint Offset, uint Length) segment ) {
		Span<byte> data = stackalloc byte[ (int) segment.Length ];
		if (!this._sourceBuffer.ReadRange( data, segment.Offset )) {
			this.LogWarning( "Failed to read data from source buffer." );
			return;
		}
		this._destinationBuffer.WriteRange( data, segment.Offset );
	}
}
