namespace Sandbox.Logic.Setup;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
public sealed class RequiresAttribute<T> : Attribute, IRequirement {
	public Type RequiredType { get; } = typeof( T );
}
