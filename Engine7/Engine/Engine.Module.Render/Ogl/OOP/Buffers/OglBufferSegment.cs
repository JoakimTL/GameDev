namespace Engine.Module.Render.Ogl.OOP.Buffers;

public sealed class OglBufferSegment : DisposableIdentifiable, IBufferSegment<uint>, IWritableBuffer<uint> {
	private readonly OglSegmentedBuffer _buffer;

	public uint BufferId => this._buffer.BufferId;

	public uint OffsetBytes { get; private set; }
	public uint LengthBytes { get; }

	public event Action<IBufferSegment<uint>>? OffsetChanged;

	internal OglBufferSegment( OglSegmentedBuffer buffer, uint offsetBytes, uint lengthBytes ) {
		this._buffer = buffer;
		this.OffsetBytes = offsetBytes;
		this.LengthBytes = lengthBytes;
	}

	public unsafe bool WriteRange<T>( Span<T> source, uint destinationOffsetBytes ) where T : unmanaged {
		if (this.Disposed)
			return false;
		if (destinationOffsetBytes + ((ulong) source.Length * (uint) sizeof( T )) > this.LengthBytes)
			return false;
		if (!this._buffer.WriteRange( source, this.OffsetBytes + destinationOffsetBytes ))
			return false;
		return true;
	}

	public unsafe bool WriteRange( void* srcPtr, uint srcLengthBytes, uint destinationOffsetBytes ) {
		if (this.Disposed)
			return false;
		if (destinationOffsetBytes + srcLengthBytes > this.LengthBytes)
			return false;
		if (!this._buffer.WriteRange( srcPtr, srcLengthBytes, this.OffsetBytes + destinationOffsetBytes ))
			return false;
		return true;
	}

	internal void SetOffsetBytes( uint newOffsetBytes ) {
		if (this.OffsetBytes == newOffsetBytes)
			return;
		this.OffsetBytes = newOffsetBytes;
		OffsetChanged?.Invoke( this );
	}

	protected override bool InternalDispose() {
		this._buffer.Free( this );
		return true;
	}
}