using Engine.Module.Entities.Container;

namespace Sandbox.Logic.World;

public sealed class WorldArchetype : ArchetypeBase {
	public WorldComponent WorldComponent { get; set; } = null!;
	public WorldTilingComponent WorldTilingComponent { get; set; } = null!;
}
