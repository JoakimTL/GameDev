using Engine.Logging;

namespace Civs.Logic.Setup;

public abstract class ResourceTypeBase : SelfIdentifyingBase {
	public string Name { get; }

	protected ResourceTypeBase( string name ) {
		this.Name = name;
	}
}

