using Engine.Datatypes.Buffers;
using Engine.Structure.Interfaces.Buffers;

namespace Engine.Rendering.Contexts.Objects.Meshes;

public sealed class LoadedAssetMesh : Identifiable, IMesh, IDisposable
{
	public Type VertexType { get; }
	private readonly ISegmentedBufferSegment _vertexSegment;
	private readonly uint _vertexSize;
	private readonly ISegmentedBufferSegment _elementSegment;
	private readonly uint _elementSize;

	public uint ElementCount => (uint)_elementSegment.OffsetBytes / _elementSize;

	public uint ElementOffset => (uint)_elementSegment.SizeBytes / _elementSize;

	public uint VertexOffset => (uint)_vertexSegment.OffsetBytes / _vertexSize;

	public event Action? Changed;

	public LoadedAssetMesh(Type vertexType, ISegmentedBufferSegment vertexSegment, uint vertexSize, ISegmentedBufferSegment elementSegment, uint elementSize)
	{
		VertexType = vertexType;
		_vertexSegment = vertexSegment;
		_vertexSize = vertexSize;
		_vertexSegment.OffsetChanged += OnChanged;
		_elementSegment = elementSegment;
		_elementSize = elementSize;
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
