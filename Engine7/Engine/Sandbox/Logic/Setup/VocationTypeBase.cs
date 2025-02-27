namespace Sandbox.Logic.Setup;

public abstract class VocationTypeBase : SelfIdentifyingBase {
	protected VocationTypeBase( string name ) {
		this.Name = name;
	}
	public string Name { get; }
}
