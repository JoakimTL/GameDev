using Engine.Module.Entities.Container;

namespace Civlike.World.Components;

public sealed class GlobeArchetype : ArchetypeBase {
	public GlobeComponent GlobeComponent { get; set; } = null!;
}