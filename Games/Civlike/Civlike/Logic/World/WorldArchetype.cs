using Engine.Module.Entities.Container;
using Engine.Standard.Entities.Components;

namespace Civlike.Logic.World;

public sealed class WorldArchetype : ArchetypeBase {
	public GlobeComponent GlobeComponent { get; set; } = null!;
}