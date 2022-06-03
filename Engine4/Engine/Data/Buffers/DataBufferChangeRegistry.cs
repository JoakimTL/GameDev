using System.Collections.Concurrent;

namespace Engine.Data.Buffers;
public class DataBufferChangeRegistry : DisposableIdentifiable {

	private readonly DataBuffer _dataBuffer;
	public readonly uint SectionSizeBytes;
	private readonly ConcurrentQueue<DataBuffer.SectionUpdate> _updates;
	private bool _resized;

	public DataBufferChangeRegistry( DataBuffer buffer, uint sectionSizeBytes = 65536 ) {
		this.SectionSizeBytes = sectionSizeBytes;
		this._updates = new ConcurrentQueue<DataBuffer.SectionUpdate>();
		this._dataBuffer = buffer;
		this._dataBuffer.SegmentUpdated += SegmentUpdated;
		this._dataBuffer.Resized += BufferResized;
	}

	private void BufferResized( ulong obj ) => this._resized = true;
	private void SegmentUpdated( in DataBuffer.SectionUpdate update ) => this._updates.Enqueue( update );

	public UpdateState ConsumeUpdates( ref bool[]? changedSections ) {
		if ( this.Disposed )
			return UpdateState.DISPOSED;
		if ( this._resized ) {
			this._updates.Clear();
			this._resized = false;
			return UpdateState.RESIZED;
		}
		ulong wantedSize = ( this._dataBuffer.SizeBytes / this.SectionSizeBytes ) + 1;
		if ( changedSections is null || ( ulong) changedSections.LongLength < wantedSize )
			changedSections = new bool[ wantedSize ];
		bool changed = false;
		while ( this._updates.TryDequeue( out DataBuffer.SectionUpdate update ) ) {
			ulong start = update.Offset / this.SectionSizeBytes;
			ulong end = ( update.Offset + update.Size - 1 ) / this.SectionSizeBytes;
			for ( ulong i = start; i <= end; i++ ) {
				changedSections[ i ] = true;
			}
			changed = true;
		}
		return changed ? UpdateState.SECTIONS : UpdateState.NONE;
	}

	protected override bool OnDispose() {
		this._dataBuffer.SegmentUpdated -= SegmentUpdated;
		this._dataBuffer.Resized -= BufferResized;
		return true;
	}

	public enum UpdateState {
		NONE,
		SECTIONS,
		RESIZED,
		DISPOSED
	}
}
