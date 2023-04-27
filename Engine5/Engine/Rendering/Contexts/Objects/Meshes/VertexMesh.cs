using Engine.Datatypes.Buffers;

namespace Engine.Rendering.Contexts.Objects.Meshes;

public class VertexMesh<T> : BufferedMeshBase where T : unmanaged
{
    public VertexMesh(string name, ISegmentedBufferSegment vertexSegment, uint vertexSizeBytes, ISegmentedBufferSegment elementSegment) : base(name, typeof(T), vertexSegment, vertexSizeBytes, elementSegment) { }

    public void WriteElements(Span<uint> indices) => ElementSegment.Write(0, (ReadOnlySpan<uint>)indices);
    public void WriteElements(uint[] indices) => ElementSegment.Write(0, indices);
    public void WriteVertices(Span<T> vertices) => VertexSegment.Write(0, (ReadOnlySpan<T>)vertices);
    public void WriteVertices(T[] vertices) => VertexSegment.Write(0, vertices);
}
