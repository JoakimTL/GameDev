using Engine.Buffers;

namespace Engine.Module.Render.Ogl.OOP.Buffers;

public class HostClientSynchronizedBufferPair<THostBuffer> : DisposableIdentifiable, IHostClientSynchronizedBufferPair<THostBuffer, ulong, OglDynamicBuffer, uint>, IClientBuffer
	where THostBuffer : IReadableBuffer<ulong>, IObservableBuffer<ulong>, IVariableLengthBuffer<ulong>, ICopyableBuffer<ulong> {

	public THostBuffer HostBuffer { get; }
	private readonly OglDynamicBuffer _clientBuffer;
	private readonly BufferChangeTracker<THostBuffer, ulong> _changeTracker;
	private readonly List<BufferChangeTracker<THostBuffer, ulong>.ChangedSection> _currentChanges;
	private bool _hostResized;

	public uint ClientBufferId => _clientBuffer.BufferId;

	public HostClientSynchronizedBufferPair( THostBuffer hostBuffer, OglDynamicBuffer clientBuffer ) {
		HostBuffer = hostBuffer;
		HostBuffer.OnBufferResized += OnHostBufferResized;
		_clientBuffer = clientBuffer;
		_changeTracker = new BufferChangeTracker<THostBuffer, ulong>( HostBuffer, 8192 );
		_currentChanges = [];
	}

	private void OnHostBufferResized( IBuffer<ulong> buffer ) => _hostResized = true;

	public void Synchronize() {
		if (!_changeTracker.HasChanges && !_hostResized)
			return;
		if (_hostResized) {
			if (HostBuffer.LengthBytes > uint.MaxValue)
				throw new ArgumentOutOfRangeException( nameof( HostBuffer.LengthBytes ), HostBuffer.LengthBytes, "Host buffer length exceeds client buffer length" );
			_hostResized = false;
			_changeTracker.Clear();
			HostBuffer.Overwrite( _clientBuffer, 0, (uint) HostBuffer.LengthBytes );
			return;
		}

		_currentChanges.Clear();
		_changeTracker.GetChanges( _currentChanges );
		foreach (BufferChangeTracker<THostBuffer, ulong>.ChangedSection change in _currentChanges) {
			if (change.OffsetBytes > uint.MaxValue)
				throw new ArgumentOutOfRangeException( nameof( BufferChangeTracker<THostBuffer, ulong>.ChangedSection.OffsetBytes ), change.OffsetBytes, "Section offset exceeds client buffer capacity." );
			if (change.SizeBytes > uint.MaxValue)
				throw new ArgumentOutOfRangeException( nameof( BufferChangeTracker<THostBuffer, ulong>.ChangedSection.SizeBytes ), change.SizeBytes, "Section size exceeds client buffer capactity." );
			HostBuffer.CopyTo( _clientBuffer, change.OffsetBytes, (uint) change.OffsetBytes, (uint) change.SizeBytes );
		}
	}

	protected override bool InternalDispose() {
		_changeTracker.Dispose();
		HostBuffer.OnBufferResized -= OnHostBufferResized;

		return true;
	}
}