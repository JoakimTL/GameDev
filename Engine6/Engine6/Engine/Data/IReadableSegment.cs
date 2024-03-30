namespace Engine.Data;

public interface IReadableSegment : IReadableBuffer {
	uint SizeBytes { get; }
	nuint OffsetBytes { get; }
}