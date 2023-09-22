namespace Engine.Modules.Entity;

[AttributeUsage( AttributeTargets.Class )]
public sealed class IdentifierAttribute : Attribute {

	public Guid Identifier { get; }

	public IdentifierAttribute( Guid identifier ) {
		this.Identifier = identifier;
	}
}