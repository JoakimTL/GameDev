using OpenGL;

namespace Engine.Modules.Rendering.Ogl.OOP;
public abstract class OglBufferBase : DisposableIdentifiable {

	public readonly BufferUsage Usage;
	public readonly uint BufferId;
	public uint LengthBytes { get; private set; }

	protected override string ExtraInformation => $"BUF{BufferId} {LengthBytes / 1024d:N2}KiB";

	protected OglBufferBase( BufferUsage usage, uint lengthBytes ) {
		this.Usage = usage;
		BufferId = Gl.CreateBuffer();
		SetSize( lengthBytes );
	}

	protected void SetSize( uint size ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		if (size == 0)
			throw new OpenGlArgumentException( "Buffer cannot be size zero", nameof( size ) );
		LengthBytes = size;
		Gl.NamedBufferData( BufferId, size, nint.Zero, Usage );

	}

	protected void ResizeWrite( nint srcPtr, uint lengthBytes ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		if (lengthBytes == 0)
			throw new OpenGlArgumentException( "Buffer cannot be size zero", nameof( lengthBytes ) );
		if (srcPtr == 0)
			throw new OpenGlArgumentException( "Buffer write source pointer cannot be null", nameof( srcPtr ) );
		LengthBytes = lengthBytes;
		Gl.NamedBufferData( BufferId, lengthBytes, srcPtr, Usage );
	}

	protected void Write( nint srcPtr, uint dstOffsetBytes, uint lengthBytes ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		if (dstOffsetBytes + lengthBytes > LengthBytes)
			throw new OpenGlArgumentException( "Buffer write would exceed buffer size", nameof( dstOffsetBytes ), nameof( lengthBytes ) );
		if (srcPtr == 0)
			throw new OpenGlArgumentException( "Buffer write source pointer cannot be null", nameof( srcPtr ) );
		Gl.NamedBufferSubData( BufferId, (nint) dstOffsetBytes, lengthBytes, srcPtr );
	}

	protected override bool InternalDispose() {
		Gl.DeleteBuffers( BufferId );
		return true;
	}
}

/// <summary>
/// 
/// </summary>
/// <param name="warningLog"></param>
/// <param name="usage"></param>
/// <param name="sizeBytes"></param>
/// <param name="resizeFactor">How much to resize when more space is needed. This is percentage, where 0 means no resizing and 1 means doubling in size.</param>
public sealed class OglSegementedBuffer( BufferUsage usage, uint sizeBytes, float resizeFactor ) : OglBufferBase( usage, sizeBytes ) {

	private readonly List<OglBufferSegment> _segments = [];
	private readonly float _resizeFactor = resizeFactor >= 0 ? resizeFactor : throw new ArgumentOutOfRangeException( nameof( resizeFactor ), "Can't have a negative resizing factor" );
	private bool _fragmented = false;
	private uint _bytesAllocated = 0;

	public IOglBufferSegment? Allocate( uint lengthBytes ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );

		if (lengthBytes == 0)
			throw new OpenGlArgumentException( "Buffer segment cannot be size zero", nameof( lengthBytes ) );
		//Check if we can fit a new segment in the buffer using the allocation and compare it with the lengthBytes to the buffer length
		//If we can't fit one we need to defragment if the buffer is fragmented, otherwise we need to resize the buffer if we're allowed. If we're not allowed to resize we return null.

		if (_bytesAllocated + lengthBytes > LengthBytes) {
			//We can't fit a new segment in the buffer as it stands
			//Is the buffer fragmented?
			if (_segments.Count == 0)
				_fragmented = false;

			if (_fragmented) {

				//The buffer is fragmented, we need to defragment it

				//To do this we loop through all the segments, and check if there are any gaps between them. We register each gap, then we loop over again.

				Span<BufferActiveDataSegment> segments = stackalloc BufferActiveDataSegment[ _segments.Count ];
				int numberOfSegments = 0;

				uint currentOffsetByte = segments[ 0 ].StartByte;
				uint currentLengthBytes = segments[ 0 ].LengthBytes;
				uint currentSegmentIndex = 0;
				uint segmentCount = 1;

				for (int i = 1; i < _segments.Count; i++) {
					OglBufferSegment seg = _segments[ i ];

					if (seg.OffsetBytes == currentOffsetByte + currentLengthBytes) {
						//This segment is adjacent to the current one, we can merge them
						currentLengthBytes += seg.LengthBytes;
						segmentCount++;
						continue;
					}

					//A gap was found, let's register the current data segment
					segments[ numberOfSegments++ ] = new( currentOffsetByte, currentLengthBytes, currentSegmentIndex, segmentCount );

					//And start a new one
					currentOffsetByte = seg.OffsetBytes;
					currentLengthBytes = seg.LengthBytes;
					currentSegmentIndex = (uint) i;
					segmentCount = 1;

				}

				segments[ numberOfSegments++ ] = new( currentOffsetByte, currentLengthBytes, currentSegmentIndex, segmentCount );

				if (numberOfSegments > 1) {
					_bytesAllocated = 0;
					for (int i = 0; i < numberOfSegments; i++)
						if (_bytesAllocated == segments[ i ].StartByte) {
							_bytesAllocated += segments[ i ].LengthBytes;
							continue;
						}
				}

				if (_bytesAllocated + lengthBytes <= LengthBytes)
					goto defragmented;
				//There are gaps, let's defragment
			}

			//We still don't have enough space, we need to resize the buffer




		}

	defragmented:

		return null;
	}

	private readonly struct BufferActiveDataSegment( uint startByte, uint lengthBytes, uint startSegment, uint segmentCount ) {
		public readonly uint StartByte = startByte;
		public readonly uint LengthBytes = lengthBytes;
		public readonly uint StartSegment = startSegment;
		public readonly uint SegmentCount = segmentCount;
	}

	internal void SegmentWrite( OglBufferSegment segment, nint srcPtr, uint dstOffsetBytes, uint lengthBytes )
		=> Write( srcPtr, segment.OffsetBytes + dstOffsetBytes, lengthBytes );

	internal void SegmentDisposed( OglBufferSegment segment ) {
		int index = _segments.IndexOf( segment );
		if (index == _segments.Count - 1)
			_fragmented = true;
		_segments.RemoveAt( index );
	}

}

