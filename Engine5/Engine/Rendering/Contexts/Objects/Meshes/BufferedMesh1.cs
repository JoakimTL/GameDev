using Engine.Datatypes.Buffers;

namespace Engine.Rendering.Contexts.Objects.Meshes;

public class BufferedMesh : BufferedMeshBase
{
    public BufferedMesh(string name, Type vertexType, ISegmentedBufferSegment vertexSegment, uint vertexSizeBytes, ISegmentedBufferSegment elementSegment) : base(name, vertexType, vertexSegment, vertexSizeBytes, elementSegment) { }

    public void WriteElements(Span<byte> indexBytes) => ElementSegment.Write(0, (ReadOnlySpan<byte>)indexBytes);
    public void WriteElements(byte[] indexBytes) => ElementSegment.Write(0, indexBytes);
    public void WriteVertices(Span<byte> vertexBytes) => VertexSegment.Write(0, (ReadOnlySpan<byte>)vertexBytes);
    public void WriteVertices(byte[] vertexBytes) => VertexSegment.Write(0, vertexBytes);
}