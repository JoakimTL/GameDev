using Engine.Buffers;
using System.Runtime.InteropServices;

namespace Engine.Module.Render.Ogl.Scenes;

public class ReadOnlyVertexMesh<TVertex> : DisposableIdentifiable, IMesh where TVertex : unmanaged {

	protected readonly BufferSegment _vertexBufferSegment;
	protected readonly BufferSegment _elementBufferSegment;

	public Type VertexType { get; }
	public uint ElementCount { get; }
	public uint ElementOffset { get; }
	public uint VertexTypeSizeBytes { get; }
	public uint VertexOffset { get; }

	public event Action? OnChanged;

	internal ReadOnlyVertexMesh( BufferSegment vertexBufferSegment, BufferSegment elementBufferSegment ) {
		this._vertexBufferSegment = vertexBufferSegment;
		this._elementBufferSegment = elementBufferSegment;
		this.VertexType = typeof( TVertex );
		this.ElementCount = (uint) elementBufferSegment.LengthBytes / IMesh.ElementSizeBytes;
		this.ElementOffset = (uint) elementBufferSegment.OffsetBytes / IMesh.ElementSizeBytes;
		this.VertexTypeSizeBytes = (uint) Marshal.SizeOf<TVertex>();
		this.VertexOffset = (uint) vertexBufferSegment.OffsetBytes / VertexTypeSizeBytes;

		vertexBufferSegment.OffsetChanged += OnOffsetChanged;
		elementBufferSegment.OffsetChanged += OnOffsetChanged;
	}

	private void OnOffsetChanged( IBufferSegment<ulong> segment ) => OnChanged?.Invoke();

	protected override bool InternalDispose() {
		this._vertexBufferSegment.Dispose();
		this._elementBufferSegment.Dispose();
		return true;
	}
}

public class VertexMesh<TVertex> : ReadOnlyVertexMesh<TVertex> where TVertex : unmanaged {

	public BufferSegment VertexBufferSegment => _vertexBufferSegment;
	public BufferSegment ElementBufferSegment => _elementBufferSegment;

	internal VertexMesh( BufferSegment vertexBufferSegment, BufferSegment elementBufferSegment ) : base( vertexBufferSegment, elementBufferSegment ) { }
}
