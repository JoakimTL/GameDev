using Engine.Datatypes.Buffers;

namespace Engine.Rendering.Objects;


/// <summary>
/// Used by external threads to write data for the OGL thread
/// </summary>
public class RenderBufferObject : SegmentedBuffer {

	private List<ChangedSection> _changes;
	private readonly BufferWriteTracker _writeTracker;
	private bool _hasResized;

	public RenderBufferObject( string name, ulong initialSizeBytes ) : base( name, initialSizeBytes, true, true ) {
		_changes = new();
		_writeTracker = new( this );
		Resized += OnBufferResized;
	}

	private void OnBufferResized( ulong newSizeBytes ) {
		_hasResized = true;
	}

	public ISegmentedBufferSegment? Allocate( ulong sizeBytes ) => AllocateSegment( sizeBytes );

	internal unsafe void SyncChanges( VertexBufferObject vbo ) {
		if ( _hasResized ) {
			_writeTracker.Clear();
			vbo.ResizeWrite( (nint) Pointer, (uint) SizeBytes );
		} else {
			_writeTracker.GetChanges( _changes );
			for ( int i = 0; i < _changes.Count; i++ ) {
				var section = _changes[ i ];
				vbo.Write( (nint) Pointer, (uint) section.Offset, (uint) section.Offset, (uint) section.Size );
			}
		}
	}
}
