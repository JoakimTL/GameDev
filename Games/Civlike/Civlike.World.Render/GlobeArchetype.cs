using Civlike.World.Components;
using Engine.Module.Entities.Container;

namespace Civlike.World.Render;

public sealed class GlobeArchetype : ArchetypeBase {
	public GlobeComponent GlobeComponent { get; set; } = null!;
}
