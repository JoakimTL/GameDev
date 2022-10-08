using Engine.Data.Buffers;
using OpenGL;

namespace Engine.Rendering.Standard;

public class VertexBufferDataObject : DisposableIdentifiable {

	public VertexBufferObject VBO { get; private set; }
	public SegmentedDataBuffer Buffer { get; }
	private readonly DataBufferChangeRegistry _changeRegistry;
	private bool[]? _changeSet;

	//Separate into a VBO and a Render data object (Basically the memory on the CPU, while the VBO is the memory on the gpu. The RDO manager would create a new VBO and the RDO would fill it if it exists)?

	public VertexBufferDataObject( string name, SegmentedDataBuffer dataBuffer, BufferUsage usage ) {
		this.VBO = new VertexBufferObject( name, (uint) dataBuffer.SizeBytes, usage );
		this.Buffer = dataBuffer;
		this._changeRegistry = new DataBufferChangeRegistry( this.Buffer );
		this._changeSet = null;
	}

	public VertexBufferDataObject( string name, uint initialSizeBytes, BufferUsage usage, uint alignment = 1, uint expansion = 0 ) : this( name, new SegmentedDataBuffer( name, initialSizeBytes, alignment, expansion ), usage ) { }

	public void ContextUpdate() {
		if ( this.VBO is null )
			return;
		this.Buffer.UpdateVBO( this.VBO, this._changeRegistry, ref this._changeSet );
	}

	protected override bool OnDispose() {
		this._changeRegistry.Dispose();
		return true;
	}
}

