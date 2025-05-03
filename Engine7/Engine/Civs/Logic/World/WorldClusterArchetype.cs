using Engine.Module.Entities.Container;
using Engine.Standard.Entities.Components;

namespace Civs.Logic.World;

public sealed class WorldClusterArchetype : ArchetypeBase {
	public BoundedRenderClusterComponent ClusterComponent { get; set; } = null!;
}
