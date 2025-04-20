using Engine.Buffers;

namespace Engine.Module.Render.Ogl.OOP.Buffers;

public class HostClientSynchronizedBufferPair<THostBuffer> : DisposableIdentifiable, IHostClientSynchronizedBufferPair<THostBuffer, ulong, OglDynamicBuffer, uint>, IClientBuffer
	where THostBuffer : ICopyableBuffer<ulong>, IObservableBuffer<ulong>, IVariableLengthBuffer<ulong> {

	public THostBuffer HostBuffer { get; }
	private OglDynamicBuffer? _clientBuffer;
	private readonly BufferChangeTracker<THostBuffer, ulong> _changeTracker;
	private readonly List<BufferChangeTracker<THostBuffer, ulong>.ChangedSection> _currentChanges;
	private bool _hostResized;

	public uint? ClientBufferId => this._clientBuffer?.BufferId;

	public HostClientSynchronizedBufferPair( THostBuffer hostBuffer, OglDynamicBuffer? clientBuffer ) {
		this.HostBuffer = hostBuffer;
		this.HostBuffer.OnBufferResized += OnHostBufferResized;
		this._clientBuffer = clientBuffer;
		this._changeTracker = new BufferChangeTracker<THostBuffer, ulong>( this.HostBuffer, 8192 );
		this._currentChanges = [];
		Nickname = $"{hostBuffer} -> {clientBuffer}";
	}

	private void OnHostBufferResized( IBuffer<ulong> buffer ) => this._hostResized = true;

	public void SetClientBuffer( OglDynamicBuffer clientBuffer ) {
		if (this._clientBuffer is not null)
			throw new InvalidOperationException( "Client buffer is already set." );
		this._clientBuffer = clientBuffer;
	}

	public void Synchronize() {
		if (_clientBuffer is null)
			throw new InvalidOperationException( "Client buffer is not set." );
		this._hostResized |= this.HostBuffer.LengthBytes != this._clientBuffer.LengthBytes;
		if (!this._changeTracker.HasChanges && !this._hostResized)
			return;
		if (this._hostResized) {
			if (this.HostBuffer.LengthBytes > uint.MaxValue)
				throw new ArgumentOutOfRangeException( nameof( this.HostBuffer.LengthBytes ), this.HostBuffer.LengthBytes, "Host buffer length exceeds client buffer length" );
			this._hostResized = false;
			this._changeTracker.Clear();
			this.HostBuffer.Overwrite( this._clientBuffer, 0, (uint) this.HostBuffer.LengthBytes );
			return;
		}

		this._currentChanges.Clear();
		this._changeTracker.GetChanges( this._currentChanges );
		foreach (BufferChangeTracker<THostBuffer, ulong>.ChangedSection change in this._currentChanges) {
			if (change.OffsetBytes > uint.MaxValue)
				throw new ArgumentOutOfRangeException( nameof( BufferChangeTracker<THostBuffer, ulong>.ChangedSection.OffsetBytes ), change.OffsetBytes, "Section offset exceeds client buffer capacity." );
			if (change.SizeBytes > uint.MaxValue)
				throw new ArgumentOutOfRangeException( nameof( BufferChangeTracker<THostBuffer, ulong>.ChangedSection.SizeBytes ), change.SizeBytes, "Section size exceeds client buffer capactity." );
			this.HostBuffer.CopyTo( this._clientBuffer, change.OffsetBytes, (uint) change.OffsetBytes, (uint) change.SizeBytes );
		}
	}

	protected override bool InternalDispose() {
		this._changeTracker.Dispose();
		this.HostBuffer.OnBufferResized -= OnHostBufferResized;

		return true;
	}
}