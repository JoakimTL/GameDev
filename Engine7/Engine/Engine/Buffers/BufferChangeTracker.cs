using Engine.Structures;
using System.Collections.Concurrent;
using System.Numerics;

namespace Engine.Buffers;

public sealed class BufferChangeTracker<THostBuffer, THostScalar> : DisposableIdentifiable
	where THostBuffer : IReadableBuffer<THostScalar>, IObservableBuffer<THostScalar>, IVariableLengthBuffer<THostScalar>
	where THostScalar : unmanaged, IBinaryInteger<THostScalar>, IUnsignedNumber<THostScalar> {

	public THostBuffer HostBuffer { get; }

	private readonly ConcurrentQueue<ChangedSection> _incoming;
	private readonly SimpleSortedList<ChangedSection> _changes;
	private readonly THostScalar _minimumHoleSizeBytes;

	public bool HasChanges => _incoming.Count > 0;

	/// <param name="hostBuffer"></param>
	/// <param name="minimumHoleSizeBytes">The minimum number of bytes between written sections before they are deemed different contigous parts.</param>
	public BufferChangeTracker( THostBuffer hostBuffer, THostScalar minimumHoleSizeBytes ) {
		_incoming = [];
		_changes = new();
		this.HostBuffer = hostBuffer;
		this._minimumHoleSizeBytes = minimumHoleSizeBytes;
		this.HostBuffer.OnBufferWrittenTo += this.OnBufferWrittenTo;
	}

	private void OnBufferWrittenTo( THostScalar offsetBytes, THostScalar lengthBytes ) => _incoming.Enqueue( new( offsetBytes, lengthBytes ) );

	public void GetChanges( List<ChangedSection> outputContainer ) {
		_changes.Clear();
		while (_incoming.TryDequeue( out ChangedSection section ))
			_changes.Add( section );

		outputContainer.Clear();
		if (_changes.Count == 0)
			return;

		THostScalar offset = _changes[ 0 ].OffsetBytes;
		THostScalar size = _changes[ 0 ].SizeBytes;

		for (int i = 1; i < _changes.Count; i++) {
			var change = _changes[ i ];
			if (change.OffsetBytes > offset + size + _minimumHoleSizeBytes ) {
				outputContainer.Add( new ChangedSection( offset, size ) );
				offset = change.OffsetBytes;
				size = change.SizeBytes;
			} else {
				size = change.OffsetBytes + change.SizeBytes - offset;
			}
		}
		outputContainer.Add( new ChangedSection( offset, size ) );
	}

	public void Clear() => _incoming.Clear();

	protected override bool InternalDispose() {
		_incoming.Clear();
		this.HostBuffer.OnBufferWrittenTo -= this.OnBufferWrittenTo;
		return true;
	}

	public readonly struct ChangedSection( THostScalar offset, THostScalar size ) : IComparable<ChangedSection> {
		public readonly THostScalar OffsetBytes = offset;
		public readonly THostScalar SizeBytes = size;

		public int CompareTo( ChangedSection other ) => OffsetBytes.CompareTo( other.OffsetBytes );
	}
}
