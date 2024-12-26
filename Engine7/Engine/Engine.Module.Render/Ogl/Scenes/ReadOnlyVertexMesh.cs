using Engine.Buffers;
using System.Runtime.InteropServices;

namespace Engine.Module.Render.Ogl.Scenes;

public class ReadOnlyVertexMesh<TVertex> : DisposableIdentifiable, IMesh where TVertex : unmanaged {

	protected readonly BufferSegment _vertexBufferSegment;
	protected readonly BufferSegment _elementBufferSegment;

	public Type VertexType { get; }
	public uint ElementCount { get; }
	public uint ElementOffset => (uint) _elementBufferSegment.OffsetBytes / IMesh.ElementSizeBytes;
	public uint VertexTypeSizeBytes { get; }
	public uint VertexOffset => (uint) _vertexBufferSegment.OffsetBytes / VertexTypeSizeBytes;

	public event Action? OnOffsetChanged;

	internal ReadOnlyVertexMesh( BufferSegment vertexBufferSegment, BufferSegment elementBufferSegment ) {
		this._vertexBufferSegment = vertexBufferSegment;
		this._elementBufferSegment = elementBufferSegment;
		this.VertexType = typeof( TVertex );
		this.ElementCount = (uint) elementBufferSegment.LengthBytes / IMesh.ElementSizeBytes;
		this.VertexTypeSizeBytes = (uint) Marshal.SizeOf<TVertex>();

		vertexBufferSegment.OffsetChanged += InvokeOnOffsetChanged;
		elementBufferSegment.OffsetChanged += InvokeOnOffsetChanged;
	}

	private void InvokeOnOffsetChanged( IBufferSegment<ulong> segment ) {
		OnOffsetChanged?.Invoke();
	}

	protected override bool InternalDispose() {
		this._vertexBufferSegment.Dispose();
		this._elementBufferSegment.Dispose();
		return true;
	}
}
