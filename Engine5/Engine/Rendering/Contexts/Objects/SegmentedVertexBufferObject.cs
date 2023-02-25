using Engine.Rendering.OGL;
using Engine.Structure.Interfaces.Buffers;
using OpenGL;

namespace Engine.Rendering.Contexts.Objects;

public sealed class SegmentedVertexBufferObject : VertexBufferObject
{

    public readonly uint ByteAlignment;
    private ulong _allocatedBytes;
    private readonly List<Segment> _segments;

    public SegmentedVertexBufferObject(string name, uint sizeBytes, BufferUsage usage, uint segmentByteAlignment = sizeof(byte)) : base(name, sizeBytes, usage)
    {
        if (segmentByteAlignment == 0)
            throw new ArgumentOutOfRangeException(nameof(segmentByteAlignment), "Must be greater than zero!");
        _allocatedBytes = 0;
        _segments = new List<Segment>();
        ByteAlignment = segmentByteAlignment;
    }

    public IBufferSegment? Allocate(uint sizeBytes)
    {
        if (sizeBytes % ByteAlignment != 0)
        {
            uint newSizeBytes = (sizeBytes / ByteAlignment + 1) * ByteAlignment;
            this.LogWarning($"Attempted to allocate segment outside alignment. Adjusting from {sizeBytes}B to {newSizeBytes}B!");
            sizeBytes = newSizeBytes;
        }

        while (_allocatedBytes + sizeBytes > SizeBytes)
            this.Resize(SizeBytes * 2);

        Segment segment = new(_allocatedBytes, sizeBytes);
        _segments.Add(segment);
        _allocatedBytes += segment.SizeBytes;
        return segment;
    }

    private class Segment : Identifiable, IBufferSegment
    {
        public ulong OffsetBytes { get; private set; }
        public ulong SizeBytes { get; private set; }
        public event Action<uint>? OffsetChanged;

        protected override string UniqueNameTag => $"{OffsetBytes}->{SizeBytes / 1024d}KiB";

        internal Segment(ulong offsetBytes, ulong sizeBytes)
        {
            SizeBytes = sizeBytes;
            OffsetBytes = offsetBytes;
        }
    }

}
