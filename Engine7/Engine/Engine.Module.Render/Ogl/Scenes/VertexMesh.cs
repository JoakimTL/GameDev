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

	public event Action? Changed;

	internal VertexMesh( BufferSegment vertexBufferSegment, BufferSegment elementBufferSegment ) {
		this.VertexBufferSegment = vertexBufferSegment;
		this.ElementBufferSegment = elementBufferSegment;
		VertexType = typeof( TVertex );
		ElementCount = (uint) elementBufferSegment.LengthBytes / IMesh.ElementSizeBytes;
		ElementOffset = (uint) elementBufferSegment.OffsetBytes / IMesh.ElementSizeBytes;
		VertexOffset = (uint) vertexBufferSegment.OffsetBytes / (uint) Marshal.SizeOf<TVertex>();

		vertexBufferSegment.OffsetChanged += OnOffsetChanged;
		elementBufferSegment.OffsetChanged += OnOffsetChanged;
	}

	private void OnOffsetChanged( IBufferSegment<ulong> segment ) => Changed?.Invoke();

	protected override bool InternalDispose() {
		VertexBufferSegment.Dispose();
		ElementBufferSegment.Dispose();
		return true;
	}
}
