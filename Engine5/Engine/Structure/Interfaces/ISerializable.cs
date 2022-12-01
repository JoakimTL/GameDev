namespace Engine.Structure.Interfaces;

public interface ISerializable<TSelf> : ITypeIdentity
{
    static abstract TSelf Deserialize(ReadOnlySpan<byte> data);
    ReadOnlySpan<byte> Serialize();
}
