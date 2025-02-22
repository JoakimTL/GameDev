namespace Sandbox.Logic.Old.OldCiv.Research;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
public sealed class SubfieldOfAttribute<T> : Attribute, ISubfieldOf where T : TechnologyFieldBase {
	public Type SubfieldOfType { get; } = typeof( T );
}
