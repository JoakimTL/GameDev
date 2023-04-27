using Engine.Structure.Interfaces.Buffers;
using OpenGL;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Contexts.Objects;

public abstract unsafe class DataBlock : Identifiable, IDisposable
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
    private readonly SegmentedVertexBufferObject _svbo;
    private IBufferSegment _segment;
    private void* _dataPointer;
    private uint _sizeBytes;
    private bool _disposed;
    protected readonly List<ShaderType> _shaderTypes;

    public event Action<DataBlock>? Disposed;

    protected abstract BufferTarget Target { get; }

    public DataBlock(SegmentedVertexBufferObject svbo, ShaderType[] shaderTypes, string blockName, uint sizeBytes, uint alignmentBytes, uint maxSizeBytes = 0) : base(blockName)
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
        _svbo = svbo;
        _dataPointer = NativeMemory.Alloc(sizeBytes);
        _sizeBytes = sizeBytes;
        _disposed = false;
        _segment = _svbo.Allocate(sizeBytes) ?? throw new Exception("Allocate data blocks only on the context thread.");
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
        Gl.BindBufferRange(Target, BoundIndex, _svbo.BufferId, (nint)Segment.OffsetBytes, (uint)Segment.SizeBytes);
    }

    public void UnbindBuffer() => Gl.BindBufferRange(Target, BoundIndex, 0, nint.Zero, 0);

    public void Write<T>(T t, uint offsetBytes = 0) where T : unmanaged => Write(&t, (uint)sizeof(T), offsetBytes);

    public void Write<T>(Span<T> data, uint offsetBytes = 0) where T : unmanaged
    {
        fixed (T* srcPtr = data)
            Write(srcPtr, (uint)(sizeof(T) * data.Length), offsetBytes);
    }

    public void Write(void* src, uint lengthBytes, uint offsetBytes = 0)
    {
        if (_disposed)
            throw new ObjectDisposedException(this.ToString());
        System.Buffer.MemoryCopy(src, (byte*)_dataPointer + offsetBytes, _sizeBytes - offsetBytes, lengthBytes);
        _svbo.Write((nint)_dataPointer, (uint)_segment.OffsetBytes, offsetBytes, lengthBytes);
    }

    public void Dispose()
    {
        NativeMemory.Free(_dataPointer);
        Disposed?.Invoke(this);
        GC.SuppressFinalize(this);
    }
}
