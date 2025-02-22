using Engine.Module.Entities.Container;
using Sandbox.Logic.Old.OldWorld.Tiles.Generation;

namespace Sandbox.Logic.Old.OldWorld;

public sealed class WorldTilingComponent : ComponentBase {
	public WorldTiling Tiling { get; } = new();
}
