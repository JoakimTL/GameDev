using Engine.Module.Entities.Container;
using Engine.Standard.Entities.Components;

namespace Civlike.Logic.World;

public sealed class WorldClusterArchetype : ArchetypeBase {
	public BoundedRenderClusterComponent ClusterComponent { get; set; } = null!;
}
