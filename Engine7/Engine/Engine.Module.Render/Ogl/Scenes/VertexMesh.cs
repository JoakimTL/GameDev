using Engine.Buffers;
using System.Runtime.InteropServices;

namespace Engine.Module.Render.Ogl.Scenes;

public class VertexMesh<TVertex> : DisposableIdentifiable, IMesh where TVertex : unmanaged {

	public BufferSegment VertexBufferSegment { get; }
	public BufferSegment ElementBufferSegment { get; }

	public Type VertexType { get; }
	public uint ElementCount { get; }
	public uint ElementOffset { get; }
	public uint VertexOffset { get; }

	public event Action? OnChanged;

	internal VertexMesh( BufferSegment vertexBufferSegment, BufferSegment elementBufferSegment ) {
		this.VertexBufferSegment = vertexBufferSegment;
		this.ElementBufferSegment = elementBufferSegment;
		this.VertexType = typeof( TVertex );
		this.ElementCount = (uint) elementBufferSegment.LengthBytes / IMesh.ElementSizeBytes;
		this.ElementOffset = (uint) elementBufferSegment.OffsetBytes / IMesh.ElementSizeBytes;
		this.VertexOffset = (uint) vertexBufferSegment.OffsetBytes / (uint) Marshal.SizeOf<TVertex>();

		vertexBufferSegment.OffsetChanged += OnOffsetChanged;
		elementBufferSegment.OffsetChanged += OnOffsetChanged;
	}

	private void OnOffsetChanged( IBufferSegment<ulong> segment ) => OnChanged?.Invoke();

	protected override bool InternalDispose() {
		this.VertexBufferSegment.Dispose();
		this.ElementBufferSegment.Dispose();
		return true;
	}
}
