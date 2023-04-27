using Engine.Datatypes.Buffers;

namespace Engine.Rendering.Contexts.Objects.Meshes;

public abstract class BufferedMeshBase : Identifiable, IMesh, IDisposable
{
    protected ISegmentedBufferSegment VertexSegment { get; }
    public uint VertexSizeBytes { get; }
    protected ISegmentedBufferSegment ElementSegment { get; }

    public Type VertexType { get; }

    public uint ElementCount { get; private set; }

    public uint ElementOffset { get; private set; }

    public uint VertexOffset { get; private set; }

    public event Action? Changed;

    public BufferedMeshBase(string name, Type vertexType, ISegmentedBufferSegment vertexSegment, uint vertexSizeBytes, ISegmentedBufferSegment elementSegment) : base(name)
    {
        VertexType = vertexType;
        VertexSegment = vertexSegment;
        VertexSizeBytes = vertexSizeBytes;
        ElementSegment = elementSegment;
        VertexSegment.OffsetChanged += OnOffsetChanged;
        ElementSegment.OffsetChanged += OnOffsetChanged;
        UpdateMeshInfo();
    }

#if DEBUG
    ~BufferedMeshBase()
    {
        System.Diagnostics.Debug.Fail($"{this} was not disposed!");
    }
#endif

    private void OnOffsetChanged(object segment, ulong newOffsetBytes)
    {
        UpdateMeshInfo();
        Changed?.Invoke();
    }

    private void UpdateMeshInfo()
    {
        ElementCount = (uint)(ElementSegment.SizeBytes / IMesh.ElementSizeBytes);
        ElementOffset = (uint)(ElementSegment.OffsetBytes / IMesh.ElementSizeBytes);
        VertexOffset = (uint)(VertexSegment.OffsetBytes / VertexSizeBytes);
    }

    public void Dispose()
    {
        VertexSegment.Dispose();
        ElementSegment.Dispose();
        GC.SuppressFinalize(this);
    }
}
