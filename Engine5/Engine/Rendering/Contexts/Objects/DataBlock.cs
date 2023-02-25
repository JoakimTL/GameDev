using Engine.Structure.Interfaces.Buffers;
using OpenGL;
using System.Runtime.CompilerServices;
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
    public IBufferSegment Segment { get; private set; }
    /// <summary>
    /// The shader type this block is found in.
    /// </summary>
    public IReadOnlyList<ShaderType> ShaderTypes => _shaderTypes;
    public uint BoundIndex { get; private set; }
    private readonly byte* _bytes;
    private readonly VertexBufferObject _buffer;
    protected readonly List<ShaderType> _shaderTypes;
#if DEBUG
    public Memory<byte> Bytes => DebugUtilities.PointerToMemory(_bytes, (uint)Segment.SizeBytes);
#endif

    protected abstract BufferTarget Target { get; }

    public DataBlock(SegmentedVertexBufferObject buffer, ShaderType[] shaderTypes, string blockName, uint sizeBytes, uint alignmentBytes, uint maxSizeBytes = 0) : base(blockName)
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
        _buffer = buffer;
        Segment = buffer.Allocate(sizeBytes) ?? throw new Exception("Allocate data blocks only on the context thread.");
        _bytes = (byte*)NativeMemory.Alloc(sizeBytes);
        _shaderTypes = new List<ShaderType>(shaderTypes);
    }

#if DEBUG
    ~DataBlock()
    {
        System.Diagnostics.Debug.Fail("Data block was not disposed!");
    }
#endif

    public abstract void BindShader(ShaderProgramBase p);

    protected void BindBuffer(uint index)
    {
        BoundIndex = index;
        Gl.BindBufferRange(Target, BoundIndex, _buffer.BufferId, (nint)Segment.OffsetBytes, (uint)Segment.SizeBytes);
    }

    public void UnbindBuffer() => Gl.BindBufferRange(Target, BoundIndex, 0, nint.Zero, 0);

    public void Write<T>(T t, uint offsetBytes = 0) where T : unmanaged
    {
        if ((uint)Marshal.SizeOf<T>() + offsetBytes > Segment.SizeBytes)
        {
            this.LogError("Can't write outsite block.");
            return;
        }
        ((T*)(_bytes + offsetBytes))[0] = t;
        _buffer.Write(new nint(_bytes), (uint)Segment.OffsetBytes + offsetBytes, offsetBytes, (uint)Marshal.SizeOf<T>());
    }

    public void Write<T>(Span<T> data, uint offsetBytes = 0) where T : unmanaged
    {
        if ((uint)(Marshal.SizeOf<T>() * data.Length) + offsetBytes > Segment.SizeBytes)
        {
            this.LogError("Can't write outsite block.");
            return;
        }
        fixed (T* src = data)
            Unsafe.CopyBlock(_bytes + offsetBytes, src, (uint)(data.Length * Marshal.SizeOf<T>()));
        _buffer.Write(new nint(_bytes), (uint)Segment.OffsetBytes + offsetBytes, offsetBytes, (uint)(data.Length * Marshal.SizeOf<T>()));
    }

    public void Write(void* src, uint length, uint offsetBytes = 0)
    {
        if (length + offsetBytes > Segment.SizeBytes)
        {
            this.LogError("Can't write outsite block.");
            return;
        }
        Unsafe.CopyBlock(_bytes + offsetBytes, src, length);
        _buffer.Write(new nint(_bytes), (uint)Segment.OffsetBytes + offsetBytes, offsetBytes, length);
    }

    public void Dispose()
    {
        NativeMemory.Free(_bytes);
        GC.SuppressFinalize(this);
    }
}
