using Engine.Rendering.OGL;
using Engine.Structure.Interfaces;
using OpenGL;

namespace Engine.Rendering.Objects;

public sealed class SegmentedVertexBufferObject : VertexBufferObject {

	public readonly uint ByteAlignment;
	private uint _allocatedBytes;
	private readonly List<Segment> _segments;

	public SegmentedVertexBufferObject( string name, uint sizeBytes, BufferUsage usage, uint segmentByteAlignment = sizeof( byte ) ) : base( name, sizeBytes, usage ) {
		if ( segmentByteAlignment == 0 )
			throw new ArgumentOutOfRangeException( nameof( segmentByteAlignment ), "Must be greater than zero!" );
		this._allocatedBytes = 0;
		this._segments = new List<Segment>();
		this.ByteAlignment = segmentByteAlignment;
	}

	public IBufferSegmentData<uint>? AllocateSynchronized( uint sizeBytes ) {
		if ( sizeBytes % this.ByteAlignment != 0 ) {
			uint newSizeBytes = ( ( sizeBytes / this.ByteAlignment ) + 1 ) * this.ByteAlignment;
			this.LogWarning( $"Attempted to allocate segment outside alignment. Adjusting from {sizeBytes}B to {newSizeBytes}B!" );
			sizeBytes = newSizeBytes;
		}

		while ( this._allocatedBytes + sizeBytes > this.SizeBytes )
			this.Resize( this.SizeBytes * 2 );

		Segment segment = new( this._allocatedBytes, sizeBytes );
		this._segments.Add( segment );
		this._allocatedBytes += segment.SizeBytes;
		return segment;
	}

	private class Segment : Identifiable, IBufferSegmentData<uint> {
		public uint OffsetBytes { get; private set; }
		public uint SizeBytes { get; private set; }
		public event Action<uint>? OffsetChanged;

		protected override string UniqueNameTag => $"{this.OffsetBytes}->{this.SizeBytes / 1024d}KiB";

		internal Segment( uint offsetBytes, uint sizeBytes ) {
			this.SizeBytes = sizeBytes;
			this.OffsetBytes = offsetBytes;
		}
	}

}
