namespace Sandbox.Logic.Setup;

public abstract class SectorTypeBase : SelfIdentifyingBase {
	protected SectorTypeBase( string name ) {
		this.Name = name;
	}
	public string Name { get; }
}
