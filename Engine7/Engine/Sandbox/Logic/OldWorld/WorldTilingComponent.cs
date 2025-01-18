using Engine.Module.Entities.Container;
using Sandbox.Logic.OldWorld.Tiles.Generation;

namespace Sandbox.Logic.OldWorld;

public sealed class WorldTilingComponent : ComponentBase {
	public WorldTiling Tiling { get; } = new();
}
