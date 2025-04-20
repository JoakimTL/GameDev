using Engine.Module.Entities.Container;
using Engine.Standard.Entities.Components;

namespace Civs.Logic.World;

public sealed class WorldClusterArchetype : ArchetypeBase {
	public TileEdgeClusterComponent ClusterComponent { get; set; } = null!;
	public Transform3Component Transform { get; set; } = null!;
}