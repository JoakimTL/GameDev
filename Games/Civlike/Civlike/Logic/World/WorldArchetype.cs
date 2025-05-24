using Engine.Module.Entities.Container;

namespace Civlike.Logic.World;

public sealed class WorldArchetype : ArchetypeBase {
	public GlobeComponent GlobeComponent { get; set; } = null!;
}