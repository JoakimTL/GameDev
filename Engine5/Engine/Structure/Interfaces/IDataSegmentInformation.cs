namespace Engine.Structure.Interfaces;

public interface IDataSegmentInformation {
	public ulong OffsetBytes { get; }
	public uint SizeBytes { get; }
	public event Action<ulong>? OffsetChanged;
}
