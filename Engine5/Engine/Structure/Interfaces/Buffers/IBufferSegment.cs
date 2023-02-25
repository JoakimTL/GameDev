namespace Engine.Structure.Interfaces.Buffers;

public interface IBufferSegment : IBuffer {
	ulong OffsetBytes { get; }
}
