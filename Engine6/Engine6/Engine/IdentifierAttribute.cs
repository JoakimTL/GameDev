namespace Engine;

[AttributeUsage( AttributeTargets.Class )]
public sealed class IdentifierAttribute : Attribute {

	public Guid Identifier { get; }

	public IdentifierAttribute( string identifier ) {
		this.Identifier = new Guid( identifier );
	}
}