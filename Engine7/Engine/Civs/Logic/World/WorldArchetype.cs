using Engine.Module.Entities.Container;
using Engine.Standard.Entities.Components;

namespace Civs.Logic.World;

public sealed class WorldArchetype : ArchetypeBase {
	public GlobeComponent GlobeComponent { get; set; } = null!;
	public Transform3Component Transform { get; set; } = null!;
}