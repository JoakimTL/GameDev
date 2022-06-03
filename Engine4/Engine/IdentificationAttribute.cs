namespace Engine;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
public class IdentificationAttribute : Attribute {

	public readonly Guid Guid;

	public IdentificationAttribute( string guid ) {
		this.Guid = new Guid( guid );
	}
}