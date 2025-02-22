using Engine.Module.Entities.Container;

namespace Sandbox.Logic.Old.OldWorld;

public sealed class WorldSelectedTileArchetype : ArchetypeBase {
	public WorldComponent WorldComponent { get; set; } = null!;
	public WorldTilingComponent WorldTilingComponent { get; set; } = null!;
	public WorldSelectedTileComponent WorldSelectedTileComponent { get; set; } = null!;
}