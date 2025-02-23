namespace Sandbox.Logic.Setup;
public abstract class ResourceBase : SelfIdentifyingBase {
	protected ResourceBase(string name) {
		this.Name = name;
	}

	public string Name { get; }
}

public sealed record ResourceAmount( ResourceBase Resource, double AmountKg ) {
	public double AmountKg { get; set; } = AmountKg;
}