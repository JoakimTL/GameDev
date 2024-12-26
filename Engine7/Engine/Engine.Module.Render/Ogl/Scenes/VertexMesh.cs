using Engine.Buffers;

namespace Engine.Module.Render.Ogl.Scenes;

public class VertexMesh<TVertex> : ReadOnlyVertexMesh<TVertex> where TVertex : unmanaged {

	public BufferSegment VertexBufferSegment => _vertexBufferSegment;
	public BufferSegment ElementBufferSegment => _elementBufferSegment;

	internal VertexMesh( BufferSegment vertexBufferSegment, BufferSegment elementBufferSegment ) : base( vertexBufferSegment, elementBufferSegment ) { }
}
