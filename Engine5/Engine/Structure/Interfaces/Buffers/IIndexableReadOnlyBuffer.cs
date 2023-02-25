namespace Engine.Structure.Interfaces.Buffers;

public interface IIndexableReadOnlyBuffer<T> : IBuffer where T : unmanaged {
	ulong ElementCount { get; }
	T this[ ulong index ] { get; }
}
