using Engine.Module.Entities.Container;

namespace Civlike.Logic.World;

public sealed class WorldClusterArchetype : ArchetypeBase {
	public BoundedRenderClusterComponent ClusterComponent { get; set; } = null!;
}
