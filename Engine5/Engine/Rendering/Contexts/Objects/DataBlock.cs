using Engine.Datatypes.Buffers;
using Engine.Structure.Interfaces.Buffers;
using OpenGL;

namespace Engine.Rendering.Contexts.Objects;

public abstract class DataBlock : Identifiable, IDisposable
{

	/// <summary>
	/// The name of the block.
	/// </summary>
	public string BlockName { get; private set; }
	/// <summary>
	/// The segment in the databuffer.
	/// </summary>
	private IBufferSegment Segment => _segment;
	/// <summary>
	/// The shader type this block is found in.
	/// </summary>
	public IReadOnlyList<ShaderType> ShaderTypes => _shaderTypes;
	public uint BoundIndex { get; private set; }
	private readonly RenderBufferObject _rbo;
	private readonly VertexBufferObject _vbo;
	private ISegmentedBufferSegment _segment;
	protected readonly List<ShaderType> _shaderTypes;

	public event Action<DataBlock>? Disposed;

	protected abstract BufferTarget Target { get; }

	public DataBlock(RenderBufferObject rbo, VertexBufferObject vbo, ShaderType[] shaderTypes, string blockName, uint sizeBytes, uint alignmentBytes, uint maxSizeBytes = 0) : base(blockName)
	{
		if (maxSizeBytes > 0 && sizeBytes > maxSizeBytes)
			throw new ArgumentOutOfRangeException($"The argument 'size' exceeds the block size limit of {maxSizeBytes}!");
		if (alignmentBytes > 0 && sizeBytes % alignmentBytes != 0)
		{
			uint newSize = (sizeBytes / alignmentBytes + 1) * alignmentBytes;
			Log.Line($"Size [{sizeBytes}] of uniform block {FullName} is not aligned. Size changed to {newSize}!", Log.Level.NORMAL);
			sizeBytes = newSize;
		}
		BlockName = blockName;
		_vbo = vbo;
		_rbo = rbo;
		_segment = _rbo.AllocateSegment(sizeBytes) ?? throw new Exception("Allocate data blocks only on the context thread.");
		_shaderTypes = new List<ShaderType>(shaderTypes);
	}

#if DEBUG
	~DataBlock()
	{
		System.Diagnostics.Debug.Fail($"{this} was not disposed!");
	}
#endif

	public abstract void BindShader(ShaderProgramBase p);

	protected void BindBuffer(uint index)
	{
		BoundIndex = index;
		Gl.BindBufferRange(Target, BoundIndex, _vbo.BufferId, (nint)Segment.OffsetBytes, (uint)Segment.SizeBytes);
	}

	public void UnbindBuffer() => Gl.BindBufferRange(Target, BoundIndex, 0, nint.Zero, 0);

	public void Write<T>(T t, uint offsetBytes = 0) where T : unmanaged => _segment.Write(offsetBytes, t);

	public void Write<T>(Span<T> data, uint offsetBytes = 0) where T : unmanaged => _segment.Write<T>(offsetBytes, data);

	public unsafe void Write(void* src, uint length, uint offsetBytes = 0) => _segment.Write(offsetBytes, src, length);

	public void Dispose()
	{
		_segment.Dispose();
		Disposed?.Invoke(this);
		GC.SuppressFinalize(this);
	}
}
