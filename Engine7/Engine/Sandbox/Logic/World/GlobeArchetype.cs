using Engine.Module.Entities.Container;

namespace Sandbox.Logic.World;

public sealed class GlobeArchetype : ArchetypeBase {
	public GlobeComponent GlobeComponent { get; set; } = null!;
}
