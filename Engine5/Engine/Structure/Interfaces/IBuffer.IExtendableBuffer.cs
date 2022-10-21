namespace Engine.Structure.Interfaces;

public interface IExtendableBuffer : IBuffer
{
    void Extend(ulong bytes);
}