using Engine.Modularity.Domains;

namespace Engine;

public abstract class DomainService : DisposableIdentifiable {
	protected Domain Domain { get; private set; } = null!;
}
public abstract class ModuleService : DisposableIdentifiable {
	protected Module Module { get; private set; } = null!;
}
