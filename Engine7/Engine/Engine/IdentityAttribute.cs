namespace Engine;

[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false )]
public sealed class IdentityAttribute( string identity ) : Attribute {
	public string Identity { get; } = identity;
}
