namespace Engine;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = false )]
public sealed class IdentityAttribute( string identity ) : Attribute {
	public string Identity { get; } = identity;
}
