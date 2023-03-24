namespace Engine.Structure.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class IdentityAttribute : Attribute
{
    public string Identity { get; }

    public IdentityAttribute(string identity)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
    }
}