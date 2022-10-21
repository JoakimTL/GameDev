namespace Engine;

public interface IExtendableBuffer : IBuffer {
	void Extend( ulong bytes );
}