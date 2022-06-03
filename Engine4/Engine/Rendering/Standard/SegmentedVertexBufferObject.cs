using Engine.Data.Buffers;
using OpenGL;

namespace Engine.Rendering.Standard;

public class SegmentedVertexBufferObject : DisposableIdentifiable {

	public VertexBufferObject VBO { get; }
	public readonly uint ByteAlignment;
	private ulong _allocatedBytes;
	private readonly List<Segment> _segments;

	public SegmentedVertexBufferObject( string name, ulong sizeBytes, BufferUsage usage, uint segmentByteAlignment = sizeof( byte ) ) : base( name ) {
		if ( segmentByteAlignment == 0 )
			throw new ArgumentOutOfRangeException( nameof( segmentByteAlignment ), "Must be greater than zero!" );
		this.VBO = new VertexBufferObject( name, (uint) sizeBytes, usage );
		this._allocatedBytes = 0;
		this._segments = new List<Segment>();
		this.ByteAlignment = segmentByteAlignment;
	}

	public IDataSegmentInformation? AllocateSynchronized( uint sizeBytes ) {
		if ( sizeBytes % this.ByteAlignment != 0 ) {
			uint newSizeBytes = ( (sizeBytes / this.ByteAlignment) + 1 ) * this.ByteAlignment;
			this.LogWarning( $"Attempted to allocate segment outside alignment. Adjusting from {sizeBytes}B to {newSizeBytes}B!" );
			sizeBytes = newSizeBytes;
		}
		while ( this._allocatedBytes + sizeBytes > this.VBO.SizeBytes )
			Utilities.BufferResizing.DirectResize( this.VBO, this.VBO.SizeBytes * 2 );

		Segment segment = new( this._allocatedBytes, sizeBytes );
		this._segments.Add( segment );
		this._allocatedBytes += segment.SizeBytes;
		return segment;
	}

	protected override bool OnDispose() {
		this.VBO.Dispose();
		return true;
	}

	private class Segment : Identifiable, IDataSegmentInformation {
		public ulong OffsetBytes { get; private set; }
		public uint SizeBytes { get; private set; }
		public event Action<ulong>? OffsetChanged;

		protected override string UniqueNameTag => $"{this.OffsetBytes}->{this.SizeBytes / 1024d}KiB";

		internal Segment( ulong offsetBytes, uint sizeBytes ) {
			this.SizeBytes = sizeBytes;
			this.OffsetBytes = offsetBytes;
		}
	}

}
