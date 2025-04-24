namespace Sandbox.Logic.Setup;
public abstract class ResourceTypeBase : SelfIdentifyingBase {
	protected ResourceTypeBase( string name ) {
		this.Name = name;
	}

	public string Name { get; }
}

public sealed record ResourceAmount( ResourceTypeBase Resource, double AmountKg ) {
	public double AmountKg { get; set; } = AmountKg;
}