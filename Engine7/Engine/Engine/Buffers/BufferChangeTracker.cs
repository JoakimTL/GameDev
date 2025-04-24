using Engine.Structures;
using System.Collections.Concurrent;
using System.Numerics;

namespace Engine.Buffers;

public sealed class BufferChangeTracker<THostBuffer, THostScalar> : DisposableIdentifiable
	where THostBuffer : IObservableBuffer<THostScalar>, IVariableLengthBuffer<THostScalar>
	where THostScalar : unmanaged, IBinaryInteger<THostScalar>, IUnsignedNumber<THostScalar> {

	public THostBuffer HostBuffer { get; }

	private readonly ConcurrentQueue<ChangedSection> _incoming;
	private readonly SimpleSortedList<ChangedSection> _changes;
	private readonly THostScalar _minimumHoleSizeBytes;

	public bool HasChanges => this._incoming.Count > 0;

	/// <param name="hostBuffer"></param>
	/// <param name="minimumHoleSizeBytes">The minimum number of bytes between written sections before they are deemed different contigous parts.</param>
	public BufferChangeTracker( THostBuffer hostBuffer, THostScalar minimumHoleSizeBytes ) {
		this._incoming = [];
		this._changes = new();
		this.HostBuffer = hostBuffer;
		this._minimumHoleSizeBytes = minimumHoleSizeBytes;
		this.HostBuffer.OnBufferWrittenTo += this.OnBufferWrittenTo;
	}

	private void OnBufferWrittenTo( THostScalar offsetBytes, THostScalar lengthBytes ) => this._incoming.Enqueue( new( offsetBytes, lengthBytes ) );

	public void GetChanges( List<ChangedSection> outputContainer ) {
		this._changes.Clear();
		while (this._incoming.TryDequeue( out ChangedSection section ))
			this._changes.Add( section );

		outputContainer.Clear();
		if (this._changes.Count == 0)
			return;

		THostScalar offset = this._changes[ 0 ].OffsetBytes;
		THostScalar size = this._changes[ 0 ].SizeBytes;

		for (int i = 1; i < this._changes.Count; i++) {
			ChangedSection change = this._changes[ i ];
			if (change.OffsetBytes > offset + size + this._minimumHoleSizeBytes) {
				outputContainer.Add( new ChangedSection( offset, size ) );
				offset = change.OffsetBytes;
				size = change.SizeBytes;
			} else {
				size = change.OffsetBytes + change.SizeBytes - offset;
			}
		}
		outputContainer.Add( new ChangedSection( offset, size ) );
	}

	public void Clear() => this._incoming.Clear();

	protected override bool InternalDispose() {
		this._incoming.Clear();
		this.HostBuffer.OnBufferWrittenTo -= this.OnBufferWrittenTo;
		return true;
	}

	public readonly struct ChangedSection( THostScalar offset, THostScalar size ) : IComparable<ChangedSection> {
		public readonly THostScalar OffsetBytes = offset;
		public readonly THostScalar SizeBytes = size;

		public int CompareTo( ChangedSection other ) => this.OffsetBytes.CompareTo( other.OffsetBytes );
	}
}
