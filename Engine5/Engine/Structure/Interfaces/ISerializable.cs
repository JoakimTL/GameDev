namespace Engine.Structure.Interfaces;

public interface ISerializable : ITypeIdentity
{
    static abstract object Deserialize(ReadOnlySpan<byte> data);
    ReadOnlySpan<byte> Serialize();
}
