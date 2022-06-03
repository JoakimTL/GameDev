using Engine.Data.Buffers;

namespace Engine.Rendering;

public class RenderDataObject : DisposableIdentifiable {

	public bool AutoUpdate { get; }
	public SegmentedDataBuffer Buffer { get; }
	private readonly DataBufferChangeRegistry _changeRegistry;
	private bool[]? _changeSet;

	public RenderDataObject( string name, SegmentedDataBuffer dataBuffer, bool autoUpdate = true ) : base( name ) {
		this.Buffer = dataBuffer;
		this.AutoUpdate = autoUpdate;
		this._changeRegistry = new DataBufferChangeRegistry( this.Buffer );
	}

	public RenderDataObject( string name, uint initialSizeBytes, uint alignment = 1, bool autoUpdate = true, uint expansion = 0 ) : this( name, new( name, initialSizeBytes, alignment, expansion ), autoUpdate ) { }

	public void UpdateVBO( VertexBufferObject vbo ) => this.Buffer.UpdateVBO( vbo, this._changeRegistry, ref this._changeSet );

	protected override bool OnDispose() {
		this._changeRegistry.Dispose();
		return true;
	}
}
