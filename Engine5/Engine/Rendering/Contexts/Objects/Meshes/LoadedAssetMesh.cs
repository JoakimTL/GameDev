using Engine.Datatypes.Buffers;

namespace Engine.Rendering.Contexts.Objects.Meshes;

public sealed class LoadedAssetMesh : Identifiable, IMesh, IDisposable
{
    public string MeshDataAssetName { get; }
    public Type VertexType { get; }
    private readonly ISegmentedBufferSegment _vertexSegment;
    private readonly uint _vertexSize;
    private readonly ISegmentedBufferSegment _elementSegment;

    public uint ElementCount => (uint)_elementSegment.SizeBytes / IMesh.ElementSizeBytes;

    public uint ElementOffset => (uint)_elementSegment.OffsetBytes / IMesh.ElementSizeBytes;

    public uint VertexOffset => (uint)_vertexSegment.OffsetBytes / IMesh.ElementSizeBytes;

    public event Action? Changed;

    public LoadedAssetMesh(string meshDataName, Type vertexType, ISegmentedBufferSegment vertexSegment, uint vertexSize, ISegmentedBufferSegment elementSegment)
    {
        MeshDataAssetName = meshDataName;
        VertexType = vertexType;
        _vertexSegment = vertexSegment;
        _vertexSize = vertexSize;
        _vertexSegment.OffsetChanged += OnChanged;
        _elementSegment = elementSegment;
        _elementSegment.OffsetChanged += OnChanged;
    }

    private void OnChanged(object segment, ulong newOffsetBytes) => Changed?.Invoke();

#if DEBUG
    ~LoadedAssetMesh()
    {
        System.Diagnostics.Debug.Fail($"{this} was not disposed!");
    }
#endif

    public void Dispose()
    {
        _vertexSegment.Dispose();
        _elementSegment.Dispose();
        GC.SuppressFinalize(this);
    }
}
