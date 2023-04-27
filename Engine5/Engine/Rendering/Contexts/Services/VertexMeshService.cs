using Engine.Rendering.Contexts.Objects;
using Engine.Rendering.Contexts.Objects.Meshes;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Contexts.Services;

public sealed class VertexMeshService : Identifiable, IContextService
{
    private readonly RenderBufferObjectService _renderBufferObjectService;

    public VertexMeshService(RenderBufferObjectService renderBufferObjectService)
    {
        _renderBufferObjectService = renderBufferObjectService;
    }

    public VertexMesh<VertexT>? Create<VertexT>(string name, uint vertexCount, uint elementCount) where VertexT : unmanaged
    {
        uint vertexSizeBytes = (uint)Marshal.SizeOf<VertexT>();
        var vertexSegment = _renderBufferObjectService.Get(typeof(VertexT)).AllocateSegment(vertexCount * vertexSizeBytes);
        if (vertexSegment is null)
            return this.LogWarningThenReturnDefault<VertexMesh<VertexT>>("Unable to create vertex segment");
        var elementSegment = _renderBufferObjectService.ElementBuffer.AllocateSegment(elementCount * IMesh.ElementSizeBytes);
        if (elementSegment is null)
        {
            vertexSegment.Dispose();
            return this.LogWarningThenReturnDefault<VertexMesh<VertexT>>("Unable to create element segment");
        }
        return new VertexMesh<VertexT>(name, vertexSegment, vertexSizeBytes, elementSegment);
    }

    public VertexMesh<VertexT>? Create<VertexT>(string name, Span<VertexT> vertices, Span<uint> indices) where VertexT : unmanaged
    {
        var mesh = Create<VertexT>(name, (uint)vertices.Length, (uint)indices.Length);
        if (mesh is null)
            return null;
        mesh.WriteVertices(vertices);
        mesh.WriteElements(indices);
        return mesh;
    }

    public VertexMesh<VertexT>? Create<VertexT>(string name, VertexT[] vertices, uint[] indices) where VertexT : unmanaged
    {
        var mesh = Create<VertexT>(name, (uint)vertices.Length, (uint)indices.Length);
        if (mesh is null)
            return null;
        mesh.WriteVertices(vertices);
        mesh.WriteElements(indices);
        return mesh;
    }
}